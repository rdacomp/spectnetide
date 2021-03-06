using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Cpu;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;
using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Abstraction.TestSupport;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Devices.Interrupt;
using Spect.Net.SpectrumEmu.Devices.Keyboard;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Devices.Sound;
using Spect.Net.SpectrumEmu.Devices.Tape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Spect.Net.SpectrumEmu.Devices.Kempston;

// ReSharper disable IdentifierTypo

#pragma warning disable 67

// ReSharper disable ConvertToAutoProperty
// ReSharper disable VirtualMemberCallInConstructor

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class represents a ZX Spectrum 48 virtual machine
    /// </summary>
    public class SpectrumEngine : ISpectrumVm,
        ISpectrumVmTestSupport,
        ISpectrumVmRunCodeSupport
    {
        /// <summary>
        /// The length of a CPU Frame in tacts
        /// </summary>
        public const int CPU_FRAME = 4196;

        private readonly List<ISpectrumBoundDevice> _spectrumDevices = new List<ISpectrumBoundDevice>();
        private readonly List<IRenderFrameBoundDevice> _frameBoundDevices;
        private readonly List<ICpuOperationBoundDevice> _cpuBoundDevices;
        private ushort? _lastBreakpoint;

        /// <summary>
        /// The CPU tick at which the last frame rendering started;
        /// </summary>
        public long LastFrameStartCpuTick;

        /// <summary>
        /// The last rendered ULA tact 
        /// </summary>
        public int LastRenderedUlaTact;

        /// <summary>
        /// Gets the reason why the execution cycle of the SpectrumEngine completed.
        /// </summary>
        public ExecutionCompletionReason ExecutionCompletionReason { get; private set; }

        /// <summary>
        /// Collection of Spectrum devices
        /// </summary>
        public DeviceInfoCollection DeviceData { get; }

        /// <summary>
        /// The Z80 CPU of the machine
        /// </summary>
        public IZ80Cpu Cpu { get; }

        /// <summary>
        /// Gets the ULA revision (2/3)
        /// </summary>
        public string UlaIssue { get; }

        /// <summary>
        /// The CPU tact at which the last execution cycle started
        /// </summary>
        public long LastExecutionStartTact { get; private set; }

        /// <summary>
        /// Gets the value of the contention accumulated since the start of 
        /// the machine
        /// </summary>
        public long ContentionAccumulated { get; set; }

        /// <summary>
        /// The current execution cycle options
        /// </summary>
        public ExecuteCycleOptions ExecuteCycleOptions { get; private set; }

        /// <summary>
        /// The configuration of the ROM
        /// </summary>
        public IRomConfiguration RomConfiguration { get; }

        /// <summary>
        /// The ROM provider object
        /// </summary>
        public IRomProvider RomProvider { get; }

        /// <summary>
        /// The ROM device used by the virtual machine
        /// </summary>
        public IRomDevice RomDevice { get; }

        /// <summary>
        /// The configuration of the memory
        /// </summary>
        public IMemoryConfiguration MemoryConfiguration { get; }

        /// <summary>
        /// The memory device used by the virtual machine
        /// </summary>
        public IMemoryDevice MemoryDevice { get; }

        /// <summary>
        /// The port device used by the virtual machine
        /// </summary>
        public IPortDevice PortDevice { get; }

        /// <summary>
        /// The configuration of the screen
        /// </summary>
        public ScreenConfiguration ScreenConfiguration { get; }

        /// <summary>
        /// The ULA device that renders the VM screen
        /// </summary>
        public IScreenDevice ScreenDevice { get; }

        /// <summary>
        /// The ULA device for rendering the shadow WM screen
        /// </summary>
        public IScreenDevice ShadowScreenDevice { get; }

        /// <summary>
        /// The ULA device that takes care of raising interrupts
        /// </summary>
        public IInterruptDevice InterruptDevice { get; }

        /// <summary>
        /// The device responsible for handling the keyboard
        /// </summary>
        public IKeyboardDevice KeyboardDevice { get; }

        /// <summary>
        /// The provider that handles the keyboard
        /// </summary>
        public IKeyboardProvider KeyboardProvider { get; }

        /// <summary>
        /// The Kempston device attached to the VM
        /// </summary>
        public IKempstonDevice KempstonDevice { get; }

        /// <summary>
        /// The Kempston provider attached to the VM
        /// </summary>
        public IKempstonProvider KempstonProvider { get; private set; }

        /// <summary>
        /// The beeper device attached to the VM
        /// </summary>
        public IBeeperDevice BeeperDevice { get; }

        /// <summary>
        /// The provider that handles the beeper
        /// </summary>
        public IBeeperProvider BeeperProvider { get; }

        /// <summary>
        /// Beeper configuration
        /// </summary>
        public IAudioConfiguration AudioConfiguration { get; }

        /// <summary>
        /// The sound device attached to the VM
        /// </summary>
        public ISoundDevice SoundDevice { get; }

        /// <summary>
        /// The provider that handles the sound
        /// </summary>
        public ISoundProvider SoundProvider { get; }

        /// <summary>
        /// Sound configuration
        /// </summary>
        public IAudioConfiguration SoundConfiguration { get; }

        /// <summary>
        /// The tape device attached to the VM
        /// </summary>
        public ITapeLoadDevice TapeLoadDevice { get; }

        /// <summary>
        /// The tape save device attached to the VM.
        /// </summary>
        public ITapeSaveDevice TapeSaveDevice { get; }

        /// <summary>
        /// The tape load provider attached to the VM
        /// </summary>
        public ITapeLoadProvider TapeLoadProvider { get; }

        /// <summary>
        /// The tape save provider attached to the VM
        /// </summary>
        public ITapeSaveProvider TapeSaveProvider { get; }

        /// <summary>
        /// The configuration of the floppy
        /// </summary>
        public IFloppyConfiguration FloppyConfiguration { get; }

        /// <summary>
        /// The optional Floppy device
        /// </summary>
        public IFloppyDevice FloppyDevice { get; }

        /// <summary>
        /// Debug info provider object
        /// </summary>
        public ISpectrumDebugInfoProvider DebugInfoProvider { get; set; }

        /// <summary>
        /// Expression evaluation context used while debugging
        /// </summary>
        public IExpressionEvaluationContext DebugExpressionContext { get; set; }

        /// <summary>
        /// #of frames rendered
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// #of tacts within the frame
        /// </summary>
        public int FrameTacts { get; private set; }

        /// <summary>
        /// Gets the current frame tact according to the CPU tick count
        /// </summary>
        public virtual int CurrentFrameTact => (int)(Cpu.Tacts - LastFrameStartCpuTick) / ClockMultiplier;

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; set; }

        /// <summary>
        /// The number of frame tact at which the interrupt signal is generated
        /// </summary>
        public int InterruptTact => ScreenConfiguration.InterruptTact;

        /// <summary>
        /// Allows to set a clock frequency multiplier value (1, 2, 4, or 8).
        /// </summary>
        public int ClockMultiplier { get; }

        /// <summary>
        /// Initializes a class instance using a collection of devices
        /// </summary>
        public SpectrumEngine(DeviceInfoCollection deviceData, string ulaIssue = "3")
        {
            DeviceData = deviceData ?? throw new ArgumentNullException(nameof(deviceData));
            UlaIssue = ulaIssue == "3" ? "3" : "2";

            // --- Prepare the memory device
            var memoryInfo = GetDeviceInfo<IMemoryDevice>();
            MemoryDevice = memoryInfo?.Device ?? new Spectrum48MemoryDevice();
            MemoryConfiguration = (IMemoryConfiguration)memoryInfo?.ConfigurationData;

            // --- Prepare the port device
            var portInfo = GetDeviceInfo<IPortDevice>();
            PortDevice = portInfo?.Device ?? new Spectrum48PortDevice();

            // --- Init the CPU 
            var cpuConfig = GetDeviceConfiguration<IZ80Cpu, ICpuConfiguration>();
            var mult = 1;
            if (cpuConfig != null)
            {
                BaseClockFrequency = cpuConfig.BaseClockFrequency;
                mult = cpuConfig.ClockMultiplier;
                if (mult < 1) mult = 1;
                else if (mult >= 2 && mult <= 3) mult = 2;
                else if (mult >= 4 && mult <= 7) mult = 4;
                else if (mult > 8) mult = 8;
            }
            ClockMultiplier = mult;
            Cpu = new Z80Cpu(MemoryDevice,
                PortDevice,
                cpuConfig?.SupportsNextOperations ?? false)
            {
                UseGateArrayContention = MemoryConfiguration.ContentionType == MemoryContentionType.GateArray
            };

            // --- Init the ROM
            var romInfo = GetDeviceInfo<IRomDevice>();
            RomProvider = (IRomProvider)romInfo.Provider;
            RomDevice = romInfo.Device ?? new SpectrumRomDevice();
            RomConfiguration = (IRomConfiguration)romInfo.ConfigurationData;

            // --- Init the screen device
            var screenInfo = GetDeviceInfo<IScreenDevice>();
            var pixelRenderer = (IScreenFrameProvider)screenInfo.Provider;
            ScreenConfiguration = new ScreenConfiguration((IScreenConfiguration)screenInfo.ConfigurationData);
            ScreenDevice = screenInfo.Device ?? new Spectrum48ScreenDevice();
            ShadowScreenDevice = new Spectrum48ScreenDevice();

            // --- Init the beeper device
            var beeperInfo = GetDeviceInfo<IBeeperDevice>();
            AudioConfiguration = (IAudioConfiguration)beeperInfo?.ConfigurationData;
            BeeperProvider = (IBeeperProvider)beeperInfo?.Provider;
            BeeperDevice = beeperInfo?.Device ?? new BeeperDevice();

            // --- Init the keyboard device
            var keyboardInfo = GetDeviceInfo<IKeyboardDevice>();
            KeyboardProvider = (IKeyboardProvider)keyboardInfo?.Provider;
            KeyboardDevice = keyboardInfo?.Device ?? new KeyboardDevice();

            // --- Init the Kempston device
            var kempstonInfo = GetDeviceInfo<IKempstonDevice>();
            KempstonProvider = (IKempstonProvider)kempstonInfo?.Provider;
            KempstonDevice = kempstonInfo?.Device ?? new KempstonDevice();
            
            // --- Init the interrupt device
            InterruptDevice = new InterruptDevice(InterruptTact);

            // --- Init the tape device
            var tapeSaveInfo = GetDeviceInfo<ITapeSaveDevice>();
            TapeSaveProvider = (ITapeSaveProvider)tapeSaveInfo?.Provider;
            var tapeLoadInfo = GetDeviceInfo<ITapeLoadDevice>();
            TapeLoadProvider = (ITapeLoadProvider)tapeLoadInfo?.Provider;
            var tapeDevice = new TapeDevice(TapeLoadProvider, TapeSaveProvider);
            TapeLoadDevice = tapeDevice;
            TapeSaveDevice = tapeDevice;

            // === Init optional devices
            // --- Init the sound device
            var soundInfo = GetDeviceInfo<ISoundDevice>();
            SoundConfiguration = (IAudioConfiguration)soundInfo?.ConfigurationData;
            SoundProvider = (ISoundProvider)soundInfo?.Provider;
            SoundDevice = soundInfo == null
                ? null
                : soundInfo.Device ?? new SoundDevice();

            // --- Init the floppy device
            var floppyInfo = GetDeviceInfo<IFloppyDevice>();
            if (floppyInfo != null)
            {
                FloppyDevice = floppyInfo.Device;
                FloppyConfiguration = (IFloppyConfiguration)floppyInfo.ConfigurationData ?? new FloppyConfiguration();
            }

            // --- Carry out frame calculations
            ResetUlaTact();
            FrameTacts = ScreenConfiguration.ScreenRenderingFrameTactCount;
            FrameCount = 0;
            Overflow = 0;
            HasFrameCompleted = true;
            _lastBreakpoint = null;

            // --- Attach providers
            AttachProvider(RomProvider);
            AttachProvider(pixelRenderer);
            AttachProvider(BeeperProvider);
            AttachProvider(KeyboardProvider);
            AttachProvider(KempstonProvider);
            AttachProvider(TapeLoadProvider);
            AttachProvider(DebugInfoProvider);

            // --- Attach optional providers
            if (SoundProvider != null)
            {
                AttachProvider(SoundProvider);
            }

            // --- Collect Spectrum devices
            _spectrumDevices.Add(RomDevice);
            _spectrumDevices.Add(MemoryDevice);
            _spectrumDevices.Add(PortDevice);
            _spectrumDevices.Add(ScreenDevice);
            _spectrumDevices.Add(ShadowScreenDevice);
            _spectrumDevices.Add(BeeperDevice);
            _spectrumDevices.Add(KeyboardDevice);
            _spectrumDevices.Add(KempstonDevice);
            _spectrumDevices.Add(InterruptDevice);
            _spectrumDevices.Add(TapeLoadDevice);

            // --- Collect optional devices
            if (SoundDevice != null) _spectrumDevices.Add(SoundDevice);
            if (FloppyDevice != null) _spectrumDevices.Add(FloppyDevice);

            // --- Now, prepare devices to find each other
            foreach (var device in _spectrumDevices)
            {
                device.OnAttachedToVm(this);
            }

            // --- Prepare bound devices
            _frameBoundDevices = _spectrumDevices
                .OfType<IRenderFrameBoundDevice>()
                .ToList();
            _cpuBoundDevices = _spectrumDevices
                .OfType<ICpuOperationBoundDevice>()
                .ToList();

            DebugInfoProvider = new SpectrumDebugInfoProvider();

            // --- Init the ROM
            InitRom(RomDevice, RomConfiguration);
        }

        /// <summary>
        /// Attache the specified provider to this VM
        /// </summary>
        private void AttachProvider(IVmComponentProvider provider)
        {
            provider?.OnAttachedToVm(this);
        }

        /// <summary>
        /// Gets the device with the provided type
        /// </summary>
        /// <typeparam name="TDevice"></typeparam>
        /// <returns></returns>
        public IDeviceInfo<TDevice, IDeviceConfiguration, IVmComponentProvider> GetDeviceInfo<TDevice>()
            where TDevice : class, IDevice
        {
            return DeviceData.TryGetValue(typeof(TDevice), out var deviceInfo)
                ? (IDeviceInfo<TDevice, IDeviceConfiguration, IVmComponentProvider>)deviceInfo
                : null;
        }

        /// <summary>
        /// Gets the device with the provided type
        /// </summary>
        /// <typeparam name="TDevice">Device type</typeparam>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <returns></returns>
        public TConfig GetDeviceConfiguration<TDevice, TConfig>()
            where TConfig : class, IDeviceConfiguration
        {
            return DeviceData.TryGetValue(typeof(TDevice), out var deviceInfo)
                ? (TConfig)deviceInfo.ConfigurationData
                : null;
        }

        /// <summary>
        /// Resets the CPU and the ULA chip
        /// </summary>
        public void Reset()
        {
            Cpu.SetResetSignal();
            ResetUlaTact();
            FrameCount = 0;
            Overflow = 0;
            LastFrameStartCpuTick = 0;
            LastExecutionStartTact = 0L;
            ContentionAccumulated = 0L;
            HasFrameCompleted = true;
            Cpu.Reset();
            Cpu.ReleaseResetSignal();
            foreach (var device in _spectrumDevices)
            {
                device.Reset();
            }
            if (DebugInfoProvider != null)
            {
                DebugInfoProvider.ImminentBreakpoint = null;
            }
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        public IDeviceState GetState() => new Spectrum48DeviceState(this);

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public void RestoreState(IDeviceState state) => state.RestoreDeviceState(this);

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            foreach (var device in _frameBoundDevices)
            {
                device.OnNewFrame();
            }
        }

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
            foreach (var device in _frameBoundDevices)
            {
                device.Overflow = Overflow;
                device.OnFrameCompleted();
            }
        }

        public event EventHandler FrameCompleted;

        /// <summary>
        /// Resets the ULA tact to start screen rendering from the beginning
        /// </summary>
        public void ResetUlaTact()
        {
            LastRenderedUlaTact = -1;
        }

        /// <summary>
        /// Sets the debug info provider to the specified object
        /// </summary>
        /// <param name="provider">Provider object</param>
        public void SetDebugInfoProvider(ISpectrumDebugInfoProvider provider)
        {
            DebugInfoProvider = provider;
        }

        /// <summary>
        /// The main execution cycle of the Spectrum VM
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <param name="options">Execution options</param>
        /// <param name="completeOnCpuFrame">The cycle should complete on CPU frame completion</param>
        /// <return>True, if the cycle completed; false, if it has been cancelled</return>
        public bool ExecuteCycle(CancellationToken token, ExecuteCycleOptions options, bool completeOnCpuFrame = false)
        {
            ExecuteCycleOptions = options;
            ExecutionCompletionReason = ExecutionCompletionReason.None;
            LastExecutionStartTact = Cpu.Tacts;

            // --- We use this variables to check whether to stop in Debug mode
            var executedInstructionCount = -1;
            var entryStepOutDepth = Cpu.StackDebugSupport.StepOutStackDepth;

            // --- Check if we're just start running the next frame
            if (HasFrameCompleted)
            {
                // --- This counter helps us to calculate where we are in the frame after
                // --- each CPU operation cycle
                LastFrameStartCpuTick = Cpu.Tacts - Overflow;

                // --- Notify devices to start a new frame
                OnNewFrame();
                LastRenderedUlaTact = Overflow;
                HasFrameCompleted = false;
            }

            // --- The physical frame cycle that goes on while CPU and ULA 
            // --- processes everything within a physical frame (0.019968 second)
            while (!HasFrameCompleted)
            {
                // --- Check debug mode when a CPU instruction has been entirely executed
                if (!Cpu.IsInOpExecution)
                {
                    // --- The next instruction is about to be executed
                    executedInstructionCount++;

                    // --- Check for cancellation
                    if (token.IsCancellationRequested)
                    {
                        ExecutionCompletionReason = ExecutionCompletionReason.Cancelled;
                        return false;
                    }

                    // --- Check for CPU frame completion
                    if (completeOnCpuFrame && Cpu.Tacts > LastExecutionStartTact + CPU_FRAME)
                    {
                        ExecutionCompletionReason = ExecutionCompletionReason.CpuFrameCompleted;
                        return true;
                    }

                    // --- Check for several termination modes
                    switch (options.EmulationMode)
                    {
                        // --- Check for reaching the termination point
                        case EmulationMode.UntilExecutionPoint when options.TerminationPoint < 0x4000:
                            {
                                // --- ROM & address must match
                                if (options.TerminationRom == MemoryDevice.GetSelectedRomIndex()
                                    && options.TerminationPoint == Cpu.Registers.PC)
                                {
                                    // --- We reached the termination point within ROM
                                    ExecutionCompletionReason = ExecutionCompletionReason.TerminationPointReached;
                                    return true;
                                }

                                break;
                            }

                        // --- Check if we reached the termination point within RAM
                        case EmulationMode.UntilExecutionPoint
                            when options.TerminationPoint == Cpu.Registers.PC:

                            ExecutionCompletionReason = ExecutionCompletionReason.TerminationPointReached;
                            return true;

                        // --- Check for a debugging stop point
                        case EmulationMode.Debugger
                            when IsDebugStop(options, executedInstructionCount, entryStepOutDepth)
                                 && DebugConditionSatisfied():

                            // --- At this point, the cycle should be stopped because of debugging reasons
                            // --- The screen should be refreshed
                            ExecutionCompletionReason = ExecutionCompletionReason.BreakpointReached;
                            return true;
                    }
                }

                // --- Check for interrupt signal generation
                InterruptDevice.CheckForInterrupt(CurrentFrameTact);

                // --- Run a single Z80 instruction
                Cpu.ExecuteCpuCycle();
                _lastBreakpoint = null;

                // --- Run a rendering cycle according to the current CPU tact count
                var lastTact = CurrentFrameTact;
                ScreenDevice.RenderScreen(LastRenderedUlaTact + 1, lastTact);
                LastRenderedUlaTact = lastTact;

                // --- Exit if the CPU is HALTed and the emulation mode specifies so
                if (options.EmulationMode == EmulationMode.UntilHalt
                    && (Cpu.StateFlags & Z80StateFlags.Halted) != 0)
                {
                    ExecutionCompletionReason = ExecutionCompletionReason.Halted;
                    return true;
                }

                // --- Notify each CPU-bound device that the current operation has been completed
                foreach (var device in _cpuBoundDevices)
                {
                    device.OnCpuOperationCompleted();
                }

                // --- Decide whether this frame has been completed
                HasFrameCompleted = !Cpu.IsInOpExecution && CurrentFrameTact >= FrameTacts;
            }

            // --- A physical frame has just been completed. Take care about screen refresh
            FrameCount++;
            Overflow = CurrentFrameTact % FrameTacts;

            // --- Notify devices that the current frame completed
            OnFrameCompleted();

            // --- We exit the cycle when the render frame has completed
            ExecutionCompletionReason = ExecutionCompletionReason.RenderFrameCompleted;
            return true;
        }

        /// <summary>
        /// Checks whether the execution cycle should be stopped for debugging
        /// </summary>
        /// <param name="options">Execution options</param>
        /// <param name="executedInstructionCount">
        /// The count of instructions already executed in this cycle
        /// </param>
        /// <param name="entryStepOutStackDepth">The initial depth of the Step-Out stack</param>
        /// <returns>True, if the execution should be stopped</returns>
        private bool IsDebugStop(ExecuteCycleOptions options, int executedInstructionCount, int entryStepOutStackDepth)
        {
            // --- No debug provider, no stop
            if (DebugInfoProvider == null)
            {
                return false;
            }

            switch (options.DebugStepMode)
            {
                // --- In Step-Into mode we always stop when we're about to
                // --- execute the next instruction
                case DebugStepMode.StepInto:
                    return executedInstructionCount > 0;

                // --- In Stop-At-Breakpoint mode we stop only if a predefined
                // --- breakpoint is reached
                case DebugStepMode.StopAtBreakpoint
                    when DebugInfoProvider.ShouldBreakAtAddress(Cpu.Registers.PC) && (executedInstructionCount > 0
                        || _lastBreakpoint == null
                        || _lastBreakpoint != Cpu.Registers.PC):
                    // --- If we are paused at a breakpoint, we do not want
                    // --- to pause again and again, unless we step through
                    _lastBreakpoint = Cpu.Registers.PC;
                    return true;

                // --- We're in Step-Over mode
                case DebugStepMode.StepOver
                    when DebugInfoProvider.ImminentBreakpoint != null:
                    {
                        // --- We also stop, if an imminent breakpoint is reached, and also remove
                        // --- this breakpoint
                        if (DebugInfoProvider.ImminentBreakpoint == Cpu.Registers.PC)
                        {
                            DebugInfoProvider.ImminentBreakpoint = null;
                            return true;
                        }
                        break;
                    }

                case DebugStepMode.StepOver:
                    {
                        var imminentJustCreated = false;

                        // --- We check for a CALL-like instruction
                        var length = Cpu.GetCallInstructionLength();
                        if (length > 0)
                        {
                            // --- Its a CALL-like instruction, create an imminent breakpoint
                            DebugInfoProvider.ImminentBreakpoint = (ushort)(Cpu.Registers.PC + length);
                            imminentJustCreated = true;
                        }

                        // --- We stop, we executed at least one instruction and if there's no imminent 
                        // --- breakpoint or we've just created one
                        if (executedInstructionCount > 0
                            && (DebugInfoProvider.ImminentBreakpoint == null || imminentJustCreated))
                        {
                            return true;
                        }
                        break;
                    }

                // --- We're in Step-Out mode and want to complete the current subroutine call
                case DebugStepMode.StepOut
                    when Cpu.StackDebugSupport.RetExecuted
                        && executedInstructionCount > 0
                        && entryStepOutStackDepth == Cpu.StackDebugSupport.StepOutStackDepth + 1:
                    {
                        if (Cpu.Registers.PC != Cpu.StackDebugSupport.StepOutAddress)
                        {
                            Cpu.StackDebugSupport.ClearStepOutStack();
                        }
                        return true;
                    }
            }

            // --- In any other case, we carry on
            return false;
        }

        /// <summary>
        /// Checks if the debug condition for the address pointed by PC is satisfied
        /// </summary>
        /// <returns>
        /// True, if condition has been satisfied; otherwise, false.
        /// </returns>
        private bool DebugConditionSatisfied()
        {
            if (ExecuteCycleOptions.DebugStepMode != DebugStepMode.StopAtBreakpoint)
            {
                // --- We always stop at imminent breakpoints
                return true;
            }
            if (!DebugInfoProvider.Breakpoints.TryGetValue(Cpu.Registers.PC, out var breakpoint))
            {
                // --- No registered breakpoint, no stop
                return false;
            }
            if (breakpoint.HitType != BreakpointHitType.None)
            {
                // --- Check if hit condition is satisfied
                breakpoint.CurrentHitCount++;
                switch (breakpoint.HitType)
                {
                    case BreakpointHitType.Less:
                        if (breakpoint.CurrentHitCount >= breakpoint.HitConditionValue)
                        {
                            return false;
                        }
                        break;
                    case BreakpointHitType.LessOrEqual:
                        if (breakpoint.CurrentHitCount > breakpoint.HitConditionValue)
                        {
                            return false;
                        }
                        break;
                    case BreakpointHitType.Equal:
                        if (breakpoint.CurrentHitCount != breakpoint.HitConditionValue)
                        {
                            return false;
                        }
                        break;
                    case BreakpointHitType.Greater:
                        if (breakpoint.CurrentHitCount <= breakpoint.HitConditionValue)
                        {
                            return false;
                        }
                        break;
                    case BreakpointHitType.GreaterOrEqual:
                        if (breakpoint.CurrentHitCount < breakpoint.HitConditionValue)
                        {
                            return false;
                        }
                        break;
                    case BreakpointHitType.Multiple:
                        if (breakpoint.CurrentHitCount % breakpoint.HitConditionValue != 0)
                        {
                            return false;
                        }
                        break;
                }
            }

            if (DebugExpressionContext != null && breakpoint.FilterExpression != null)
            {
                // --- Check if filter condition is satisfied
                try
                {
                    var value = breakpoint.FilterExpression.Evaluate(DebugExpressionContext);
                    return value == ExpressionValue.Error || value.Value != 0;
                }
                catch
                {
                    // --- In case of evaluation error, we stop at the breakpoint
                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// This flag tells if the frame has just been completed.
        /// </summary>
        public bool HasFrameCompleted { get; private set; }

        /// <summary>
        /// Writes a byte to the memory
        /// </summary>
        /// <param name="addr">Memory address</param>
        /// <param name="value">Data byte</param>
        public void WriteSpectrumMemory(ushort addr, byte value) =>
            MemoryDevice.Write(addr, value);

        /// <summary>
        /// Sets the ULA frame tact for testing purposes
        /// </summary>
        /// <param name="tacts">ULA frame tact to set</param>
        public void SetUlaFrameTact(int tacts)
        {
            LastRenderedUlaTact = tacts;
            var cpuTest = Cpu as IZ80CpuTestSupport;
            cpuTest?.SetTacts(tacts);
            HasFrameCompleted = tacts == 0;
        }

        /// <summary>
        /// Gets the frequency of the virtual machine's clock in Hz
        /// </summary>
        public int BaseClockFrequency { get; } = 3_500_000;

        #region ISpectrumVmRunCodeSupport

        /// <summary>
        /// Injects code into the memory
        /// </summary>
        /// <param name="addr">Start address</param>
        /// <param name="code">Code to inject</param>
        /// <remarks>The code leaves the ROM area untouched.</remarks>
        public void InjectCodeToMemory(ushort addr, IReadOnlyCollection<byte> code)
        {
            // --- Now, inject the code
            foreach (var codeByte in code)
            {
                MemoryDevice.Write(addr++, codeByte);
            }
        }

        /// <summary>
        /// Injects code to the specified bank, starting at address 0
        /// </summary>
        /// <param name="bankIndex">Index of the bank</param>
        /// <param name="bankOffset">Offset within the bank</param>
        /// <param name="code">Code to insert</param>
        public void InjectCodeToBank(int bankIndex, int bankOffset, IReadOnlyCollection<byte> code)
        {
            if (!(MemoryDevice is BankedMemoryDeviceBase bankedDevice) 
                || bankIndex < 0 
                || bankIndex > bankedDevice.RamBanks.Length) return;

            // --- Now, inject the code
            var addr = bankOffset;
            foreach (var codeByte in code)
            {
                bankedDevice.RamBanks[bankIndex][addr++] = codeByte;
            }
        }

        /// <summary>
        /// Prepares the custom code for running, as if it were started
        /// with the RUN command
        /// </summary>
        public void PrepareRunMode(HashSet<string> options)
        {
            if (!options.Contains("cursork"))
            {
                // --- Set the keyboard in "L" mode
                var flags = MemoryDevice.Read(0x5C3B);
                flags |= 0x08;
                MemoryDevice.Write(0x5C3B, flags);
            }
        }

        #endregion

        #region Helper functions

        /// <summary>
        /// Loads the content of the ROM through the specified provider
        /// </summary>
        /// <param name="romDevice">ROM device instance</param>
        /// <param name="romConfig">ROM configuration</param>
        /// <remarks>
        /// The content of the ROM is copied into the memory
        /// </remarks>
        public void InitRom(IRomDevice romDevice, IRomConfiguration romConfig)
        {
            for (var i = 0; i < romConfig.NumberOfRoms; i++)
            {
                MemoryDevice.SelectRom(i);
                MemoryDevice.CopyRom(romDevice.GetRomBytes(i));
            }
            MemoryDevice.SelectRom(0);
        }

        #endregion

        #region VM State

        /// <summary>
        /// Gets the virtual machine's state serialized to JSON
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public string GetVmState(string modelName)
        {
            return JsonConvert.SerializeObject(new Spectrum48DeviceState(this, modelName), Formatting.Indented);
        }

        /// <summary>
        /// Sets the virtual machine's state from the JSON string
        /// </summary>
        /// <param name="json">JSON representation of the VM's state</param>
        /// <param name="modelName">Current virtual machine model name</param>
        public void SetVmState(string json, string modelName)
        {
            var state = JObject.Parse(json);
            var storedModelName = state[nameof(Spectrum48DeviceState.ModelName)].ToString();
            if (storedModelName != modelName)
            {
                throw new InvalidVmStateException(
                $"The stored model ({storedModelName}) is not compatible with the current virtual machine model ({modelName})");
            }

            // --- Read main device elements
            // ReSharper disable once UseObjectOrCollectionInitializer
            var spState = new Spectrum48DeviceState();
            spState.LastFrameStartCpuTick = state[nameof(Spectrum48DeviceState.LastFrameStartCpuTick)].Value<long>();
            spState.LastRenderedUlaTact = state[nameof(Spectrum48DeviceState.LastRenderedUlaTact)].Value<int>();
            spState.FrameCount = state[nameof(Spectrum48DeviceState.FrameCount)].Value<int>();
            spState.FrameTacts = state[nameof(Spectrum48DeviceState.FrameTacts)].Value<int>();
            spState.Overflow = state[nameof(Spectrum48DeviceState.Overflow)].Value<int>();
            spState.RunsInMaskableInterrupt = state[nameof(Spectrum48DeviceState.RunsInMaskableInterrupt)].Value<bool>();
            spState.Z80CpuState = GetDeviceState(state, nameof(Spectrum48DeviceState.Z80CpuState), "CPU");
            spState.RomDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.RomDeviceState), "ROM");
            spState.MemoryDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.MemoryDeviceState),
                "memory device");
            spState.PortDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.PortDeviceState),
                "port device");
            spState.ScreenDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.ScreenDeviceState),
                "screen device");
            spState.InterruptDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.InterruptDeviceState),
                "interrupt device");
            spState.KeyboardDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.KeyboardDeviceState),
                "keyboard device");
            spState.BeeperDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.BeeperDeviceState),
                "beeper device");
            spState.SoundDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.SoundDeviceState),
                "sound device");
            spState.TapeDeviceState = GetDeviceState(state, nameof(Spectrum48DeviceState.TapeDeviceState),
                "tape device");

            // --- Store back device state
            spState.RestoreDeviceState(this);
        }

        /// <summary>
        /// Gets the device state from the deserialized JSON state
        /// </summary>
        /// <param name="fullState">Deserialized JSON state</param>
        /// <param name="name">Field that names the type that stores device state information</param>
        /// <param name="label">Device label in error messages</param>
        /// <returns></returns>
        private static IDeviceState GetDeviceState(JObject fullState, string name, string label)
        {
            var deviceTypeName = fullState[$"{name}Type"].Value<string>();
            if (deviceTypeName == null) return null;

            var deviceType = Type.GetType(deviceTypeName);
            if (deviceType == null)
            {
                throw new InvalidVmStateException(
                    $"Cannot find type '{deviceTypeName}' to deserialize {label} state information");
            }
            var stateString = fullState[name].ToString();
            return (IDeviceState)JsonConvert.DeserializeObject(stateString, deviceType);
        }

        /// <summary>
        /// Clears the screep
        /// </summary>
        public void ClearScreen()
        {
            // --- Clear the screen
            for (var i = 0; i < 0x1800; i++)
            {
                MemoryDevice.Write((ushort)(0x4000 + i), 0x00);
            }
            for (var i = 0; i < 0x300; i++)
            {
                MemoryDevice.Write((ushort)(0x5800 + i), 0x38);
            }
        }

        /// <summary>
        /// Describes the entire state of the Spectrum virtual machine
        /// </summary>
        public class Spectrum48DeviceState : IDeviceState
        {
            public string ModelName { get; set; }
            public long LastFrameStartCpuTick { get; set; }
            public int LastRenderedUlaTact { get; set; }
            public int FrameCount { get; set; }
            public int FrameTacts { get; set; }
            public int Overflow { get; set; }
            public bool RunsInMaskableInterrupt { get; set; }

            public IDeviceState Z80CpuState { get; set; }
            public string Z80CpuStateType { get; set; }
            public IDeviceState RomDeviceState { get; set; }
            public string RomDeviceStateType { get; set; }
            public IDeviceState MemoryDeviceState { get; set; }
            public string MemoryDeviceStateType { get; set; }
            public IDeviceState PortDeviceState { get; set; }
            public string PortDeviceStateType { get; set; }
            public IDeviceState ScreenDeviceState { get; set; }
            public string ScreenDeviceStateType { get; set; }
            public IDeviceState InterruptDeviceState { get; set; }
            public string InterruptDeviceStateType { get; set; }
            public IDeviceState KeyboardDeviceState { get; set; }
            public string KeyboardDeviceStateType { get; set; }
            public IDeviceState BeeperDeviceState { get; set; }
            public string BeeperDeviceStateType { get; set; }
            public IDeviceState SoundDeviceState { get; set; }
            public string SoundDeviceStateType { get; set; }
            public IDeviceState TapeDeviceState { get; set; }
            public string TapeDeviceStateType { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object" /> class.
            /// </summary>
            public Spectrum48DeviceState()
            {
            }

            /// <summary>
            /// Initializes the state from the specified instance
            /// </summary>
            public Spectrum48DeviceState(SpectrumEngine spectrum, string modelName = null)
            {
                if (spectrum == null) return;

                ModelName = modelName;
                LastFrameStartCpuTick = spectrum.LastFrameStartCpuTick;
                LastRenderedUlaTact = spectrum.LastRenderedUlaTact;
                FrameCount = spectrum.FrameCount;
                FrameTacts = spectrum.FrameTacts;
                Overflow = spectrum.Overflow;

                Z80CpuState = spectrum.Cpu?.GetState();
                Z80CpuStateType = Z80CpuState?.GetType().AssemblyQualifiedName;
                RomDeviceState = spectrum.RomDevice?.GetState();
                RomDeviceStateType = RomDeviceState?.GetType().AssemblyQualifiedName;
                MemoryDeviceState = spectrum.MemoryDevice?.GetState();
                MemoryDeviceStateType = MemoryDeviceState?.GetType().AssemblyQualifiedName;
                PortDeviceState = spectrum.PortDevice?.GetState();
                PortDeviceStateType = PortDeviceState?.GetType().AssemblyQualifiedName;
                ScreenDeviceState = spectrum.ScreenDevice?.GetState();
                ScreenDeviceStateType = ScreenDeviceState?.GetType().AssemblyQualifiedName;
                InterruptDeviceState = spectrum.InterruptDevice?.GetState();
                InterruptDeviceStateType = InterruptDeviceState?.GetType().AssemblyQualifiedName;
                KeyboardDeviceState = spectrum.KeyboardDevice?.GetState();
                KeyboardDeviceStateType = KeyboardDeviceState?.GetType().AssemblyQualifiedName;
                BeeperDeviceState = spectrum.BeeperDevice?.GetState();
                BeeperDeviceStateType = BeeperDeviceState?.GetType().AssemblyQualifiedName;
                SoundDeviceState = spectrum.SoundDevice?.GetState();
                SoundDeviceStateType = SoundDeviceState?.GetType().AssemblyQualifiedName;
                TapeDeviceState = spectrum.TapeLoadDevice?.GetState();
                TapeDeviceStateType = TapeDeviceState?.GetType().AssemblyQualifiedName;
            }

            /// <summary>
            /// Restores the advice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public void RestoreDeviceState(IDevice device)
            {
                if (!(device is SpectrumEngine spectrum)) return;

                spectrum.LastFrameStartCpuTick = LastFrameStartCpuTick;
                spectrum.LastRenderedUlaTact = LastRenderedUlaTact;
                spectrum.FrameCount = FrameCount;
                spectrum.FrameTacts = FrameTacts;
                spectrum.Overflow = Overflow;

                spectrum.Cpu?.RestoreState(Z80CpuState);
                spectrum.RomDevice?.RestoreState(RomDeviceState);
                spectrum.MemoryDevice?.RestoreState(MemoryDeviceState);
                spectrum.PortDevice?.RestoreState(PortDeviceState);
                spectrum.ScreenDevice?.RestoreState(ScreenDeviceState);
                spectrum.InterruptDevice?.RestoreState(InterruptDeviceState);
                spectrum.KeyboardDevice?.RestoreState(KeyboardDeviceState);
                spectrum.BeeperDevice?.RestoreState(BeeperDeviceState);
                spectrum.SoundDevice?.RestoreState(SoundDeviceState);
                spectrum.TapeLoadDevice?.RestoreState(TapeDeviceState);
            }
        }

        #endregion
    }

#pragma warning restore
}