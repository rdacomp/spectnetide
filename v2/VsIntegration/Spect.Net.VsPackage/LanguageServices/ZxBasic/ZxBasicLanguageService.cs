﻿using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Runtime.InteropServices;

namespace Spect.Net.VsPackage.LanguageServices.ZxBasic
{
    /// <summary>
    /// This class implements the language service that can handle the Z80 Assembly
    /// language.
    /// </summary>
    [Guid("FFD2DBF0-6E11-485A-8F6B-2AE9330A6548")]
    public class ZxBasicLanguageService : LanguageService
    {
        public const string LANGUAGE_NAME = "ZxBasic";

        /// <summary>
        /// Cache language preferences here
        /// </summary>
        private LanguagePreferences _preferences;

        public ZxBasicLanguageService(object site)
        {
            SetSite(site);
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (_preferences == null)
            {
                _preferences = new LanguagePreferences(Site, typeof(ZxBasicLanguageService).GUID, Name);
                if (_preferences != null)
                {
                    _preferences.Init();

                    _preferences.EnableCodeSense = true;
                    _preferences.EnableMatchBraces = true;
                    _preferences.EnableMatchBracesAtCaret = true;
                    _preferences.EnableShowMatchingBrace = true;
                    _preferences.EnableCommenting = true;
                    _preferences.HighlightMatchingBraceFlags = _HighlightMatchingBraceFlags.HMB_USERECTANGLEBRACES;
                    _preferences.LineNumbers = false;
                    _preferences.MaxErrorMessages = 100;
                    _preferences.AutoOutlining = false;
                    _preferences.MaxRegionTime = 2000;
                    _preferences.InsertTabs = false;
                    _preferences.IndentSize = 2;
                    _preferences.IndentStyle = IndentingStyle.Smart;

                    _preferences.WordWrap = false;
                    _preferences.WordWrapGlyphs = false;

                    _preferences.AutoListMembers = true;
                    _preferences.EnableQuickInfo = true;
                    _preferences.ParameterInformation = true;
                }
            }

            return _preferences;
        }

        /// <summary>
        /// Signs that Z80Assembler does not use a scanner
        /// </summary>
        /// <param name="buffer">Text lines in the buffer</param>
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return null;
        }

        /// <summary>
        /// Signs that we do not use a parse source
        /// </summary>
        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return null;
        }

        /// <summary>
        /// Retrieves the filter list for the language file
        /// </summary>
        public override string GetFormatFilterList()
        {
            return "ZX Basic File (*.zxbas)|*.zxbas";
        }

        /// <summary>
        /// Gets the name of the language
        /// </summary>
        public override string Name => LANGUAGE_NAME;

        /// <summary>
        /// Cleanup the sources, uiShell, shell, preferences and imageList objects
        /// and unregister this language service with VS.
        /// </summary>
        public override void Dispose()
        {
            try
            {
                if (_preferences != null)
                {
                    _preferences.Dispose();
                    _preferences = null;
                }
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}
