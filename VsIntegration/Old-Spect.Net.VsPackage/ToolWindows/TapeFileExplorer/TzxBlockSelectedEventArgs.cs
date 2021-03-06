using System;
using Spect.Net.VsPackage.CustomEditors.TzxEditor;

namespace Spect.Net.VsPackage.ToolWindows.TapeFileExplorer
{
    /// <summary>
    /// Represetns the arguments of the event when a TZX block has been selected
    /// </summary>
    public class TzxBlockSelectedEventArgs: EventArgs
    {
        /// <summary>
        /// The selected block
        /// </summary>
        public TapeBlockViewModelBase Block { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.EventArgs" /> class.</summary>
        public TzxBlockSelectedEventArgs(TapeBlockViewModelBase block)
        {
            Block = block;
        }
    }
}