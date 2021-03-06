using System.Collections.Generic;
using Spect.Net.SpectrumEmu.Abstraction.Configuration;
using Spect.Net.SpectrumEmu.Abstraction.Discovery;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Memory;
using Spect.Net.SpectrumEmu.Devices.Ports;
using Spect.Net.SpectrumEmu.Devices.Rom;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.SpectrumEmu.Providers;

namespace Spect.Net.SpectrumEmu.Test.Helpers
{
    public class Spectrum128AdvancedTestMachine: SpectrumEngine, IStackDebugSupport
    {
        public List<StackPointerManipulationEvent> StackPointerManipulations { get; }

        public List<StackContentManipulationEvent> StackContentManipulations { get; }

        public List<BranchEvent> BranchEvents { get; }

        private readonly Stack<ushort> _stepOutStack = new Stack<ushort>();
        /// 
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public Spectrum128AdvancedTestMachine(IScreenFrameProvider renderer = null, 
            IScreenConfiguration screenConfig = null, ICpuConfiguration cpuConfig = null): 
            base(new DeviceInfoCollection
            {
                new CpuDeviceInfo(cpuConfig ?? SpectrumModels.ZxSpectrum128Pal.Cpu),
                new RomDeviceInfo(new DefaultRomProvider(), 
                    new RomConfigurationData
                    {
                        NumberOfRoms = 2,
                        RomName = "ZxSpectrum128",
                        Spectrum48RomIndex = 1
                    }, 
                    new SpectrumRomDevice()),
                new MemoryDeviceInfo(new MemoryConfigurationData
                {
                    SupportsBanking = true,
                    RamBanks = 8
                }, new Spectrum128MemoryDevice()),
                new PortDeviceInfo(null, new Spectrum128PortDevice()),
                new BeeperDeviceInfo(new AudioConfigurationData
                {
                    AudioSampleRate = 35000,
                    SamplesPerFrame = 699,
                    TactsPerSample = 100
                }, null),
                new ScreenDeviceInfo(screenConfig ?? SpectrumModels.ZxSpectrum128Pal.Screen, 
                    renderer ?? new TestPixelRenderer(screenConfig ?? SpectrumModels.ZxSpectrum48Pal.Screen)),
                new SoundDeviceInfo(new AudioConfigurationData
                {
                    AudioSampleRate = 55420,
                    SamplesPerFrame = 1107,
                    TactsPerSample = 64
                }, null)
            })
        {
            StackPointerManipulations = new List<StackPointerManipulationEvent>();
            StackContentManipulations = new List<StackContentManipulationEvent>();
            BranchEvents = new List<BranchEvent>();
            Cpu.StackDebugSupport = this;
        }

        /// <summary>
        /// Initializes the code passed in <paramref name="programCode"/>. This code
        /// is put into the memory from <paramref name="codeAddress"/> and
        /// </summary>
        /// <param name="programCode">Program code</param>
        /// <param name="codeAddress">Address of first code byte</param>
        /// <param name="startAddress">Code start address, null if same as the first byte</param>
        public void InitCode(IEnumerable<byte> programCode = null, ushort codeAddress = 0x8000, 
            ushort? startAddress = null)
        {
            if (programCode == null) return;
            if (startAddress == null) startAddress = codeAddress;

            // --- Initialize the code
            foreach (var op in programCode)
            {
                WriteSpectrumMemory(codeAddress++, op);
            }
            while (codeAddress != 0)
            {
                WriteSpectrumMemory(codeAddress++, 0);
            }

            Cpu.Reset();
            Cpu.Registers.PC = startAddress.Value;
        }

        /// <summary>
        /// Resets the debug support
        /// </summary>
        void IStackDebugSupport.Reset()
        {
        }

        /// <summary>
        /// Records a stack pointer manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackPointerManipulationEvent(StackPointerManipulationEvent ev)
        {
            StackPointerManipulations.Add(ev);
        }

        /// <summary>
        /// Records a stack content manipulation event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordStackContentManipulationEvent(StackContentManipulationEvent ev)
        {
            StackContentManipulations.Add(ev);
        }

        /// <summary>
        /// Checks if the Step-Out stack contains any information
        /// </summary>
        /// <returns></returns>
        public bool HasStepOutInfo()
        {
            return _stepOutStack.Count > 0;
        }

        /// <summary>
        /// The depth of the Step-Out stack
        /// </summary>
        public int StepOutStackDepth => _stepOutStack.Count;

        /// <summary>
        /// Clears the content of the Step-Out stack
        /// </summary>
        public void ClearStepOutStack()
        {
            _stepOutStack.Clear();
        }

        /// <summary>
        /// Pushes the specified return address to the Step-Out stack
        /// </summary>
        /// <param name="address"></param>
        public void PushStepOutAddress(ushort address)
        {
            _stepOutStack.Push(address);
        }

        /// <summary>
        /// Pops a Step-Out return point address from the stack
        /// </summary>
        /// <returns>Address popped from the stack</returns>
        /// <returns>Zeor, if the Step-Out stack is empty</returns>
        public ushort PopStepOutAddress()
        {
            if (_stepOutStack.Count > 0)
            {
                StepOutAddress = _stepOutStack.Pop();
                return StepOutAddress.Value;
            }
            StepOutAddress = null;
            return 0;
        }

        /// <summary>
        /// Indicates that the last instruction executed by the CPU was a CALL
        /// </summary>
        public bool CallExecuted { get; set; }

        /// <summary>
        /// Indicates that the last instruction executed by the CPU was a RET
        /// </summary>
        public bool RetExecuted { get; set; }

        /// <summary>
        /// Gets the last popped Step-Out address
        /// </summary>
        public ushort? StepOutAddress { get; set; }

        /// <summary>
        /// Records a branching event
        /// </summary>
        /// <param name="ev">Event information</param>
        public void RecordBranchEvent(BranchEvent ev)
        {
            BranchEvents.Add(ev);
        }
    }
}