using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Spect.Net.SpectrumEmu.Cpu;
using Spect.Net.VsPackage.Utility;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// This view model represents a memory line with 2x8 bytes displayed
    /// </summary>
    public class MemoryLineViewModel: EnhancedViewModelBase
    {
        private static readonly Regex s_ColorSpecRegex = new Regex(@"^\s*#([a-fA-F0-9]{2})([a-fA-F0-9]{2})([a-fA-F0-9]{2})$");

        private static Brush s_SymbolBrush;
        private static Brush s_AnnBrush;
        private static Brush s_BcBrush;
        private static Brush s_DeBrush;
        private static Brush s_HlBrush;
        private static Brush s_IxBrush;
        private static Brush s_IyBrush;
        private static Brush s_SpBrush;
        private static Brush s_PcBrush;

        private readonly Registers _regs;
        private BankAwareToolWindowViewModelBase _bankViewModel;
        private string _addr1;
        private string _value0;
        private string _value1;
        private string _value2;
        private string _value3;
        private string _value4;
        private string _value5;
        private string _value6;
        private string _value7;
        private string _dump1;
        private string _addr2;
        private string _value8;
        private string _value9;
        private string _valueA;
        private string _valueB;
        private string _valueC;
        private string _valueD;
        private string _valueE;
        private string _valueF;
        private string _dump2;
        private Brush _mark0;
        private Brush _mark1;
        private Brush _mark2;
        private Brush _mark3;
        private Brush _mark4;
        private Brush _mark5;
        private Brush _mark6;
        private Brush _mark7;
        private Brush _mark8;
        private Brush _mark9;
        private Brush _markA;
        private Brush _markB;
        private Brush _markC;
        private Brush _markD;
        private Brush _markE;
        private Brush _markF;
        private int _tag0;
        private int _tag1;
        private int _tag2;
        private int _tag3;
        private int _tag4;
        private int _tag5;
        private int _tag6;
        private int _tag7;
        private int _tag8;
        private int _tag9;
        private int _tagA;
        private int _tagB;
        private int _tagC;
        private int _tagD;
        private int _tagE;
        private int _tagF;
        private Brush _symbolMark0;
        private Brush _symbolMark1;
        private Brush _symbolMark2;
        private Brush _symbolMark3;
        private Brush _symbolMark4;
        private Brush _symbolMark5;
        private Brush _symbolMark6;
        private Brush _symbolMark7;
        private Brush _symbolMark8;
        private Brush _symbolMark9;
        private Brush _symbolMarkA;
        private Brush _symbolMarkB;
        private Brush _symbolMarkC;
        private Brush _symbolMarkD;
        private Brush _symbolMarkE;
        private Brush _symbolMarkF;

        /// <summary>
        /// Base address of the memory line
        /// </summary>
        public int BaseAddress { get; }

        /// <summary>
        /// Top address of the memory line
        /// </summary>
        public int TopAddress { get; }

        public string Addr1
        {
            get => _addr1;
            set => Set(ref _addr1, value);
        }

        public string Value0
        {
            get => _value0;
            set => Set(ref _value0, value);
        }

        public string Value1
        {
            get => _value1;
            set => Set(ref _value1, value);
        }

        public string Value2
        {
            get => _value2;
            set => Set(ref _value2, value);
        }

        public string Value3
        {
            get => _value3;
            set => Set(ref _value3, value);
        }

        public string Value4
        {
            get => _value4;
            set => Set(ref _value4, value);
        }

        public string Value5
        {
            get => _value5;
            set => Set(ref _value5, value);
        }

        public string Value6
        {
            get => _value6;
            set => Set(ref _value6, value);
        }

        public string Value7
        {
            get => _value7;
            set => Set(ref _value7, value);
        }

        public string Dump1
        {
            get => _dump1;
            set => Set(ref _dump1, value);
        }

        public string Addr2
        {
            get => _addr2;
            set => Set(ref _addr2, value);
        }

        public string Value8
        {
            get => _value8;
            set => Set(ref _value8, value);
        }

        public string Value9
        {
            get => _value9;
            set => Set(ref _value9, value);
        }

        public string ValueA
        {
            get => _valueA;
            set => Set(ref _valueA, value);
        }

        public string ValueB
        {
            get => _valueB;
            set => Set(ref _valueB, value);
        }

        public string ValueC
        {
            get => _valueC;
            set => Set(ref _valueC, value);
        }

        public string ValueD
        {
            get => _valueD;
            set => Set(ref _valueD, value);
        }

        public string ValueE
        {
            get => _valueE;
            set => Set(ref _valueE, value);
        }

        public string ValueF
        {
            get => _valueF;
            set => Set(ref _valueF, value);
        }

        public string Dump2
        {
            get => _dump2;
            set => Set(ref _dump2, value);
        }

        public Brush Mark0
        {
            get => _mark0;
            set => Set( ref _mark0, value);
        }

        public Brush Mark1
        {
            get => _mark1;
            set => Set(ref _mark1, value);
        }

        public Brush Mark2
        {
            get => _mark2;
            set => Set(ref _mark2, value);
        }

        public Brush Mark3
        {
            get => _mark3;
            set => Set(ref _mark3, value);
        }

        public Brush Mark4
        {
            get => _mark4;
            set => Set(ref _mark4, value);
        }

        public Brush Mark5
        {
            get => _mark5;
            set => Set(ref _mark5, value);
        }

        public Brush Mark6
        {
            get => _mark6;
            set => Set(ref _mark6, value);
        }

        public Brush Mark7
        {
            get => _mark7;
            set => Set(ref _mark7, value);
        }

        public Brush Mark8
        {
            get => _mark8;
            set => Set(ref _mark8, value);
        }

        public Brush Mark9
        {
            get => _mark9;
            set => Set(ref _mark9, value);
        }

        public Brush MarkA
        {
            get => _markA;
            set => Set(ref _markA, value);
        }

        public Brush MarkB
        {
            get => _markB;
            set => Set(ref _markB, value);
        }

        public Brush MarkC
        {
            get => _markC;
            set => Set(ref _markC, value);
        }

        public Brush MarkD
        {
            get => _markD;
            set => Set(ref _markD, value);
        }

        public Brush MarkE
        {
            get => _markE;
            set => Set(ref _markE, value);
        }

        public Brush MarkF
        {
            get => _markF;
            set => Set(ref _markF, value);
        }

        public Brush SymbolMark0
        {
            get => _symbolMark0;
            set => Set(ref _symbolMark0, value);
        }

        public Brush SymbolMark1
        {
            get => _symbolMark1;
            set => Set(ref _symbolMark1, value);
        }

        public Brush SymbolMark2
        {
            get => _symbolMark2;
            set => Set(ref _symbolMark2, value);
        }

        public Brush SymbolMark3
        {
            get => _symbolMark3;
            set => Set(ref _symbolMark3, value);
        }

        public Brush SymbolMark4
        {
            get => _symbolMark4;
            set => Set(ref _symbolMark4, value);
        }

        public Brush SymbolMark5
        {
            get => _symbolMark5;
            set => Set(ref _symbolMark5, value);
        }

        public Brush SymbolMark6
        {
            get => _symbolMark6;
            set => Set(ref _symbolMark6, value);
        }

        public Brush SymbolMark7
        {
            get => _symbolMark7;
            set => Set(ref _symbolMark7, value);
        }

        public Brush SymbolMark8
        {
            get => _symbolMark8;
            set => Set(ref _symbolMark8, value);
        }

        public Brush SymbolMark9
        {
            get => _symbolMark9;
            set => Set(ref _symbolMark9, value);
        }

        public Brush SymbolMarkA
        {
            get => _symbolMarkA;
            set => Set(ref _symbolMarkA, value);
        }

        public Brush SymbolMarkB
        {
            get => _symbolMarkB;
            set => Set(ref _symbolMarkB, value);
        }

        public Brush SymbolMarkC
        {
            get => _symbolMarkC;
            set => Set(ref _symbolMarkC, value);
        }

        public Brush SymbolMarkD
        {
            get => _symbolMarkD;
            set => Set(ref _symbolMarkD, value);
        }

        public Brush SymbolMarkE
        {
            get => _symbolMarkE;
            set => Set(ref _symbolMarkE, value);
        }

        public Brush SymbolMarkF
        {
            get => _symbolMarkF;
            set => Set(ref _symbolMarkF, value);
        }
        
        public int Tag0
        {
            get => _tag0;
            set => Set(ref _tag0, value);
        }

        public int Tag1
        {
            get => _tag1;
            set => Set(ref _tag1, value);
        }

        public int Tag2
        {
            get => _tag2;
            set => Set(ref _tag2, value);
        }

        public int Tag3
        {
            get => _tag3;
            set => Set(ref _tag3, value);
        }

        public int Tag4
        {
            get => _tag4;
            set => Set(ref _tag4, value);
        }

        public int Tag5
        {
            get => _tag5;
            set => Set(ref _tag5, value);
        }

        public int Tag6
        {
            get => _tag6;
            set => Set(ref _tag6, value);
        }

        public int Tag7
        {
            get => _tag7;
            set => Set(ref _tag7, value);
        }

        public int Tag8
        {
            get => _tag8;
            set => Set(ref _tag8, value);
        }

        public int Tag9
        {
            get => _tag9;
            set => Set(ref _tag9, value);
        }

        public int TagA
        {
            get => _tagA;
            set => Set(ref _tagA, value);
        }

        public int TagB
        {
            get => _tagB;
            set => Set(ref _tagB, value);
        }

        public int TagC
        {
            get => _tagC;
            set => Set(ref _tagC, value);
        }

        public int TagD
        {
            get => _tagD;
            set => Set(ref _tagD, value);
        }

        public int TagE
        {
            get => _tagE;
            set => Set(ref _tagE, value);
        }

        public int TagF
        {
            get => _tagF;
            set => Set(ref _tagF, value);
        }

        // ReSharper disable once UnusedMember.Global
        public MemoryLineViewModel()
        {
            if (IsInDesignMode)
            {
                Addr1 = "4000";
                Value0 = "01";
                Value1 = "12";
                Value2 = "23";
                Value3 = "34";
                Value4 = "45";
                Value5 = "56";
                Value6 = "67";
                Value7 = "78";
                Dump1 = "..34..78";
                Addr2 = "4008";
                Value8 = "89";
                Value9 = "9A";
                ValueA = "AB";
                ValueB = "BC";
                ValueC = "CD";
                ValueD = "DE";
                ValueE = "EF";
                ValueF = "F0";
                Dump2 = "..AB..EF";
            }

            Mark0 = Mark1 = Mark2 = Mark3 = Mark4 = Mark5 = Mark6 = Mark7 =
            Mark8 = Mark9 = MarkA = MarkB = MarkC = MarkD = MarkE = MarkF =
                Brushes.Transparent;
        }

        /// <summary>
        /// Creates a memory line with the specified base address and top address
        /// </summary>
        /// <param name="regs">Z80 register current values</param>
        /// <param name="baseAddr">Memory base address</param>
        /// <param name="topAddress">Memory top address</param>
        public MemoryLineViewModel(Registers regs, int baseAddr, int topAddress = 0xFFFF)
        {
            _regs = regs;
            BaseAddress = baseAddr;
            TopAddress = topAddress;
        }

        /// <summary>
        /// Binds this memory line to the specified memory address
        /// </summary>
        /// <param name="memory">Memory array</param>
        /// <param name="bankViewModel">Optional view model to set symbol border</param>
        public void BindTo(byte[] memory, BankAwareToolWindowViewModelBase bankViewModel = null)
        {
            _bankViewModel = bankViewModel;
            Addr1 = BaseAddress.AsHexWord();
            Dump1 = DumpValue(memory, BaseAddress);
            Value0 = GetByte(memory, 0);
            Mark0 = GetBrush(0);
            Tag0 = BaseAddress + 0;
            Value1 = GetByte(memory, 1);
            Mark1 = GetBrush(1);
            Tag1 = BaseAddress + 1;
            Value2 = GetByte(memory, 2);
            Mark2 = GetBrush(2);
            Tag2 = BaseAddress + 2;
            Value3 = GetByte(memory, 3);
            Mark3 = GetBrush(3);
            Tag3 = BaseAddress + 3;
            Value4 = GetByte(memory, 4);
            Mark4 = GetBrush(4);
            Tag4 = BaseAddress + 4;
            Value5 = GetByte(memory, 5);
            Mark5 = GetBrush(5);
            Tag5 = BaseAddress + 5;
            Value6 = GetByte(memory, 6);
            Mark6 = GetBrush(6);
            Tag6 = BaseAddress + 6;
            Value7 = GetByte(memory, 7);
            Mark7 = GetBrush(7);
            Tag7 = BaseAddress + 7;

            if (BaseAddress + 8 > TopAddress) return;

            Addr2 = (BaseAddress + 8).AsHexWord();
            Dump2 = DumpValue(memory, BaseAddress + 8);
            Value8 = GetByte(memory, 8);
            Mark8 = GetBrush(8);
            Tag8 = BaseAddress + 8;
            Value9 = GetByte(memory, 9);
            Mark9 = GetBrush(9);
            Tag9 = BaseAddress + 9;
            ValueA = GetByte(memory, 10);
            MarkA = GetBrush(10);
            TagA = BaseAddress + 10;
            ValueB = GetByte(memory, 11);
            MarkB = GetBrush(11);
            TagB = BaseAddress + 11;
            ValueC = GetByte(memory, 12);
            MarkC = GetBrush(12);
            TagC = BaseAddress + 12;
            ValueD = GetByte(memory, 13);
            MarkD = GetBrush(13);
            TagD = BaseAddress + 13;
            ValueE = GetByte(memory, 14);
            MarkE = GetBrush(14);
            TagE = BaseAddress + 14;
            ValueF = GetByte(memory, 15);
            MarkF = GetBrush(15);
            TagF = BaseAddress + 15;

            if (bankViewModel?.AnnotationHandler != null)
            {
                SymbolMark0 = GetSymbolBrush(bankViewModel, 0);
                SymbolMark1 = GetSymbolBrush(bankViewModel, 1);
                SymbolMark2 = GetSymbolBrush(bankViewModel, 2);
                SymbolMark3 = GetSymbolBrush(bankViewModel, 3);
                SymbolMark4 = GetSymbolBrush(bankViewModel, 4);
                SymbolMark5 = GetSymbolBrush(bankViewModel, 5);
                SymbolMark6 = GetSymbolBrush(bankViewModel, 6);
                SymbolMark7 = GetSymbolBrush(bankViewModel, 7);
                SymbolMark8 = GetSymbolBrush(bankViewModel, 8);
                SymbolMark9 = GetSymbolBrush(bankViewModel, 9);
                SymbolMarkA = GetSymbolBrush(bankViewModel, 10);
                SymbolMarkB = GetSymbolBrush(bankViewModel, 11);
                SymbolMarkC = GetSymbolBrush(bankViewModel, 12);
                SymbolMarkD = GetSymbolBrush(bankViewModel, 13);
                SymbolMarkE = GetSymbolBrush(bankViewModel, 14);
                SymbolMarkF = GetSymbolBrush(bankViewModel, 15);
            }
        }

        public List<string> GetAffectedRegisters(int address)
        {
            var result = new List<string>();
            if (_regs != null)
            {
                if (_regs.BC == address) result.Add("BC");
                if (_regs.DE == address) result.Add("DE");
                if (_regs.HL == address) result.Add("HL");
                if (_regs.IX == address) result.Add("IX");
                if (_regs.IY == address) result.Add("IY");
                if (_regs.PC == address) result.Add("PC");
                if (_regs.SP == address) result.Add("SP");
            }
            return result;
        }

        public List<string> GetAffectedSymbols(ushort address)
        {
            var result = new List<string>();
            if (_bankViewModel == null) return result;
            if (_bankViewModel.CompilerOutput != null)
            {
                if (_bankViewModel.CompilerOutput.SymbolMap.TryGetValue(address, out var symbolList))
                {
                    result.AddRange(symbolList);
                }
            }

            if (_bankViewModel?.AnnotationHandler != null)
            {
                var ann = _bankViewModel.GetAnnotationFor(address, out var memAddress);
                if (ann != null && ann.Labels.TryGetValue(memAddress, out var symbol))
                {
                    result.Add(symbol);
                }
            }
            return result.Distinct().OrderBy(item => item).ToList();
        }

        /// <summary>
        /// Refreshes the register highlight brush colors from SpectNet options
        /// </summary>
        public static void RefreshRegisterBrushes()
        {
            s_SymbolBrush = GetBrushFromColor(SpectNetPackage.Default.Options.SymbolColor);
            s_AnnBrush = GetBrushFromColor(SpectNetPackage.Default.Options.AnnotationColor);
            s_BcBrush = GetBrushFromColor(SpectNetPackage.Default.Options.BcColor);
            s_DeBrush = GetBrushFromColor(SpectNetPackage.Default.Options.DeColor);
            s_HlBrush = GetBrushFromColor(SpectNetPackage.Default.Options.HlColor);
            s_IxBrush = GetBrushFromColor(SpectNetPackage.Default.Options.IxColor);
            s_IyBrush = GetBrushFromColor(SpectNetPackage.Default.Options.IyColor);
            s_SpBrush = GetBrushFromColor(SpectNetPackage.Default.Options.SpColor);
            s_PcBrush = GetBrushFromColor(SpectNetPackage.Default.Options.PcColor);
        }

        private static Brush GetBrushFromColor(string color)
        {
            var prop = typeof(Brushes).GetProperty(color,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static);
            if (prop != null && prop.PropertyType == typeof(SolidColorBrush))
            {
                return (SolidColorBrush) prop.GetValue(null);
            }

            var match = s_ColorSpecRegex.Match(color);
            if (match.Success)
            {
                var red = byte.Parse(match.Groups[1].Captures[0].Value, NumberStyles.HexNumber);
                var green = byte.Parse(match.Groups[2].Captures[0].Value, NumberStyles.HexNumber);
                var blue = byte.Parse(match.Groups[3].Captures[0].Value, NumberStyles.HexNumber);
                return new SolidColorBrush(Color.FromRgb(red, green, blue));
            }
            return Brushes.Transparent;
        }

        private string DumpValue(IReadOnlyList<byte> memory, int startAddr)
        {
            var sb = new StringBuilder(8);
            for (var i = 0; i < 8; i++)
            {
                if (startAddr + i > TopAddress) break;
                if (memory == null)
                {
                    sb.Append('-');
                }
                else
                {
                    var ch = (char) memory[startAddr + i];
                    sb.Append(char.IsControl(ch) ? '.' : ch);
                }
            }
            return sb.ToString();
        }

        private string GetByte(byte[] memory, int offset)
        {
            var memAddr = BaseAddress + offset;
            return memAddr <= TopAddress ? (memory == null ? "--" : memory[memAddr].AsHexaByte()) : null;
        }

        private Brush GetBrush(int offset)
        {
            if (_regs != null)
            {
                var addr = BaseAddress + offset;
                if (_regs.BC == addr) return s_BcBrush;
                if (_regs.DE == addr) return s_DeBrush;
                if (_regs.HL == addr) return s_HlBrush;
                if (_regs.IX == addr) return s_IxBrush;
                if (_regs.IY == addr) return s_IyBrush;
                if (_regs.PC == addr) return s_PcBrush;
                if (_regs.SP == addr) return s_SpBrush;
            }
            return Brushes.Transparent;
        }

        private Brush GetSymbolBrush(BankAwareToolWindowViewModelBase vm, int offset)
        {
            var addr = (ushort)(BaseAddress + offset);
            if (vm.CompilerOutput?.SymbolMap.ContainsKey(addr) == true)
            {
                return s_SymbolBrush;
            }

            var ann = vm.GetAnnotationFor(addr, out var memAddress);
            return ann != null && ann.Labels.ContainsKey(memAddress) 
                ? s_AnnBrush 
                : Brushes.Transparent;
        }
    }
}