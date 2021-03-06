using Spect.Net.SpectrumEmu.Abstraction.Devices.Keyboard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using WindowsKeyboard = System.Windows.Input.Keyboard;

namespace Spect.Net.VsPackage.ToolWindows.SpectrumEmulator
{
    /// <summary>
    /// This class is responsible for scanning the entire keyboard
    /// </summary>
    public class KeyboardScanner
    {
        // --- Keyboard layout codes to define separate key mappings for each of them
        private const string ENG_US_LAYOUT = "00000409";
        private const string HUN_LAYOUT = "0000040E";
        private const string HUN_101_LAYOUT = "0001040E";

        // --- You can create a default layout, provided you have non-implemented custom layout
        private const string DEFAULT_LAYOUT = "default";

        // --- Hungarian 101 Symbol Shift normal mappings
        private static readonly List<Key> s_Hun101SShiftNormal =
            new List<Key> 
            {
                Key.LeftShift, 
                Key.RightShift,
                Key.OemComma, 
                Key.OemPeriod, 
                Key.Decimal, 
                Key.Divide, 
                Key.Multiply,
                Key.Add, 
                Key.Subtract
            };

        // --- Hungarian 101 Caps Shift normal mappings
        private static readonly List<Key> s_Hun101CShiftNormal =
            new List<Key> 
            { 
                Key.RightAlt, 
                Key.Back,
                Key.Left, 
                Key.Up, 
                Key.Down, 
                Key.Right, 
                Key.Home
            };

        // --- Hungarian 101 Symbol Shift swapped mappings
        private static readonly List<Key> s_Hun101SShiftSwapped =
            new List<Key>
            {
                Key.RightAlt,
                Key.OemComma,
                Key.OemPeriod,
                Key.Decimal,
                Key.Divide,
                Key.Multiply,
                Key.Add,
                Key.Subtract
            };

        // --- Hungarian 101 Caps Shift swapped mappings
        private static readonly List<Key> s_Hun101CShiftSwapped =
            new List<Key>
            {
                Key.LeftShift,
                Key.RightShift,
                Key.Back,
                Key.Left,
                Key.Up,
                Key.Down,
                Key.Right,
                Key.Home
            };


        /// <summary>
        /// Maps Spectrum keys to the PC keyboard keys for Hungarian 101 keyboard layout
        /// </summary>
        /// <remarks>
        /// The key specifies the Spectrum keyboard code mapped to a physical key.
        /// The value is a collection of physical keys. If any of them changes 
        /// its state, the Spectrum key changes, too.
        /// </remarks>
        private static readonly Dictionary<SpectrumKeyCode, List<Key>> s_Hun101KeyMappings =
            new Dictionary<SpectrumKeyCode, List<Key>>
            {
                { SpectrumKeyCode.SShift, s_Hun101SShiftNormal },
                { SpectrumKeyCode.CShift, s_Hun101CShiftNormal },
                { SpectrumKeyCode.Space, new List<Key> { Key.Space} },
                { SpectrumKeyCode.Enter, new List<Key> { Key.Enter } },
                { SpectrumKeyCode.Q, new List<Key> { Key.Q } },
                { SpectrumKeyCode.W, new List<Key> { Key.W } },
                { SpectrumKeyCode.E, new List<Key> { Key.E } },
                { SpectrumKeyCode.R, new List<Key> { Key.R } },
                { SpectrumKeyCode.T, new List<Key> { Key.T } },
                { SpectrumKeyCode.Y, new List<Key> { Key.Y } },
                { SpectrumKeyCode.U, new List<Key> { Key.U } },
                { SpectrumKeyCode.I, new List<Key> { Key.I } },
                { SpectrumKeyCode.O, new List<Key> { Key.O } },
                { SpectrumKeyCode.P, new List<Key> { Key.P } },
                { SpectrumKeyCode.A, new List<Key> { Key.A } },
                { SpectrumKeyCode.S, new List<Key> { Key.S } },
                { SpectrumKeyCode.D, new List<Key> { Key.D } },
                { SpectrumKeyCode.F, new List<Key> { Key.F } },
                { SpectrumKeyCode.G, new List<Key> { Key.G } },
                { SpectrumKeyCode.H, new List<Key> { Key.H } },
                { SpectrumKeyCode.J, new List<Key> { Key.J, Key.Subtract } },
                { SpectrumKeyCode.K, new List<Key> { Key.K, Key.Add } },
                { SpectrumKeyCode.L, new List<Key> { Key.L } },
                { SpectrumKeyCode.Z, new List<Key> { Key.Z } },
                { SpectrumKeyCode.X, new List<Key> { Key.X } },
                { SpectrumKeyCode.C, new List<Key> { Key.C } },
                { SpectrumKeyCode.V, new List<Key> { Key.V, Key.Divide } },
                { SpectrumKeyCode.B, new List<Key> { Key.B, Key.Multiply } },
                { SpectrumKeyCode.N, new List<Key> { Key.N, Key.OemComma } },
                { SpectrumKeyCode.M, new List<Key> { Key.M, Key.OemPeriod, Key.Decimal } },
                { SpectrumKeyCode.N0, new List<Key> { Key.D0, Key.NumPad0, Key.Back } },
                { SpectrumKeyCode.N1, new List<Key> { Key.D1, Key.NumPad1, Key.Home } },
                { SpectrumKeyCode.N2, new List<Key> { Key.D2, Key.NumPad2 } },
                { SpectrumKeyCode.N3, new List<Key> { Key.D3, Key.NumPad3 } },
                { SpectrumKeyCode.N4, new List<Key> { Key.D4, Key.NumPad4 } },
                { SpectrumKeyCode.N5, new List<Key> { Key.D5, Key.NumPad5, Key.Left } },
                { SpectrumKeyCode.N6, new List<Key> { Key.D6, Key.NumPad6, Key.Down } },
                { SpectrumKeyCode.N7, new List<Key> { Key.D7, Key.NumPad7, Key.Up } },
                { SpectrumKeyCode.N8, new List<Key> { Key.D8, Key.NumPad8, Key.Right } },
                { SpectrumKeyCode.N9, new List<Key> { Key.D9, Key.NumPad9 } },
            };

        // --- Hungarian Symbol Shift normal mappings
        private static readonly List<Key> s_HunSShiftNormal =
            new List<Key> 
            { 
                Key.LeftShift, 
                Key.RightShift,
                Key.OemComma, 
                Key.OemPeriod, 
                Key.Decimal, 
                Key.Divide, 
                Key.Multiply,
                Key.Add, 
                Key.Subtract
            };

        // --- Hungarian Caps Shift normal mappings
        private static readonly List<Key> s_HunCShiftNormal =
            new List<Key> 
            { 
                Key.RightAlt, 
                Key.Back,
                Key.Left, 
                Key.Up, 
                Key.Down, 
                Key.Right, 
                Key.Home
            };

        // --- Hungarian Symbol Shift normal mappings
        private static readonly List<Key> s_HunSShiftSwapped =
            new List<Key>
            {
                Key.RightAlt,
                Key.OemComma,
                Key.OemPeriod,
                Key.Decimal,
                Key.Divide,
                Key.Multiply,
                Key.Add,
                Key.Subtract
            };

        // --- Hungarian Caps Shift normal mappings
        private static readonly List<Key> s_HunCShiftSwapped =
            new List<Key>
            {
                Key.LeftShift,
                Key.RightShift,
                Key.Back,
                Key.Left,
                Key.Up,
                Key.Down,
                Key.Right,
                Key.Home
            };

        /// <summary>
        /// Maps Spectrum keys to the PC keyboard keys for Hungarian keyboard layout
        /// </summary>
        /// <remarks>
        /// The key specifies the Spectrum keyboard code mapped to a physical key.
        /// The value is a collection of physical keys. If any of them changes 
        /// its state, the Spectrum key changes, too.
        /// </remarks>
        private static readonly Dictionary<SpectrumKeyCode, List<Key>> s_HunKeyMappings =
            new Dictionary<SpectrumKeyCode, List<Key>>
            {
                { SpectrumKeyCode.SShift, s_HunSShiftNormal },
                { SpectrumKeyCode.CShift, s_HunCShiftNormal },
                { SpectrumKeyCode.Space, new List<Key> { Key.Space} },
                { SpectrumKeyCode.Enter, new List<Key> { Key.Enter } },
                { SpectrumKeyCode.Q, new List<Key> { Key.Q } },
                { SpectrumKeyCode.W, new List<Key> { Key.W } },
                { SpectrumKeyCode.E, new List<Key> { Key.E } },
                { SpectrumKeyCode.R, new List<Key> { Key.R } },
                { SpectrumKeyCode.T, new List<Key> { Key.T } },
                { SpectrumKeyCode.Y, new List<Key> { Key.Z } },
                { SpectrumKeyCode.U, new List<Key> { Key.U } },
                { SpectrumKeyCode.I, new List<Key> { Key.I } },
                { SpectrumKeyCode.O, new List<Key> { Key.O } },
                { SpectrumKeyCode.P, new List<Key> { Key.P } },
                { SpectrumKeyCode.A, new List<Key> { Key.A } },
                { SpectrumKeyCode.S, new List<Key> { Key.S } },
                { SpectrumKeyCode.D, new List<Key> { Key.D } },
                { SpectrumKeyCode.F, new List<Key> { Key.F } },
                { SpectrumKeyCode.G, new List<Key> { Key.G } },
                { SpectrumKeyCode.H, new List<Key> { Key.H } },
                { SpectrumKeyCode.J, new List<Key> { Key.J, Key.Subtract } },
                { SpectrumKeyCode.K, new List<Key> { Key.K, Key.Add } },
                { SpectrumKeyCode.L, new List<Key> { Key.L } },
                { SpectrumKeyCode.Z, new List<Key> { Key.Y } },
                { SpectrumKeyCode.X, new List<Key> { Key.X } },
                { SpectrumKeyCode.C, new List<Key> { Key.C } },
                { SpectrumKeyCode.V, new List<Key> { Key.V, Key.Divide } },
                { SpectrumKeyCode.B, new List<Key> { Key.B, Key.Multiply } },
                { SpectrumKeyCode.N, new List<Key> { Key.N, Key.OemComma } },
                { SpectrumKeyCode.M, new List<Key> { Key.M, Key.OemPeriod, Key.Decimal } },
                { SpectrumKeyCode.N0, new List<Key> { Key.D0, Key.NumPad0, Key.Back } },
                { SpectrumKeyCode.N1, new List<Key> { Key.D1, Key.NumPad1, Key.Home } },
                { SpectrumKeyCode.N2, new List<Key> { Key.D2, Key.NumPad2 } },
                { SpectrumKeyCode.N3, new List<Key> { Key.D3, Key.NumPad3 } },
                { SpectrumKeyCode.N4, new List<Key> { Key.D4, Key.NumPad4 } },
                { SpectrumKeyCode.N5, new List<Key> { Key.D5, Key.NumPad5, Key.Left } },
                { SpectrumKeyCode.N6, new List<Key> { Key.D6, Key.NumPad6, Key.Down } },
                { SpectrumKeyCode.N7, new List<Key> { Key.D7, Key.NumPad7, Key.Up } },
                { SpectrumKeyCode.N8, new List<Key> { Key.D8, Key.NumPad8, Key.Right } },
                { SpectrumKeyCode.N9, new List<Key> { Key.D9, Key.NumPad9 } },
            };

        // --- English US Symbol Shift normal mappings
        private static readonly List<Key> s_EngUsSShiftNormal =
            new List<Key> 
            { 
                Key.LeftShift, 
                Key.RightShift,
                Key.OemComma, 
                Key.OemPeriod, 
                Key.Decimal, 
                Key.Divide, 
                Key.Multiply,
                Key.Add, 
                Key.Subtract
            };

        // --- English US Caps Shift normal mappings
        private static readonly List<Key> s_EngUsCShiftNormal =
            new List<Key> 
            { 
                Key.RightAlt, 
                Key.Back,
                Key.Left,
                Key.Up,
                Key.Down,
                Key.Right,
                Key.Home
            };

        // --- English US Symbol Shift swapped mappings
        private static readonly List<Key> s_EngUsSShiftSwapped =
            new List<Key>
            {
                Key.RightAlt,
                Key.OemComma,
                Key.OemPeriod,
                Key.Decimal,
                Key.Divide,
                Key.Multiply,
                Key.Add,
                Key.Subtract
            };

        // --- English US Caps Shift swapped mappings
        private static readonly List<Key> s_EngUsCShiftSwapped =
            new List<Key>
            {
                Key.LeftShift,
                Key.RightShift,
                Key.Back,
                Key.Left,
                Key.Up,
                Key.Down,
                Key.Right,
                Key.Home
            };

        /// <summary>
        /// Maps Spectrum keys to the PC keyboard keys for English US keyboard layout
        /// </summary>
        /// <remarks>
        /// The key specifies the Spectrum keyboard code mapped to a physical key.
        /// The value is a collection of physical keys. If any of them changes 
        /// its state, the Spectrum key changes, too.
        /// </remarks>
        private static readonly Dictionary<SpectrumKeyCode, List<Key>> s_EngUsKeyMappings =
            new Dictionary<SpectrumKeyCode, List<Key>>
            {
                { SpectrumKeyCode.SShift, s_EngUsSShiftNormal },
                { SpectrumKeyCode.CShift, s_EngUsCShiftNormal },
                { SpectrumKeyCode.Space, new List<Key> { Key.Space} },
                { SpectrumKeyCode.Enter, new List<Key> { Key.Enter } },
                { SpectrumKeyCode.Q, new List<Key> { Key.Q } },
                { SpectrumKeyCode.W, new List<Key> { Key.W } },
                { SpectrumKeyCode.E, new List<Key> { Key.E } },
                { SpectrumKeyCode.R, new List<Key> { Key.R } },
                { SpectrumKeyCode.T, new List<Key> { Key.T } },
                { SpectrumKeyCode.Y, new List<Key> { Key.Y } },
                { SpectrumKeyCode.U, new List<Key> { Key.U } },
                { SpectrumKeyCode.I, new List<Key> { Key.I } },
                { SpectrumKeyCode.O, new List<Key> { Key.O } },
                { SpectrumKeyCode.P, new List<Key> { Key.P } },
                { SpectrumKeyCode.A, new List<Key> { Key.A } },
                { SpectrumKeyCode.S, new List<Key> { Key.S } },
                { SpectrumKeyCode.D, new List<Key> { Key.D } },
                { SpectrumKeyCode.F, new List<Key> { Key.F } },
                { SpectrumKeyCode.G, new List<Key> { Key.G } },
                { SpectrumKeyCode.H, new List<Key> { Key.H } },
                { SpectrumKeyCode.J, new List<Key> { Key.J, Key.Subtract } },
                { SpectrumKeyCode.K, new List<Key> { Key.K, Key.Add } },
                { SpectrumKeyCode.L, new List<Key> { Key.L } },
                { SpectrumKeyCode.Z, new List<Key> { Key.Z } },
                { SpectrumKeyCode.X, new List<Key> { Key.X } },
                { SpectrumKeyCode.C, new List<Key> { Key.C } },
                { SpectrumKeyCode.V, new List<Key> { Key.V, Key.Divide } },
                { SpectrumKeyCode.B, new List<Key> { Key.B, Key.Multiply } },
                { SpectrumKeyCode.N, new List<Key> { Key.N, Key.OemComma } },
                { SpectrumKeyCode.M, new List<Key> { Key.M, Key.OemPeriod, Key.Decimal } },
                { SpectrumKeyCode.N0, new List<Key> { Key.D0, Key.NumPad0, Key.Back } },
                { SpectrumKeyCode.N1, new List<Key> { Key.D1, Key.NumPad1, Key.Home } },
                { SpectrumKeyCode.N2, new List<Key> { Key.D2, Key.NumPad2 } },
                { SpectrumKeyCode.N3, new List<Key> { Key.D3, Key.NumPad3 } },
                { SpectrumKeyCode.N4, new List<Key> { Key.D4, Key.NumPad4 } },
                { SpectrumKeyCode.N5, new List<Key> { Key.D5, Key.NumPad5, Key.Left } },
                { SpectrumKeyCode.N6, new List<Key> { Key.D6, Key.NumPad6, Key.Down } },
                { SpectrumKeyCode.N7, new List<Key> { Key.D7, Key.NumPad7, Key.Up } },
                { SpectrumKeyCode.N8, new List<Key> { Key.D8, Key.NumPad8, Key.Right } },
                { SpectrumKeyCode.N9, new List<Key> { Key.D9, Key.NumPad9 } },
            };

        /// <summary>
        /// Stores keyboard layouts and related key mappings
        /// </summary>
        private static readonly Dictionary<string, Dictionary<SpectrumKeyCode, List<Key>>> s_LayoutMappings =
            new Dictionary<string, Dictionary<SpectrumKeyCode, List<Key>>>
            {
                { DEFAULT_LAYOUT, s_Hun101KeyMappings },
                { ENG_US_LAYOUT, s_EngUsKeyMappings },
                { HUN_101_LAYOUT, s_Hun101KeyMappings },
                { HUN_LAYOUT, s_HunKeyMappings },
            };

        /// <summary>
        /// Initiate scanning the entire keyboard
        /// </summary>
        /// <remarks>
        /// If the physical keyboard is not allowed, the device can use other
        /// ways to emulate the virtual machine's keyboard
        /// </remarks>
        public List<KeyStatus> Scan()
        {
            var result = new List<KeyStatus>();
            if (!ApplicationIsActivated() || !SpectNetPackage.Default.EmulatorViewModel.EnableKeyboardScan)
            {
                return result;
            }

            // --- Obtain the layout mappings for the current keyboard layout
            var layoutBuilder = new StringBuilder(256);
            GetKeyboardLayoutName(layoutBuilder);
            var layoutId = layoutBuilder.ToString();

            // --- Obtain the mapping for the current layout
            if (!s_LayoutMappings.TryGetValue(layoutId, out var layoutMappings))
            {
                if (!s_LayoutMappings.TryGetValue(DEFAULT_LAYOUT, out layoutMappings))
                {
                    // --- No default layout 
                    return result;
                }
            }

            // --- Check the state of the keys
            foreach (var keyInfo in layoutMappings)
            {
                var keyState = keyInfo.Value.Any(WindowsKeyboard.IsKeyDown);
                result.Add(new KeyStatus(keyInfo.Key, keyState));
            }

            return result;
        }

        /// <summary>
        /// Binds keyboard shift type change to this object
        /// </summary>
        public void BindShiftChange()
        {
            SpectNetPackage.Default.Options.KeyboardShiftTypeChanged += OnKeyboardShiftChange;
        }

        /// <summary>
        /// Releases keyboard shift type change from this object
        /// </summary>
        public void ReleaseShiftChange()
        {
            SpectNetPackage.Default.Options.KeyboardShiftTypeChanged -= OnKeyboardShiftChange;
        }

        /// <summary>
        /// Responds to keyboard shift type change events
        /// </summary>
        /// <param name="shiftType"></param>
        private void OnKeyboardShiftChange(object sender, KeyboardShiftTypeChangedEventArgs args)
        {
            if (args.KeyboardShiftOptions == KeyboardShiftOptions.Normal)
            {
                s_Hun101KeyMappings[SpectrumKeyCode.SShift] = s_Hun101SShiftNormal;
                s_Hun101KeyMappings[SpectrumKeyCode.CShift] = s_Hun101CShiftNormal;
                s_HunKeyMappings[SpectrumKeyCode.SShift] = s_HunSShiftNormal;
                s_HunKeyMappings[SpectrumKeyCode.CShift] = s_HunCShiftNormal;
                s_EngUsKeyMappings[SpectrumKeyCode.SShift] = s_EngUsSShiftNormal;
                s_EngUsKeyMappings[SpectrumKeyCode.CShift] = s_EngUsCShiftNormal;
            }
            else
            {
                s_Hun101KeyMappings[SpectrumKeyCode.SShift] = s_Hun101SShiftSwapped;
                s_Hun101KeyMappings[SpectrumKeyCode.CShift] = s_Hun101CShiftSwapped;
                s_HunKeyMappings[SpectrumKeyCode.SShift] = s_HunSShiftSwapped;
                s_HunKeyMappings[SpectrumKeyCode.CShift] = s_HunCShiftSwapped;
                s_EngUsKeyMappings[SpectrumKeyCode.SShift] = s_EngUsSShiftSwapped;
                s_EngUsKeyMappings[SpectrumKeyCode.CShift] = s_EngUsCShiftSwapped;
            }
        }

        /// <summary>
        /// Retrieves the name of the active input locale identifier 
        /// (formerly called the keyboard layout) for the system.
        /// </summary>
        /// <param name="pwszKlid">
        /// The buffer (of at least KL_NAMELENGTH characters in length) 
        /// that receives the name of the input locale identifier, including 
        /// the terminating null character. This will be a copy of the string 
        /// provided to the LoadKeyboardLayout function, unless layout 
        /// substitution took place.
        /// </param>
        [DllImport("user32.dll")]
        private static extern long GetKeyboardLayoutName(StringBuilder pwszKlid);

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which 
        /// the user is currently working). The system assigns a slightly higher 
        /// priority to the thread that creates the foreground window than it 
        /// does to other threads.
        /// </summary>
        /// <returns>
        /// The return value is a handle to the foreground window.
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified 
        /// window and, optionally, the identifier of the process that created 
        /// the window.
        /// </summary>
        /// <param name="handle">
        /// A handle to the window.
        /// </param>
        /// <param name="processId">
        /// A pointer to a variable that receives the process identifier.
        /// </param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        /// <summary>
        /// Checks if the current application is activated
        /// </summary>
        private static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;
            }
            var procId = Process.GetCurrentProcess().Id;
            GetWindowThreadProcessId(activatedHandle, out var activeProcId);
            return activeProcId == procId;
        }
    }

}