﻿using System;
using System.Threading.Tasks;
using Spect.Net.Assembler.Assembler;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.Commands
{
    /// <summary>
    /// Debug a Z80 program command
    /// </summary>
    [CommandId(0x0801)]
    public class DebugZ80CodeCommand : RunZ80CodeCommand
    {
        /// <summary>
        /// Override this command to start the ZX Spectrum virtual machine
        /// </summary>
        protected override async Task ResumeVm()
        {
            await Package.MachineViewModel.StartDebugVm();
        }

        /// <summary>
        /// Override this method to prepare assembler options
        /// </summary>
        /// <returns>Options to use with the assembler</returns>
        protected override AssemblerOptions PrepareOptions()
        {
            var options = base.PrepareOptions();
            var debugOptions = Package.Options.DebugSymbols;
            if (debugOptions != null)
            {
                var symbols = debugOptions.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var symbol in symbols)
                {
                    if (!options.PredefinedSymbols.Contains(symbol))
                    {
                        options.PredefinedSymbols.Add(symbol);
                    }
                }
            }
            return options;
        }
    }
}