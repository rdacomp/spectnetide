using System;
using Spect.Net.Wpf.Mvvm;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// Base class for SpectNetPackage-related view models
    /// </summary>
    public abstract class SpectNetPackageToolWindowViewModelBase: EnhancedViewModelBase, IDisposable
    {
        private bool _viewInitializedWithSolution;

        /// <summary>
        /// The hosting package
        /// </summary>
        public SpectNetPackage Package => SpectNetPackage.Default;

        /// <summary>
        /// This flag shows if this tool window has already been initialized after
        /// opening the solution
        /// </summary>
        public bool ViewInitializedWithSolution
        {
            get => _viewInitializedWithSolution;
            set => Set(ref _viewInitializedWithSolution, value);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}