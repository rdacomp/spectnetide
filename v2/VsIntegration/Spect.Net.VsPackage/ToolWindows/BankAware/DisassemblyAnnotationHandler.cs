using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Disassembler;
using Spect.Net.VsPackage.ToolWindows.Disassembly;
using Spect.Net.VsPackage.VsxLibrary;

namespace Spect.Net.VsPackage.ToolWindows.BankAware
{
    /// <summary>
    /// This class is responsible for managing disassembly annotations
    /// </summary>
    public class DisassemblyAnnotationHandler : IDisposable
    {
        /// <summary>
        /// The parent view model
        /// </summary>
        public BankAwareToolWindowViewModelBase Parent { get; }

        /// <summary>
        /// Annotations for ROM pages
        /// </summary>
        public Dictionary<int, DisassemblyAnnotation> RomPageAnnotations { get; private set; }

        /// <summary>
        /// ROM annotation files
        /// </summary>
        public Dictionary<int, string> RomAnnotationFiles { get; private set; }

        /// <summary>
        /// Annotations for RAM banks
        /// </summary>
        public Dictionary<int, DisassemblyAnnotation> RamBankAnnotations { get; private set; }

        /// <summary>
        /// The file to save RAM annotations to
        /// </summary>
        public string RamBankAnnotationFile { get; private set; }

        /// <summary>
        /// Initializes a new instance of the this class.
        /// </summary>
        /// <param name="parent">Parent view model</param>
        public DisassemblyAnnotationHandler(BankAwareToolWindowViewModelBase parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Sets up the annotations for the current machine.
        /// </summary>
        public void SetupMachineAnnotations()
        {
            // --- Read ROM annotations
            var spectrumVm = Parent.SpectrumVm;
            RomPageAnnotations = new Dictionary<int, DisassemblyAnnotation>();
            RomAnnotationFiles = new Dictionary<int, string>();
            var romConfig = spectrumVm.RomConfiguration;
            var roms = romConfig.NumberOfRoms;
            for (var i = 0; i < roms; i++)
            {
                var annFile = spectrumVm.RomProvider.GetAnnotationResourceName(romConfig.RomName,
                    roms == 1 ? -1 : i);
                var annData = spectrumVm.RomProvider.LoadRomAnnotations(romConfig.RomName,
                    roms == 1 ? -1 : i);

                DisassemblyAnnotation.Deserialize(annData, out var annotation);
                RomPageAnnotations.Add(i, annotation);
                RomAnnotationFiles.Add(i, annFile);
            }

            // --- Read the initial RAM annotations
            RamBankAnnotations = new Dictionary<int, DisassemblyAnnotation>();
            SpectNetPackage.Default.CodeManager.AnnotationFileChanged += OnAnnotationFileChanged;
            OnAnnotationFileChanged(null, EventArgs.Empty);

            // --- Register Disassembly providers to use
            if (RomPageAnnotations.TryGetValue(romConfig.Spectrum48RomIndex, out var spectrumRomAnn))
            {
                Z80Disassembler.SetProvider<ISpectrum48RomLabelProvider>(
                    new Spectrum48RomLabelProvider(spectrumRomAnn));
            }
        }

        /// <summary>
        /// Updates the RAM annotation file according to changes
        /// </summary>
        private void OnAnnotationFileChanged(object sender, EventArgs eventArgs)
        {
            var project = SpectNetPackage.Default.ActiveProject;
            var annFile = project?.DefaultAnnotationItem
                          ?? project?.AnnotationProjectItems?.FirstOrDefault();
            RamBankAnnotations.Clear();
            if (annFile == null) return;

            RamBankAnnotationFile = annFile.Filename;
            try
            {
                var disAnn = File.ReadAllText(annFile.Filename);
                DisassemblyAnnotation.DeserializeBankAnnotations(disAnn, out var annotations);
                RamBankAnnotations = annotations;
            } 
            catch (Exception ex)
            {
                VsxDialogs.Show(ex.Message, "Error loading annotation file", MessageBoxButton.OK, VsxMessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Stores the label in the annotations
        /// </summary>
        /// <param name="address">Label address</param>
        /// <param name="label">Label text</param>
        /// <param name="validationMessage">Validation message to display</param>
        public void SetLabel(ushort address, string label, out string validationMessage)
        {
            validationMessage = null;
            if (!string.IsNullOrWhiteSpace(label))
            {
                if (LabelDefinedInOtherBank(label))
                {
                    validationMessage = "Label name is duplicated";
                    return;
                }
            }
            var annotation = Parent.GetAnnotationFor(address, out var annAddr);
            var result = annotation.SetLabel(annAddr, label);
            if (result)
            {
                SaveAnnotations(annotation, address);
                return;
            }
            validationMessage = "Label name is invalid/duplicated";
        }

        /// <summary>
        /// Stores a comment in annotations
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        public void SetComment(ushort address, string comment)
        {
            var annotation = Parent.GetAnnotationFor(address, out var annAddr);
            annotation.SetComment(annAddr, comment);
            SaveAnnotations(annotation, address);
        }

        /// <summary>
        /// Stores a prefix name in this collection
        /// </summary>
        /// <param name="address">Comment address</param>
        /// <param name="comment">Comment text</param>
        public void SetPrefixComment(ushort address, string comment)
        {
            var annotation = Parent.GetAnnotationFor(address, out var annAddr);
            annotation.SetPrefixComment(annAddr, comment);
            SaveAnnotations(annotation, address);
        }

        /// <summary>
        /// Stores a section in this collection
        /// </summary>
        /// <param name="startAddress">Start address</param>
        /// <param name="endAddress">End address</param>
        /// <param name="type">Memory section type</param>
        public void AddSection(ushort startAddress, ushort endAddress, MemorySectionType type)
        {
            var startAnn = Parent.GetAnnotationFor(startAddress, out var start);
            var endAnn = Parent.GetAnnotationFor((ushort)(endAddress - 1), out var end);
            if (startAnn == endAnn)
            {
                // --- The section is within one bank
                var tempSection = new MemorySection(start, end, type);
                startAnn.MemoryMap.Add(tempSection);
                startAnn.MemoryMap.Normalize();
                SaveAnnotations(startAnn, startAddress);
            }
            else
            {
                // --- The section overlaps multiple banks
                // --- We must be in FullViewMode to get here
                var origSection = new MemorySection(startAddress, endAddress, type);
                for (var bank = 0; bank <= 3; bank++)
                {
                    var bankSection = new MemorySection((ushort)(bank * 0x4000), (ushort)(bank * 0x4000 + 0x3FFF));
                    if (origSection.Overlaps(bankSection))
                    {
                        // --- There is a memory section for this bank
                        var cutSection = origSection.Intersect(bankSection);
                        var bankAnn = Parent.GetAnnotationFor(cutSection.StartAddress, out var cutStart);
                        Parent.GetAnnotationFor(cutSection.EndAddress, out var cutEnd);
                        bankAnn.MemoryMap.Add(new MemorySection(cutStart, cutEnd, type));
                        bankAnn.MemoryMap.Normalize();
                        SaveAnnotations(bankAnn, startAddress);
                    }
                }
            }
        }

        /// <summary>
        /// Replaces a literal in the disassembly item for the specified address. If
        /// the named literal does not exists, creates one for the symbol.
        /// </summary>
        /// <param name="address">Disassembly item address</param>
        /// <param name="literalName">Literal name</param>
        /// <param name="lineIndexes">Indexes for disassembly lines</param>
        /// <param name="disassemblyItems">All disassembly items</param>
        /// <returns>Null, if operation id ok, otherwise, error message</returns>
        /// <remarks>If the literal already exists, it must have the symbol's value.</remarks>
        public string ApplyLiteral(ushort address, string literalName,
            Dictionary<ushort, int> lineIndexes, Collection<DisassemblyItemViewModel> disassemblyItems)
        {
            if (!lineIndexes.TryGetValue(address, out int lineIndex))
            {
                return $"No disassembly line is associated with address #{address:X4}";
            }

            var disassItem = disassemblyItems[lineIndex];
            if (!disassItem.Item.HasSymbol)
            {
                return $"Disassembly line #{address:X4} does not have an associated value to replace";
            }

            var symbolValue = disassItem.Item.SymbolValue;
            if (disassItem.Item.HasLabelSymbol)
            {
                return
                    $"%L {symbolValue:X4} {literalName}%Disassembly line #{address:X4} refers to a label. Use the 'L {symbolValue:X4}' command to define a label.";
            }

            var annotation = Parent.GetAnnotationFor(address, out _);
            var message = annotation.ApplyLiteral(address, symbolValue, literalName);
            if (message != null) return message;

            SaveAnnotations(annotation, address);
            return null;
        }

        /// <summary>
        /// Saves the annotation file for the specified address
        /// </summary>
        /// <param name="annotation">Annotation to save</param>
        /// <param name="address"></param>
        public void SaveAnnotations(DisassemblyAnnotation annotation, ushort address)
        {
            string filename;
            var isRom = false;
            var spectrumVm = SpectNetPackage.Default.EmulatorViewModel.Machine.SpectrumVm;
            if (Parent.FullViewMode)
            {
                var memDevice = spectrumVm.MemoryDevice;
                var (isInRom, index, _) = memDevice.GetAddressLocation(address);
                if (isInRom)
                {
                    filename = RomAnnotationFiles.TryGetValue(index, out var romFile)
                        ? romFile : null;
                    isRom = true;
                }
                else
                {
                    filename = RamBankAnnotationFile;
                }
            }
            else if (Parent.RomViewMode)
            {
                filename = RomAnnotationFiles.TryGetValue(Parent.RomIndex, out var romFile)
                    ? romFile : null;
                isRom = true;
            }
            else
            {
                filename = RamBankAnnotationFile;
            }
            if (filename == null) return;

            var annotationData = isRom ? annotation.Serialize() : SerializeRamBankAnnotations();
            File.WriteAllText(filename, annotationData);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            SpectNetPackage.Default.CodeManager.AnnotationFileChanged -= OnAnnotationFileChanged;
            Parent?.Dispose();
        }

        #region Helper methods

        /// <summary>
        /// Serializes RAM bank annotations
        /// </summary>
        /// <returns></returns>
        private string SerializeRamBankAnnotations()
        {
            var annData = RamBankAnnotations.ToDictionary(k => k.Key,
                v => v.Value.ToDisassemblyDecorationData());
            return JsonConvert.SerializeObject(annData, Formatting.Indented);
        }

        /// <summary>
        /// Checks if the specified label is already defined
        /// </summary>
        /// <param name="label">Label to check</param>
        /// <returns>True, if label is already defined; otherwise, false</returns>
        private bool LabelDefinedInOtherBank(string label)
        {
            var memoryDevice = SpectNetPackage.Default.EmulatorViewModel.Machine.SpectrumVm.MemoryDevice;
            if (Parent.RomViewMode)
            {
                // --- The label is allowed in another ROM, but not in the current one
                return false;
            }
            if (Parent.RamBankViewMode || Parent.FullViewMode)
            {
                var contains = RamBankAnnotations.Values.Any(ann => ann.Labels.Values.Any(
                    l => string.Compare(l, label, StringComparison.OrdinalIgnoreCase) == 0));
                if (contains) return true;
            }
            if (!Parent.FullViewMode) return false;
            return RomPageAnnotations.TryGetValue(memoryDevice.GetSelectedRomIndex(), out var romAnn) &&
                romAnn.Labels.Values.Any(l => string.Compare(l, label,
                    StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion
    }

}