using Spect.Net.TestParser.Generated;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// Represents a flag assignment
    /// </summary>
    public class FlagAssignmentNode : AssignmentNode
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public FlagAssignmentNode(Z80TestParser.FlagStatusContext context) : base(context)
        {
            FlagName = context.flag()?.GetText().Replace(".","").Replace(" ", "").ToLower();
        }

        /// <summary>
        /// The flag's name
        /// </summary>
        public string FlagName { get; }
    }
}