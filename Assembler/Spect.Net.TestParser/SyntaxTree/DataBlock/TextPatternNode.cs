using Antlr4.Runtime.Tree;
using Spect.Net.TestParser.Generated;

namespace Spect.Net.TestParser.SyntaxTree.DataBlock
{
    /// <summary>
    /// Represents a text pattern
    /// </summary>
    public class TextPatternNode : MemoryPatternNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public TextPatternNode(Z80TestParser.TextContext context) : base(context)
        {
            if (context.TEXT() != null)
            {
                TextKeywordSpan = new TextSpan(context.TEXT());
            }
            StringSpan = new TextSpan(context.STRING());
            String = context.STRING()?.GetText().Unquote();
        }

        /// <summary>
        /// The 'text' span
        /// </summary>
        public TextSpan TextKeywordSpan { get; }

        /// <summary>
        /// The span of the string
        /// </summary>
        public TextSpan StringSpan { get; }

        /// <summary>
        /// The string itself
        /// </summary>
        public string String { get; }
    }
}