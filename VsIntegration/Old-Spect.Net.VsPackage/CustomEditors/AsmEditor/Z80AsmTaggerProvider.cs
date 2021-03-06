using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable 649

namespace Spect.Net.VsPackage.CustomEditors.AsmEditor
{
    /// <summary>
    /// Tagger provider for the Z80 Assembly editor
    /// </summary>
    [Export(typeof(ITaggerProvider))]
    [ContentType("z80Asm")]
    [TagType(typeof(Z80AsmTokenTag))]
    internal class Z80AsmTaggerProvider : ITaggerProvider
    {
        /// <summary>
        /// Creates a tag provider for the specified buffer.
        /// </summary>
        /// <typeparam name="T">The type of the tag.</typeparam>
        /// <param name="buffer">The <see cref="T:Microsoft.VisualStudio.Text.ITextBuffer" />.</param>
        /// <returns>The tagger we use to create Z80 assembly tags</returns>
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            string filePath = null;
            if (buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument docProperty))
            {
                filePath = docProperty.FilePath;
            }
            var tagger = new Z80AsmTokenTagger(buffer, filePath);
            return tagger as ITagger<T>;
        }
    }
}

#pragma warning restore 649
