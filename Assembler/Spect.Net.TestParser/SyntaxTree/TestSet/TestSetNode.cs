using System.Collections.Generic;
using Spect.Net.TestParser.Generated;
using Spect.Net.TestParser.SyntaxTree.DataBlock;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    public class TestSetNode: NodeBase
    {
        /// <summary>
        /// Creates a 'testset' clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public TestSetNode(Z80TestParser.TestSetContext context) : base(context)
        {
            TestSetKeywordSpan = new TextSpan(context.TESTSET());
            if (context.IDENTIFIER() != null)
            {
                TestSetIdSpan = new TextSpan(context.IDENTIFIER());
                TestSetId = context.IDENTIFIER().GetText();
            }
            if (context.sp48Mode() != null)
            {
                Sp48ModeSpan = new TextSpan(context.sp48Mode());
                Sp48Mode = true;
            }
            TestBlocks = new List<TestBlockNode>();
        }

        /// <summary>
        /// The 'testset' keyword span
        /// </summary>
        public TextSpan TestSetKeywordSpan { get; set; }

        /// <summary>
        /// The ID of the test set
        /// </summary>
        public string TestSetId { get; set; }

        /// <summary>
        /// The span of the test set ID
        /// </summary>
        public TextSpan TestSetIdSpan { get; set; }

        /// <summary>
        /// The source contex clause
        /// </summary>
        public SourceContextNode SourceContext { get; set; }

        /// <summary>
        /// The text span of Sp48Mode
        /// </summary>
        public TextSpan Sp48ModeSpan { get; set; }
        
        /// <summary>
        /// Indicates if the test set runs in Spectrum 48K mode
        /// </summary>
        public bool Sp48Mode { get; set; }

        /// <summary>
        /// The call stub node
        /// </summary>
        public CallStubNode CallStub { get; set; }

        /// <summary>
        /// The data block clause
        /// </summary>
        public DataBlockNode DataBlock { get; set; }

        /// <summary>
        /// The init clause
        /// </summary>
        public AssignmentsNode Init { get; set; }

        /// <summary>
        /// The test block of this test set
        /// </summary>
        public List<TestBlockNode> TestBlocks { get; }
    }
}