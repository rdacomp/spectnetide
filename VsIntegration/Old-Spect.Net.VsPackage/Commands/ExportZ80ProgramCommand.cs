using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Devices.Tape.Tzx;
using Spect.Net.VsPackage.ProjectStructure;
using Spect.Net.VsPackage.Vsx;
using Spect.Net.VsPackage.Z80Programs.Commands;
using Spect.Net.VsPackage.Z80Programs.Export;
using Task = System.Threading.Tasks.Task;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Export a Z80 program command
    /// </summary>
    [CommandId(0x0802)]
    public class ExportZ80ProgramCommand : Z80CompileCodeCommandBase
    {
        private const string FILE_EXISTS_MESSAGE = "The exported tape file exists in the project. " +
                                                   "Would you like to override it?";

        private const string INVALID_FOLDER_MESSAGE = "The tape folder specified in the Options dialog " +
                                                      "contains invalid characters or an absolute path. Go to the Options dialog and " +
                                                      "fix the issue so that you can add the tape file to the project.";
        
        protected bool Success { get; set; }

        /// <summary>
        /// Compiles the Z80 code file
        /// </summary>
        protected override async Task ExecuteAsync()
        {
            // --- Prepare the appropriate file to export
            Success = true;
            GetCodeItem(out var hierarchy, out var itemId);

            // --- Step #1: Compile the code
            if (!CompileCode(hierarchy, itemId)) return;

            // --- Step #2: Check for zero code length
            if (Output.Segments.Sum(s => s.EmittedCode.Count) == 0)
            {
                VsxDialogs.Show("The length of the compiled code is 0, " +
                                "so there is no code to export.",
                    "No code to export.");
                Success = false;
                return;
            }

            // --- Step #2: Collect export parameters from the UI
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (DisplayExportParameterDialog(out var vm)) return;

            if (vm.Format == ExportFormat.IntelHex)
            {
                Package.CodeManager.SaveIntelHexFile(vm.Filename, Output);
                return;
            }

            // --- Step #3: Check screen file again
            var useScreenFile = !string.IsNullOrEmpty(vm.ScreenFile) && vm.ScreenFile.Trim().Length > 0;
            if (useScreenFile && !CommonTapeFilePlayer.CheckScreenFile(vm.ScreenFile))
            {
                VsxDialogs.Show("The specified screen file cannot be read as a ZX Spectrum compatible screen file.",
                    "Screen file error.", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
                Success = false;
                return;
            }

            // --- Step #4: Create code segments
            var codeBlocks = Package.CodeManager.CreateTapeBlocks(vm.Name, Output, vm.SingleBlock);
            List<byte[]> screenBlocks = null;
            if (useScreenFile)
            {
                screenBlocks = Package.CodeManager.CreatScreenBlocks(vm.ScreenFile);
            }

            // --- Step #5: Create Auto Start header block, if required
            var blocksToSave = new List<byte[]>();
            if (!ushort.TryParse(vm.StartAddress, out var startAddress))
            {
                startAddress = (ushort)ExportStartAddress;
            }
            var autoStartBlocks = Package.CodeManager.CreateAutoStartBlock(
                vm.Name, 
                useScreenFile, 
                vm.AddPause0, 
                vm.Border,
                codeBlocks.Count >> 1,
                startAddress,
                vm.ApplyClear
                    ? Output.Segments.Min(s => s.StartAddress)
                    : (ushort?) null);
            blocksToSave.AddRange(autoStartBlocks);

            // --- Step #6: Save all the blocks
            if (screenBlocks != null)
            {
                blocksToSave.AddRange(screenBlocks);
            }
            blocksToSave.AddRange(codeBlocks);
            SaveDataBlocks(vm, blocksToSave);

            if (!vm.AddToProject) return;

            // --- Step #6: Add the saved item to the project
            // --- Check path segment names
            DiscoveryProject.AddFileToProject(Package.Options.TapeFolder, vm.Filename,
                INVALID_FOLDER_MESSAGE, FILE_EXISTS_MESSAGE);
        }

        /// <summary>
        /// Override this method to define the action to execute on the main
        /// thread of Visual Studio -- finally
        /// </summary>
        protected override async Task FinallyOnMainThreadAsync()
        {
            await base.FinallyOnMainThreadAsync();
            if (Success && !IsCancelled && Package.Options.ConfirmCodeExport && Output.ErrorCount == 0)
            {
                VsxDialogs.Show("The code has been exported.");
            }
        }

        /// <summary>
        /// Displays the Export Z80 Code dialog to collect parameter data
        /// </summary>
        /// <param name="vm">View model with collected data</param>
        /// <returns>
        /// True, if the user stars export; false, if the export is cancelled
        /// </returns>
        private bool DisplayExportParameterDialog(out ExportZ80ProgramViewModel vm)
        {
            var exportDialog = new ExportZ80ProgramDialog
            {
                HasMaximizeButton = false,
                HasMinimizeButton = false
            };

            var programName = Path.GetFileNameWithoutExtension(CompiledItemPath) ?? "MyCode";
            var filename = Path.Combine(Package.Options.CodeExportPath, $"{programName}.tzx");
            vm = new ExportZ80ProgramViewModel
            {
                Format = ExportFormat.Tzx,
                Name = programName,
                Filename = filename,
                SingleBlock = true,
                AddToProject = false,
                AutoStart = true,
                ApplyClear = true,
                AddPause0 = false,
                StartAddress = ExportStartAddress.ToString()
            };
            exportDialog.SetVm(vm);
            var accepted = exportDialog.ShowModal();
            if (!accepted.HasValue || !accepted.Value)
            {
                IsCancelled = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Save data blocks
        /// </summary>
        /// <param name="vm">Export parameters</param>
        /// <param name="blocksToSave">Collection of data blocks to save</param>
        private static void SaveDataBlocks(ExportZ80ProgramViewModel vm, IEnumerable<byte[]> blocksToSave)
        {
            // --- Create directory
            var dirName = Path.GetDirectoryName(vm.Filename);
            if (dirName != null && !Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // --- Save data blocks
            if (vm.Format == ExportFormat.Tzx)
            {
                using (var writer = new BinaryWriter(File.Create(vm.Filename)))
                {
                    var header = new TzxHeader();
                    header.WriteTo(writer);

                    foreach (var block in blocksToSave)
                    {
                        var tzxBlock = new TzxStandardSpeedDataBlock
                        {
                            Data = block,
                            DataLength = (ushort) block.Length
                        };
                        tzxBlock.WriteTo(writer);
                    }
                }
            }
            else
            {
                using (var writer = new BinaryWriter(File.Create(vm.Filename)))
                {
                    foreach (var block in blocksToSave)
                    {
                        writer.Write((ushort) block.Length);
                        writer.Write(block);
                    }
                }
            }
        }
    }
}