using System.IO;
using Spect.Net.Assembler.Assembler;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.Z80Programs.Export
{
    /// <summary>
    /// This class represents the view model of the 
    /// Export Z80 Program command's UI
    /// </summary>
    public class ExportZ80ProgramViewModel: EnhancedViewModelBase
    {
        private ExportFormat _format;
        private string _name;
        private string _filename;
        private bool _autoStart;
        private bool _applyClear;
        private bool _singleBlock;
        private bool _addToProject;
        private string _startAddress;
        private string _startAddressHex;
        private string _screenFile;
        private bool _addPause0;
        private string _border;
        private bool _enableTapeFileArgs;

        public ExportZ80ProgramViewModel()
        {
            if (IsInDesignMode)
            {
                EnableTapeFileArgs = true;
            }
        }

        /// <summary>
        /// Gets or sets the tape format of the export
        /// </summary>
        public ExportFormat Format
        {
            get => _format;
            set
            {
                if (!Set(ref _format, value)) return;
                string ext;
                switch (_format)
                {
                    case ExportFormat.Tzx:
                        EnableTapeFileArgs = true;
                        ext = ".tzx";
                        break;
                    case ExportFormat.Tap:
                        EnableTapeFileArgs = true;
                        ext = ".tap";
                        break;
                    case ExportFormat.IntelHex:
                        EnableTapeFileArgs = false;
                        ext = ".hex";
                        break;
                    default:
                        EnableTapeFileArgs = false;
                        ext = ".txt";
                        break;
                }
                Filename = Path.ChangeExtension(Filename, ext);
            }
        }

        /// <summary>
        /// Indicates if tape file-related arguments are enabled
        /// </summary>
        public bool EnableTapeFileArgs
        {
            get => _enableTapeFileArgs;
            set
            {
                Set(ref _enableTapeFileArgs, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged("AutoStartEnabled");
            }
        }

        /// <summary>
        /// Gets or sets the name of the program to export
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                Set(ref _name, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        /// <summary>
        /// Gets or sets the name of the file (with full path) to save the code
        /// </summary>
        public string Filename
        {
            get => _filename;
            set
            {
                Set(ref _filename, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        /// <summary>
        /// Indicates if autostart block should be saved
        /// </summary>
        public bool AutoStart
        {
            get => _autoStart;
            set
            {
                Set(ref _autoStart, value);
                // ReSharper disable once ExplicitCallerInfoArgument
                RaisePropertyChanged("AutoStartEnabled");
            }
        }

        /// <summary>
        /// Indicates that the AutoStart option is enabled
        /// </summary>
        public bool AutoStartEnabled => AutoStart && EnableTapeFileArgs;

        /// <summary>
        /// Indicates if a clear commands should be applied in
        /// the auto start block
        /// </summary>
        public bool ApplyClear
        {
            get => _applyClear;
            set => Set(ref _applyClear, value);
        }

        /// <summary>
        /// Indicates if multiple segments should be saved into a single block
        /// </summary>
        public bool SingleBlock
        {
            get => _singleBlock;
            set => Set(ref _singleBlock, value);
        }

        /// <summary>
        /// Indicates if the saved project file should be added to the project
        /// </summary>
        public bool AddToProject
        {
            get => _addToProject;
            set => Set(ref _addToProject, value);
        }

        /**
         * Indicates if a "PAUSE 0" should be added before running the code
         */
        public bool AddPause0
        {
            get => _addPause0;
            set => Set(ref _addPause0, value);
        }

        /// <summary>
        /// Signs if the dialog content is valid
        /// </summary>
        public bool IsValid => 
            !string.IsNullOrWhiteSpace(Name) 
                && !string.IsNullOrWhiteSpace(Filename)
                && (Format == ExportFormat.Tzx 
                    || Format == ExportFormat.Tap 
                    || Format == ExportFormat.IntelHex);

        /// <summary>
        /// Start address of the code
        /// </summary>
        public string StartAddress
        {
            get => _startAddress;
            set
            {
                if (Set(ref _startAddress, value))
                {
                    StartAddressHex = int.TryParse(value, out var intVal) 
                        ? $"#{intVal:X4}" 
                        : "#????";
                }
            }
        }

        /// <summary>
        /// Start address of the code in hex format
        /// </summary>
        public string StartAddressHex
        {
            get => _startAddressHex;
            set => Set(ref _startAddressHex, value);
        }

        /// <summary>
        /// Optional screen file
        /// </summary>
        public string ScreenFile
        {
            get => _screenFile;
            set => Set(ref _screenFile, value);
        }

        /// <summary>
        /// Border color
        /// </summary>
        public string Border
        {
            get => _border;
            set => Set(ref _border, value);
        }

        /// <summary>
        /// The assembler output to save
        /// </summary>
        public AssemblerOutput AssemblerOutput { get; set; }
    }
}