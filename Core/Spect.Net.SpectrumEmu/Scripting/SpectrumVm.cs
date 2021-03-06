using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spect.Net.Assembler.Assembler;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Machine;
// ReSharper disable ArgumentsStyleLiteral

namespace Spect.Net.SpectrumEmu.Scripting
{
    /// <summary>
    /// This class represents a Spectrum virtual machine
    /// </summary>
    public sealed class SpectrumVm: IDisposable, ISpectrumVmController
    {
        private const ushort DEFAULT_CALL_STUB_ADDRESS = 0x5BA0;

        private readonly ISpectrumVm _spectrumVm;
        private readonly SpectrumVmStateFileManager _stateFileManager;
        private CancellationTokenSource _cancellationTokenSource;

        #region Machine properties

        /// <summary>
        /// The key of the Spectrum model
        /// </summary>
        public string ModelKey { get; }

        /// <summary>
        /// The edition key of the model
        /// </summary>
        public string EditionKey { get; }

        /// <summary>
        /// The CPU of the machine
        /// </summary>
        public CpuZ80 Cpu { get; }

        /// <summary>
        /// Provides access to the individual ROM pages of the machine
        /// </summary>
        public IReadOnlyList<ReadOnlyMemorySlice> Roms { get; }

        /// <summary>
        /// Gets the number of ROM pages
        /// </summary>
        public int RomCount => Roms.Count;
        
        /// <summary>
        /// Allows to obtain paging information about the memory
        /// </summary>
        public MemoryPagingInfo PagingInfo { get; }
        
        /// <summary>
        /// The current Contents of the machine's 64K addressable memory
        /// </summary>
        public SpectrumMemoryContents Memory { get; }

        /// <summary>
        /// Provides access to the individual RAM banks of the machine
        /// </summary>
        public IReadOnlyList<MemorySlice> RamBanks { get; }

        /// <summary>
        /// Gets the number of RAM banks
        /// </summary>
        public int RamBankCount => RamBanks.Count;

        /// <summary>
        /// Allows to emulate keyboard keys and query the keyboard state
        /// </summary>
        public KeyboardEmulator Keyboard { get; }

        /// <summary>
        /// Allows read-only access to screen rendering configuration
        /// </summary>
        public ScreenConfiguration ScreenConfiguration { get; }

        /// <summary>
        /// Allows read-only access to the screen rendering table
        /// </summary>
        public ScreenRenderingTable ScreenRenderingTable { get; }

        /// <summary>
        /// A bitmap that represents the current visible screen's pixels, including the border
        /// </summary>
        public ScreenBitmap ScreenBitmap { get; }

        /// <summary>
        /// Gets the current screen rendering status of the machine.
        /// </summary>
        public ScreenRenderingStatus ScreenRenderingStatus { get; }

        /// <summary>
        /// Gets the beeper configuration of the machine
        /// </summary>
        public IAudioConfiguration BeeperConfiguration { get; }

        /// <summary>
        /// Gets the beeper samples of the current rendering frame
        /// </summary>
        public AudioSamples BeeperSamples { get; }

        /// <summary>
        /// Gets the sound (PSG) configuration of the machine
        /// </summary>
        public IAudioConfiguration SoundConfiguration { get; }

        /// <summary>
        /// Gets the sound (PSG) samples of the current rendering frame
        /// </summary>
        public AudioSamples AudioSamples { get; }

        /// <summary>
        /// The collection of breakpoints
        /// </summary>
        public CodeBreakpoints Breakpoints { get; }

        /// <summary>
        /// Runs until the timeout value specified in milliseconds
        /// ellapses.
        /// </summary>
        /// <remarks>Set this value to zero to infinite timeout</remarks>
        public long TimeoutInMs
        {
            get => 1000 * TimeoutTacts / _spectrumVm.BaseClockFrequency / _spectrumVm.ClockMultiplier;
            set => TimeoutTacts = value * _spectrumVm.BaseClockFrequency * _spectrumVm.ClockMultiplier / 1000;
        }

        /// <summary>
        /// Runs until the timeout value specified in CPU tact values
        /// ellapses.
        /// </summary>
        /// <remarks>Set this value to zero to infinite timeout</remarks>
        public long TimeoutTacts { get; set; }

        /// <summary>
        /// Indicates if the machine runs in real time mode
        /// </summary>
        public bool RealTimeMode { get; set; }

        /// <summary>
        /// Indicates if the machine renders the screen
        /// </summary>
        public bool DisableScreenRendering { get; set; }

        /// <summary>
        /// Gets the reason that tells why the machine has been stopped or paused
        /// </summary>
        public ExecutionCompletionReason ExecutionCompletionReason { get; private set; }

        /// <summary>
        /// Gets the current state of the machine
        /// </summary>
        public VmState MachineState { get; private set; }

        /// <summary>
        /// Indicates if the Spectrum virtual machine runs in debug mode
        /// </summary>
        public bool RunsInDebugMode { get; private set; }

        /// <summary>
        /// The task that represents the completion of the execution cycle
        /// </summary>
        public Task CompletionTask { get; private set; }

        /// <summary>
        /// Gets or sets the folder that stores the cached .vmstate files
        /// </summary>
        public string CachedVmStateFolder { get; set; }

        #endregion

        #region Lifecycle methods

        /// <summary>
        /// Creates an instance of the virtual machine
        /// </summary>
        /// <param name="modelKey">The model key of the virtual machine</param>
        /// <param name="editionKey">The edition key of the virtual machine</param>
        /// <param name="devices">Devices to create the machine</param>
        internal SpectrumVm(string modelKey, string editionKey, DeviceInfoCollection devices)
        {
            ModelKey = modelKey;
            EditionKey = editionKey;
            RealTimeMode = false;
            DisableScreenRendering = false;
            CachedVmStateFolder = Directory.GetCurrentDirectory();

            _spectrumVm = new SpectrumEngine(devices);
            _stateFileManager = new SpectrumVmStateFileManager(modelKey, _spectrumVm, this,
                () => CachedVmStateFolder);

            Cpu = new CpuZ80(_spectrumVm.Cpu);

            var roms = new List<ReadOnlyMemorySlice>();
            for (var i = 0; i < _spectrumVm.RomConfiguration.NumberOfRoms; i++)
            {
                roms.Add(new ReadOnlyMemorySlice(_spectrumVm.RomDevice.GetRomBytes(i)));
            }
            Roms = new ReadOnlyCollection<ReadOnlyMemorySlice>(roms);

            PagingInfo = new MemoryPagingInfo(_spectrumVm.MemoryDevice);
            Memory = new SpectrumMemoryContents(_spectrumVm.MemoryDevice, _spectrumVm.Cpu);

            var ramBanks = new List<MemorySlice>();
            if (_spectrumVm.MemoryConfiguration.RamBanks != null)
            {
                for (var i = 0; i < _spectrumVm.MemoryConfiguration.RamBanks; i++)
                {
                    ramBanks.Add(new MemorySlice(_spectrumVm.MemoryDevice.GetRamBank(i)));
                }
            }
            RamBanks = new ReadOnlyCollection<MemorySlice>(ramBanks);

            Keyboard = new KeyboardEmulator(_spectrumVm.KeyboardDevice);
            ScreenConfiguration = _spectrumVm.ScreenConfiguration;
            ScreenRenderingTable = new ScreenRenderingTable(_spectrumVm.ScreenDevice);
            ScreenBitmap = new ScreenBitmap(_spectrumVm.ScreenDevice);
            ScreenRenderingStatus = new ScreenRenderingStatus(_spectrumVm);
            BeeperConfiguration = _spectrumVm.AudioConfiguration;
            BeeperSamples = new AudioSamples(_spectrumVm.BeeperDevice);
            SoundConfiguration = _spectrumVm.SoundConfiguration;
            AudioSamples = new AudioSamples(_spectrumVm.SoundDevice);
            Breakpoints = new CodeBreakpoints(_spectrumVm.DebugInfoProvider);

            MachineState = VmState.None;
            ExecutionCompletionReason = ExecutionCompletionReason.None;
            IsFirstStart = false;
            IsFirstPause = false;
            TimeoutTacts = 0;
            _spectrumVm.ScreenDevice.FrameCompleted += OnScreenFrameCompleted;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public async void Dispose()
        {
            if (MachineState != VmState.Stopped)
            {
                await Stop();
            }
            _spectrumVm.ScreenDevice.FrameCompleted -= OnScreenFrameCompleted;
        }

        /// <summary>
        /// Raises the VmFrameCompleted event
        /// </summary>
        private void OnScreenFrameCompleted(object sender, EventArgs eventArgs)
        {
            VmFrameCompleted?.Invoke(sender, eventArgs);
        }

        #endregion

        #region Machine control methods and properties

        /// <summary>
        /// Signs that this is the very first start of the
        /// virtual machine 
        /// </summary>
        public bool IsFirstStart { get; private set; }

        /// <summary>
        /// Signs that this is the very first paused state
        /// of the virtual machine
        /// </summary>
        public bool IsFirstPause { get; private set; }

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread.
        /// </summary>
        /// <remarks>The task completes when the machine has been started its execution cycle</remarks>
        public void Start() => Start(new ExecuteCycleOptions(
            timeoutTacts: TimeoutTacts,
            fastTapeMode: true,
            fastVmMode: !RealTimeMode,
            disableScreenRendering: DisableScreenRendering));

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread unless it reaches a breakpoint.
        /// </summary>
        public void StartDebug()
        {
            RunsInDebugMode = true;
            Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                timeoutTacts: TimeoutTacts,
                fastTapeMode: true,
                fastVmMode: !RealTimeMode,
                disableScreenRendering: DisableScreenRendering));
        }

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until it reaches a 
        /// HALT instruction.
        /// </summary>
        public void RunUntilHalt() => Start(new ExecuteCycleOptions(
            EmulationMode.UntilHalt,
            timeoutTacts: TimeoutTacts,
            fastTapeMode: true,
            fastVmMode: !RealTimeMode,
            disableScreenRendering: DisableScreenRendering));

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until the current
        /// frame is completed.
        /// </summary>
        public void RunUntilFrameCompletion() => Start(new ExecuteCycleOptions(
            EmulationMode.UntilFrameEnds,
            timeoutTacts: TimeoutTacts,
            fastTapeMode: true,
            fastVmMode: !RealTimeMode,
            disableScreenRendering: DisableScreenRendering));

        /// <summary>
        /// Starts the Spectrum machine and runs it on a background thread until the 
        /// CPU reaches the specified termination point.
        /// </summary>
        /// <param name="address">Termination address</param>
        /// <param name="romIndex">The index of the ROM, provided the address is in ROM</param>
        public void RunUntilTerminationPoint(ushort address, int romIndex = 0) => Start(
            new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                terminationRom: romIndex,
                terminationPoint: address,
                timeoutTacts: TimeoutTacts,
                fastTapeMode: true,
                fastVmMode: !RealTimeMode,
                disableScreenRendering: DisableScreenRendering));

        /// <summary>
        /// Sets the debug mode
        /// </summary>
        /// <param name="mode">True, if the machine should run in debug mode</param>
        void ISpectrumVmController.SetDebugMode(bool mode)
        {
            RunsInDebugMode = mode;
        }

        /// <summary>
        /// Pauses the Spectrum machine.
        /// </summary>
        /// <remarks>
        /// If the machine is paused or stopped, it leaves the machine in its state.
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public async Task Pause()
        {
            if (MachineState == VmState.None || MachineState == VmState.Stopped) return;

            // --- Prepare the machine to pause
            IsFirstPause = IsFirstStart;
            MoveToState(VmState.Pausing);

            // --- Wait for cancellation
            _cancellationTokenSource?.Cancel();
            await CompletionTask;

            // --- Now, it's been paused
            MoveToState(VmState.Paused);
        }

        /// <summary>
        /// Stops the Spectrum machine.
        /// </summary>
        /// <remarks>
        /// If the machine is paused or stopped, it leaves the machine in its state.
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public async Task Stop()
        {
            // --- Stop only running machine    
            switch (MachineState)
            {
                case VmState.Stopped:
                    return;

                case VmState.Paused:
                    // --- The machine is paused, it can be quicky stopped
                    MoveToState(VmState.Stopping);
                    MoveToState(VmState.Stopped);
                    break;

                default:
                    // --- Initiate stop
                    MoveToState(VmState.Stopping);
                    if (_cancellationTokenSource == null)
                    {
                        MoveToState(VmState.Stopped);
                    }
                    else
                    {
                        _cancellationTokenSource.Cancel();
                        await CompletionTask;
                        MoveToState(VmState.Stopped);
                    }
                    break;
            }
        }

        /// <summary>
        /// Executes the subsequent Z80 instruction.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public void StepInto()
        {
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepInto,
                timeoutTacts: TimeoutTacts,
                fastTapeMode: true,
                fastVmMode: !RealTimeMode,
                disableScreenRendering: DisableScreenRendering));
        }

        /// <summary>
        /// Executes the subsequent Z80 CALL, RST, or block instruction entirely.
        /// </summary>
        /// <remarks>
        /// The task completes when the machine has completed its execution cycle.
        /// </remarks>
        public void StepOver()
        {
            if (MachineState != VmState.Paused) return;

            RunsInDebugMode = true;
            Start(new ExecuteCycleOptions(EmulationMode.Debugger,
                DebugStepMode.StepOver,
                timeoutTacts: TimeoutTacts,
                fastTapeMode: true,
                fastVmMode: !RealTimeMode,
                disableScreenRendering: DisableScreenRendering));
        }

        /// <summary>
        /// Starts the virtual machine and pauses it when it reaches its main
        /// execution cycle.
        /// </summary>
        /// <param name="spectrum48Mode">Use Spectrum 48 mode?</param>
        /// <returns></returns>
        public async Task StartAndRunToMain(bool spectrum48Mode = false)
        {
            await _stateFileManager.SetProjectMachineStartupState(spectrum48Mode);
        }

        #endregion

        #region Machine state file function

        /// <summary>
        /// Saves the state of the Spectrum machine into the specified file
        /// </summary>
        /// <param name="filename">Machine state file name</param>
        public void SaveMachineStateTo(string filename)
        {
            _stateFileManager.SaveVmStateFile(filename);
        }

        /// <summary>
        /// Restores the machine state from the specified file
        /// </summary>
        /// <param name="filename">Machine state file name</param>
        public void RestoreMachineState(string filename)
        {
            _stateFileManager.LoadVmStateFile(filename);
        }

        #endregion

        #region Code manipulation function

        /// <summary>
        /// Injects the code into the RAM of the machine
        /// </summary>
        /// <param name="address">Start address of the code</param>
        /// <param name="codeArray">Code bytes</param>
        public void InjectCode(ushort address, byte[] codeArray)
        {
            if (MachineState != VmState.Paused)
            {
                throw new InvalidOperationException(
                    "The virtual machine must be in Paused state to allow code injection.");
            }
            if (_spectrumVm is ISpectrumVmRunCodeSupport runSupport)
            {
                // --- Go through all code segments and inject them
                runSupport.InjectCodeToMemory(address, codeArray);
                runSupport.PrepareRunMode();
            }
        }

        /// <summary>
        /// Compiles the provided source code and injects it into the virtual machine
        /// </summary>
        /// <param name="asmSource">Z80 assembly source code</param>
        /// <param name="options">Assembler options</param>
        /// <returns>The entry address of the code</returns>
        public ushort InjectCode(string asmSource, AssemblerOptions options = null)
        {
            if (MachineState != VmState.Paused)
            {
                throw new InvalidOperationException(
                    $"Machine should be in Paused state to allow code injection, not is is in {MachineState} state");
            }

            // --- Prepare assembler options
            if (options == null)
            {
                options = new AssemblerOptions
                {
                    CurrentModel = SpectrumModels.GetModelTypeFromName(ModelKey)
                };
            }

            // --- Compile the code
            var compiler = new Z80Assembler();
            var output = compiler.Compile(asmSource, options);
            if (output.ErrorCount > 0)
            {
                throw new InvalidOperationException(
                    $"Compilation failed with {output.ErrorCount} error(s). Code cannot be injected.");
            }

            // --- Check compatibility
            var modelType = output.ModelType ?? SpectrumModels.GetModelTypeFromName(ModelKey);
            if (!SpectrumModels.IsModelCompatibleWith(ModelKey, modelType))
            {
                throw new InvalidOperationException(
                    $"The model type defined in the code ({modelType}) is not compatible with the current" +
                    $"Spectum virtual machine ({ModelKey})");
            }

            // --- Check code length
            if (output.Segments.Sum(s => s.EmittedCode.Count) == 0)
            {
                throw new InvalidOperationException("The lenght of the compiled code is 0, " +
                                "so there is no code to inject into the virtual machine and run.");
            }

            // --- Do the code injection
            if (_spectrumVm is ISpectrumVmRunCodeSupport runSupport)
            {
                // --- Go through all code segments and inject them
                foreach (var segment in output.Segments)
                {
                    var addr = segment.StartAddress + (segment.Displacement ?? 0);
                    runSupport.InjectCodeToMemory((ushort)addr, segment.EmittedCode);
                }

                // --- Prepare the machine for RUN mode
                runSupport.PrepareRunMode();
            }
            return output.EntryAddress ?? output.Segments[0].StartAddress;
        }

        /// <summary>
        /// Calls the code at the specified subroutine start address
        /// </summary>
        /// <param name="startAddress">Subroutine start address</param>
        /// <param name="callStubAddress">Optional address for a call stub</param>
        /// <remarks>
        /// Generates a call stub and uses it to execute the specified subroutine.
        /// </remarks>
        public void CallCode(ushort startAddress, ushort? callStubAddress = null)
        {
            // --- Just for extra safety
            if (!(_spectrumVm is ISpectrumVmRunCodeSupport runSupport))
            {
                return;
            }

            // --- Set the call stub address
            if (callStubAddress == null)
            {
                callStubAddress = DEFAULT_CALL_STUB_ADDRESS;
            }

            // --- Create the call stub
            runSupport.InjectCodeToMemory(callStubAddress.Value, new byte[]
            {
                0xCD,
                (byte)startAddress,
                (byte)(startAddress >> 8)
            });
            var runOptions = new ExecuteCycleOptions(EmulationMode.UntilExecutionPoint,
                timeoutTacts: TimeoutTacts,
                terminationPoint: (ushort)(callStubAddress + 3),
                fastVmMode: true);

            // --- Jump to call stub
            Cpu.PC = callStubAddress.Value;
            Start(runOptions);
        }

        #endregion

        #region Events

        /// <summary>
        /// This event is raised whenever the state of the virtual machine changes
        /// </summary>
        public event EventHandler<VmStateChangedEventArgs> VmStateChanged;

        /// <summary>
        /// This event is raised when the engine stops because of an exception
        /// </summary>
        public event EventHandler<VmStoppedWithExceptionEventArgs> VmStoppedWithException;

        /// <summary>
        /// This event is raised when a screen rendering frame is completed
        /// </summary>
        public event EventHandler VmFrameCompleted;

        #endregion

        #region Implementation

        /// <summary>
        /// Starts the machine in a background thread.
        /// </summary>
        /// <param name="options">Options to start the machine with.</param>
        /// <remarks>
        /// Reports completion when the machine starts executing its cycles. The machine can
        /// go into Paused or Stopped state, if the execution options allow, for example, 
        /// when it runs to a predefined breakpoint.
        /// </remarks>
        public void Start(ExecuteCycleOptions options)
        {
            if (MachineState == VmState.Running) return;

            // --- Prepare the machine to run
            IsFirstStart = MachineState == VmState.None || MachineState == VmState.Stopped;
            if (IsFirstStart)
            {
                _spectrumVm.Reset();
            }
            _spectrumVm.DebugInfoProvider?.PrepareBreakpoints();

            // --- Dispose the previous cancellation token, and create a new one
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            // --- Set up the task that runs the machine
            CompletionTask = new Task(() =>
            {
                ExecutionCompletionReason = ExecutionCompletionReason.None;
                Exception cycleException = null;
                try
                {
                    _spectrumVm.ExecuteCycle(_cancellationTokenSource.Token, options);
                    ExecutionCompletionReason = _spectrumVm.ExecutionCompletionReason;
                }
                catch (TaskCanceledException)
                {
                    ExecutionCompletionReason = ExecutionCompletionReason.Cancelled;
                }
                catch (Exception ex)
                {
                    cycleException = ex;
                    ExecutionCompletionReason = ExecutionCompletionReason.Exception;
                }

                // --- Conclude the execution task
                MoveToState(MachineState == VmState.Stopping
                            || MachineState == VmState.Stopped
                            || cycleException != null
                    ? VmState.Stopped
                    : VmState.Paused);

                if (cycleException != null)
                {
                    VmStoppedWithException?.Invoke(this,
                        new VmStoppedWithExceptionEventArgs(cycleException));
                }
            });

            MoveToState(VmState.Running);
            CompletionTask.Start();
        }

        /// <summary>
        /// Forces the machine into Paused state
        /// </summary>
        public void ForcePausedState()
        {
            if (MachineState == VmState.Paused) return;
            if (MachineState == VmState.None || MachineState == VmState.Stopped)
            {
                IsFirstPause = true;
                MoveToState(VmState.Paused);
            }
        }

        /// <summary>
        /// Moves the virtual machine to the specified new state
        /// </summary>
        /// <param name="newState">New machine state</param>
        private void MoveToState(VmState newState)
        {
            var oldState = MachineState;
            MachineState = newState;
            VmStateChanged?.Invoke(this, new VmStateChangedEventArgs(oldState, newState));
        }

        #endregion
    }
}