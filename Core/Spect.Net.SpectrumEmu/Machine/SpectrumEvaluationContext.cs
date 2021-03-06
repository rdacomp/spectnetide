using Spect.Net.EvalParser.SyntaxTree;
using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// This class defines an evaluation context that uses a ZX Spectrum virtual machine instance
    /// </summary>
    public class SpectrumEvaluationContext: IExpressionEvaluationContext
    {
        /// <summary>
        /// The ZX Spectrum virtual machine
        /// </summary>
        public ISpectrumVm SpectrumVm { get; }

        public SpectrumEvaluationContext(ISpectrumVm spectrumVm)
        {
            SpectrumVm = spectrumVm;
            spectrumVm.DebugExpressionContext = this;
        }

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public virtual ExpressionValue GetSymbolValue(string symbol)
        {
            return ExpressionValue.Error;
        }

        /// <summary>
        /// Gets the current value of the specified Z80 register
        /// </summary>
        /// <param name="registerName">Name of the register</param>
        /// <param name="is8Bit">Is it an 8-bit register?</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetZ80RegisterValue(string registerName, out bool is8Bit)
        {
            is8Bit = true;
            var regs = SpectrumVm.Cpu.Registers;
            switch (registerName.ToLower())
            {
                case "a":
                    return new ExpressionValue(regs.A);
                case "b":
                    return new ExpressionValue(regs.B);
                case "c":
                    return new ExpressionValue(regs.C);
                case "d":
                    return new ExpressionValue(regs.D);
                case "e":
                    return new ExpressionValue(regs.E);
                case "h":
                    return new ExpressionValue(regs.H);
                case "l":
                    return new ExpressionValue(regs.L);
                case "f":
                    return new ExpressionValue(regs.F);
                case "i":
                    return new ExpressionValue(regs.I);
                case "r":
                    return new ExpressionValue(regs.R);
                case "xh":
                case "ixh":
                    return new ExpressionValue(regs.XH);
                case "xl":
                case "ixl":
                    return new ExpressionValue(regs.XL);
                case "yh":
                case "iyh":
                    return new ExpressionValue(regs.YH);
                case "yl":
                case "iyl":
                    return new ExpressionValue(regs.YL);
                case "af":
                    is8Bit = false;
                    return new ExpressionValue(regs.AF);
                case "bc":
                    is8Bit = false;
                    return new ExpressionValue(regs.BC);
                case "de":
                    is8Bit = false;
                    return new ExpressionValue(regs.DE);
                case "hl":
                    is8Bit = false;
                    return new ExpressionValue(regs.HL);
                case "af'":
                    is8Bit = false;
                    return new ExpressionValue(regs._AF_);
                case "bc'":
                    is8Bit = false;
                    return new ExpressionValue(regs._BC_);
                case "de'":
                    is8Bit = false;
                    return new ExpressionValue(regs._DE_);
                case "hl'":
                    is8Bit = false;
                    return new ExpressionValue(regs._HL_);
                case "ix":
                    is8Bit = false;
                    return new ExpressionValue(regs.IX);
                case "iy":
                    is8Bit = false;
                    return new ExpressionValue(regs.IY);
                case "pc":
                    is8Bit = false;
                    return new ExpressionValue(regs.PC);
                case "sp":
                    is8Bit = false;
                    return new ExpressionValue(regs.SP);
                case "wz":
                    is8Bit = false;
                    return new ExpressionValue(regs.WZ);
                default:
                    return ExpressionValue.Error;
            }
        }

        /// <summary>
        /// Gets the current value of the specified Z80 flag
        /// </summary>
        /// <param name="flagName">Name of the flag</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetZ80FlagValue(string flagName)
        {
            var regs = SpectrumVm.Cpu.Registers;
            switch (flagName.Substring(1).ToLower())
            {
                case "z":
                    return new ExpressionValue(regs.ZFlag);
                case "nz":
                    return new ExpressionValue(!regs.ZFlag);
                case "c":
                    return new ExpressionValue(regs.CFlag);
                case "nc":
                    return new ExpressionValue(!regs.CFlag);
                case "pe":
                    return new ExpressionValue(regs.PFlag);
                case "po":
                    return new ExpressionValue(!regs.PFlag);
                case "m":
                    return new ExpressionValue(regs.SFlag);
                case "p":
                    return new ExpressionValue(!regs.SFlag);
                case "h":
                    return new ExpressionValue(regs.HFlag);
                case "nh":
                    return new ExpressionValue(!regs.HFlag);
                case "n":
                    return new ExpressionValue(regs.NFlag);
                case "nn":
                    return new ExpressionValue(!regs.NFlag);
                case "3":
                    return new ExpressionValue(regs.R3Flag);
                case "n3":
                    return new ExpressionValue(!regs.R3Flag);
                case "5":
                    return new ExpressionValue(regs.R5Flag);
                case "n5":
                    return new ExpressionValue(!regs.R5Flag);
                default:
                    return ExpressionValue.Error;
            }
        }

        /// <summary>
        /// Gets the current value of the memory pointed by the specified Z80 register
        /// </summary>
        /// <param name="address">Memory address</param>
        /// <returns>Z80 register value</returns>
        public ExpressionValue GetMemoryIndirectValue(ExpressionValue address)
        {
            var memAddress = (ushort)address.Value;
            return new ExpressionValue(SpectrumVm.MemoryDevice.Read(memAddress, true));
        }
    }
}