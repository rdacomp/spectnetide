using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Spect.Net.VsPackage.Debugging;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Spect.Net.VsPackage.LanguageServices.ZxBasic
{
    /// <summary>
    /// This class provides a classification for the ZX BASIC language
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType(ZxBasicLanguageService.LANGUAGE_NAME)]
    internal class ZxBasicClassifierProvider: IClassifierProvider
    {
        // --- We keep track of active classifiers
        private static Dictionary<string, ZxBasicClassifier> s_ActiveClassifiers =
            new Dictionary<string, ZxBasicClassifier>();

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        [Import]
        private IClassificationTypeRegistryService ClassificationRegistry { get; set; }

        /// <summary>
        /// Sites this classification provider with the package
        /// </summary>
        public static void AttachToPackage()
        {
            SpectNetPackage.Default.ApplicationObject.Events.WindowEvents.WindowClosing += OnWindowClosing;
            SpectNetPackage.Default.BreakpointChangeWatcher.BreakpointsChanged += OnBreakpointsChanged; ;
        }

        /// <summary>
        /// Detaches this classification provider frod the package
        /// </summary>
        public static void DetachFromPackage()
        {
            try
            {
                SpectNetPackage.Default.ApplicationObject.Events.WindowEvents.WindowClosing -= OnWindowClosing; ;
                SpectNetPackage.Default.BreakpointChangeWatcher.BreakpointsChanged -= OnBreakpointsChanged; ;
            }
            catch
            {
                // --- This exception is intentionally ignored
            }
        }

        /// <summary>
        /// Gets th classifier object for the specified text buffer
        /// </summary>
        /// <param name="buffer">Text buffer to get the specifier for</param>
        /// <returns>Classifier object</returns>
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            var classifier = new ZxBasicClassifier(buffer, ClassificationRegistry);
            s_ActiveClassifiers[classifier.GetFilePath()] = classifier;
            return buffer.Properties.GetOrCreateSingletonProperty(()
                => classifier);
        }

        /// <summary>
        /// Refreshes all active classifiers
        /// </summary>
        public static void RefreshAllActive()
        {
            foreach (var classifier in s_ActiveClassifiers.Values)
            {
                classifier.Refresh();
            }
        }

        /// <summary>
        /// Refreshes the specified line
        /// </summary>
        /// <param name="doc">Document file name</param>
        /// <param name="lineNo">Lien number to refresh</param>
        public static void RefreshLine(string doc, int lineNo)
        {
            if (!s_ActiveClassifiers.TryGetValue(doc, out var classifier))
            {
                return;
            }
            classifier.RefreshLine(lineNo);
        }

        /// <summary>
        /// Remove the matching classifier from the document list
        /// </summary>
        private static void OnWindowClosing(EnvDTE.Window Window)
        {
            ScanActiveClassifierWindows();
        }

        /// <summary>
        /// Scans the active classification windows
        /// </summary>
        private static void ScanActiveClassifierWindows()
        {
            var windows = SpectNetPackage.Default.ApplicationObject.Windows;
            var activeClassifiers = new Dictionary<string, ZxBasicClassifier>();

            // --- Collect active document names
            foreach (EnvDTE.Window window in windows)
            {
                var docName = window.Document?.FullName;
                if (docName != null && s_ActiveClassifiers.TryGetValue(docName, out var classifier))
                {
                    activeClassifiers.Add(docName, classifier);
                }
            }
            s_ActiveClassifiers = activeClassifiers;
        }

        /// <summary>
        /// Breakpoints changed, so refresh the views
        /// </summary>
        private static Task OnBreakpointsChanged(object sender, BreakpointsChangedEventArgs args)
        {
            RefreshAllActive();
            return Task.FromResult(0);
        }
    }
}
