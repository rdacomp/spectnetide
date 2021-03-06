using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.SolutionItems;
using Spect.Net.VsPackage.VsxLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// This class contains the code that carries out disassembly export
    /// </summary>
    public class DisassemblyExporter
    {
        private const string FILE_EXISTS_MESSAGE = "The disassembly export file exists in the project. " +
                                                   "Would you like to override it?";

        private const string INVALID_FOLDER_MESSAGE = "The disassembly export folder specified in the Options dialog " +
                                                      "contains invalid characters or an absolute path. Go to the Options dialog and " +
                                                      "fix the issue so that you can add the exported file to the project.";

        /// <summary>
        /// Instantiates this object with the specified set of export parameters
        /// </summary>
        /// <param name="exportParams">Export parameters collected from the dialog</param>
        /// <param name="parentViewModel">Disassembly parent view model</param>
        public DisassemblyExporter(ExportDisassemblyViewModel exportParams, IDisassemblyItemParent parentViewModel)
        {
            ExportParams = exportParams;
            ParentViewModel = parentViewModel;
            MaxLineLength = GetMaxLineLength();
            InstructionIndent = new string(' ', GetIndentSpacesCount());
            InitCommentProperties();
        }

        public IDisassemblyItemParent ParentViewModel { get; }

        /// <summary>
        /// Export disassembly parameters
        /// </summary>
        public ExportDisassemblyViewModel ExportParams { get; }

        /// <summary>
        /// The maximum length of an output line
        /// </summary>
        public int MaxLineLength { get; }

        /// <summary>
        /// The string (spaces) used to indent instructions
        /// </summary>
        public string InstructionIndent { get; }

        /// <summary>
        /// Number of comment token characters
        /// </summary>
        public int CommentCharCount { get; private set; }

        /// <summary>
        /// The beginning character of a comment
        /// </summary>
        public string CommentBegins { get; private set; }

        /// <summary>
        /// The ending character of a comment
        /// </summary>
        public string CommentEnds { get; private set; }

        /// <summary>
        /// Exports the disassembly using the specified disassembler object
        /// </summary>
        /// <param name="disassembler">Disassembler set up for annotations</param>
        public void ExportDisassembly(Z80Disassembler disassembler)
        {
            // --- Create the disassembly output
            if (!ushort.TryParse(ExportParams.StartAddress, out var startAddress)) return;
            if (!ushort.TryParse(ExportParams.EndAddress, out var endAddress)) return;
            var output = disassembler.Disassemble(startAddress, endAddress);
            var equs = new Dictionary<string, ushort>();
            var items = new List<DisassemblyItemViewModel>();
            var addresses = new HashSet<ushort>();

            // --- Create the set of addresses that may be referred through labels
            foreach (var outputItem in output.OutputItems)
            {
                addresses.Add(outputItem.Address);
            }

            // --- Collect all external labels and symbols
            foreach (var outputItem in output.OutputItems)
            {
                items.Add(new DisassemblyItemViewModel(ParentViewModel, outputItem));
                if (outputItem.HasLabelSymbol)
                {
                    // --- Check if it is an external label
                    if (outputItem.SymbolValue >= startAddress && outputItem.SymbolValue <= endAddress)
                    {
                        continue;
                    }
                    if (ParentViewModel.GetLabel(outputItem.SymbolValue, out var extLabel))
                    {
                        equs[extLabel] = outputItem.SymbolValue;
                    }
                }
                // --- Check if literal replacement
                else if (ParentViewModel.GetLiteralReplacement(outputItem.Address, out var literal))
                {
                    equs[literal] = outputItem.SymbolValue;
                }
            }

            // --- Create the exported contents item by item
            var contents = new StringBuilder(4000);

            // --- Export the origin
            contents.AppendLine($"{InstructionIndent}.org #{startAddress:X4}");
            contents.AppendLine();

            // --- Create .EQU lines for external labels and symbols
            contents.AppendLine($"{CommentBegins}External symbols{CommentEnds}");
            foreach (var symbolKey in equs.Keys.OrderBy(k => k))
            {
                if (ExportParams.HangingLabels)
                {
                    contents.AppendLine($"{symbolKey}:");
                    contents.AppendLine($"{InstructionIndent}.equ #{equs[symbolKey]:X4}");
                }
                else
                {
                    contents.AppendLine($"{symbolKey}: .equ #{equs[symbolKey]:X4}");
                }
            }
            contents.AppendLine();

            // --- Iterate output items
            foreach (var item in items)
            {
                var lineContents = new StringBuilder(200);

                // --- Handle prefix comment
                if (item.HasPrefixComment)
                {
                    contents.AppendLine();
                    OutputComment(item.PrefixCommentFormatted, contents, MaxLineLength - CommentCharCount, "");
                }

                // --- Handle label
                if (!string.IsNullOrEmpty(item.LabelFormatted))
                {
                    if (ExportParams.HangingLabels)
                    {
                        // --- Hanging label to a separate line
                        contents.AppendLine($"{item.LabelFormatted}");
                        lineContents.Append(InstructionIndent);
                    }
                    else
                    {
                        // ---Label goes to the instruction line
                        lineContents.Append($"{item.LabelFormatted} ");
                    }
                }
                else
                {
                    // --- Instruction only
                    lineContents.Append(InstructionIndent);
                }

                // --- Instruction part: take care labels that cannot be accessed through instructions
                var instruction = item.InstructionFormatted;
                var instrItem = item.Item;
                if (item.Item.HasLabelSymbol && !addresses.Contains(item.Item.SymbolValue))
                {
                    if (instrItem.SymbolValue >= startAddress && instrItem.SymbolValue <= endAddress
                        || !ParentViewModel.GetLabel(instrItem.SymbolValue, out _))
                    {
                        // --- Internal or external label without associated symbol
                        // --- Change the disassembly label name to the corresponding address
                        var addressString = $"#{instrItem.SymbolValue:X4}";
                        instruction = instrItem.Instruction.Substring(0, instrItem.TokenPosition)
                                      + addressString
                                      + instrItem.Instruction.Substring(instrItem.TokenPosition + instrItem.TokenLength);
                    }
                }
                lineContents.Append(instruction);

                // --- Handle line comment
                if (!string.IsNullOrEmpty(item.CommentFormatted))
                {
                    var maxCommentLength = MaxLineLength - CommentCharCount - lineContents.Length - 1;
                    if (maxCommentLength < 20)
                    {
                        // --- Comment does not fit into this line
                        contents.AppendLine(lineContents.ToString());
                        OutputComment(item.CommentFormatted, contents,
                            MaxLineLength - CommentCharCount - InstructionIndent.Length,
                            InstructionIndent);
                    }
                    else
                    {
                        // --- Comment fits into this line
                        lineContents.Append(" ");
                        OutputComment(item.CommentFormatted, contents, maxCommentLength, lineContents.ToString());
                    }
                }
                else
                {
                    // --- Output the remainder of the line
                    if (lineContents.Length > 0)
                    {
                        contents.AppendLine(lineContents.ToString());
                    }
                }
            }

            // --- Save the file
            try
            {
                var dirName = Path.GetDirectoryName(ExportParams.Filename);
                if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                File.WriteAllText(ExportParams.Filename, contents.ToString());
            }
            catch (Exception ex)
            {
                VsxDialogs.Show($"Error while exporting to file {ExportParams.Filename}: {ex.Message}",
                    "Export disassembly error.", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                return;
            }
            if (!ExportParams.AddToProject) return;

            // --- Step #6: Add the saved item to the project
            // --- Check path segment names
            SpectrumProject.AddFileToProject(SpectNetPackage.Default.Options.DisassExportFolder,
                ExportParams.Filename,
                INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE);
        }

        /// <summary>
        /// Sends the prefix comment to the output
        /// </summary>
        /// <param name="comment">Comment to display</param>
        /// <param name="contents">Contents stream</param>
        /// <param name="maxLength">Maximum line length</param>
        /// <param name="linePrefix">Line prefix to add for the first line</param>
        private void OutputComment(string comment, StringBuilder contents, int maxLength, string linePrefix)
        {
            var parts = comment.Split(' ');
            var emptySpace = new string(' ', linePrefix?.Length ?? 0);
            var commentBuilder = new StringBuilder();
            var lineBegins = true;
            foreach (var part in parts)
            {
                // --- Skip end-of-line spaces
                var word = part;
                if (lineBegins && word == "") continue;

                // --- Handle long words
                if (commentBuilder.Length + word.Length > maxLength)
                {
                    if (commentBuilder.Length == 0)
                    {
                        // --- This is a word longer than the line
                        while (word.Length > maxLength)
                        {
                            contents.AppendLine($"{linePrefix}{CommentBegins}{word.Substring(0, maxLength).TrimEnd()}{CommentEnds}");
                            linePrefix = emptySpace;
                            word = word.Substring(maxLength);
                        }
                        if (word.Length == 0) continue;

                        // --- Remainder of the word goes for the next line
                        commentBuilder.Clear();
                        commentBuilder.Append(word);
                        commentBuilder.Append(" ");
                        lineBegins = true;
                    }
                    else
                    {
                        // --- Output the current line
                        contents.AppendLine($"{linePrefix}{CommentBegins}{commentBuilder.ToString().TrimEnd()}{CommentEnds}");
                        linePrefix = emptySpace;

                        // --- This word goes for the next line
                        commentBuilder.Clear();
                        commentBuilder.Append(word);
                        commentBuilder.Append(" ");
                        lineBegins = true;
                    }
                }
                else
                {
                    // --- Word fits to the end
                    commentBuilder.Append(word);
                    commentBuilder.Append(" ");
                    lineBegins = false;
                }
            }

            // --- Output the remainder of the line
            if (commentBuilder.Length > 0)
            {
                contents.AppendLine($"{linePrefix}{CommentBegins}{commentBuilder.ToString().TrimEnd()}{CommentEnds}");
            }
        }

        /// <summary>
        /// Gets the maximum length of the output line
        /// </summary>
        /// <returns></returns>
        private int GetMaxLineLength()
        {
            switch (ExportParams.MaxLineLengthType)
            {
                case LineLengthType.NoBreak:
                    return 1024 * 1024;
                case LineLengthType.L60:
                    return 60;
                case LineLengthType.L80:
                    return 80;
                case LineLengthType.L100:
                    return 100;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Gets the number of characters taken by comment token characters
        /// </summary>
        /// <returns></returns>
        private void InitCommentProperties()
        {
            switch (ExportParams.CommentStyle)
            {
                case CommentStyle.Semicolon:
                    CommentCharCount = 2;
                    CommentBegins = "; ";
                    CommentEnds = string.Empty;
                    return;
                case CommentStyle.DoubleSlash:
                    CommentCharCount = 3;
                    CommentBegins = "// ";
                    CommentEnds = string.Empty;
                    return;
                case CommentStyle.Block:
                    CommentCharCount = 6;
                    CommentBegins = "/* ";
                    CommentEnds = " */";
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetIndentSpacesCount()
        {
            switch (ExportParams.IndentDepth)
            {
                case IndentDepthType.Zero:
                    return 0;
                case IndentDepthType.Two:
                    return 2;
                case IndentDepthType.Four:
                    return 4;
                case IndentDepthType.Eight:
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
