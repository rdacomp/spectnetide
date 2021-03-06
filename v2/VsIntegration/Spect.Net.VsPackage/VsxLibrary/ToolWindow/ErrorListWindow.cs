using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace Spect.Net.VsPackage.VsxLibrary.ToolWindow
{
    /// <summary>
    /// This class provides access to Visual Studio's Error List tool window.
    /// </summary>
    public class ErrorListWindow
    {
        private readonly ErrorListProvider _errorListProvider;

        public ErrorListWindow()
        {
            _errorListProvider = new ErrorListProvider(SpectNetPackage.Default);
        }

        /// <summary>
        /// Clears the content of the Error List
        /// </summary>
        public void Clear()
        {
            _errorListProvider.Tasks.Clear();
        }

        /// <summary>
        /// Adds the specified error to the provider
        /// </summary>
        /// <param name="error">Error information entry</param>
        public void AddErrorTask(ErrorTask error)
        {
            _errorListProvider.Tasks.Add(error);
        }

        /// <summary>
        /// Navigates to the source code associated with the specified task
        /// </summary>
        /// <param name="task">Error task</param>
        public void Navigate(ErrorTask task)
        {
            _errorListProvider.Navigate(task, VSConstants.LOGVIEWID_Primary);
        }
    }
}