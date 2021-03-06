using System.Collections.Generic;
using Spect.Net.TestParser.Generated;

namespace Spect.Net.TestParser.SyntaxTree.TestSet
{
    /// <summary>
    /// This class represents a test block of the language
    /// </summary>
    public class TestBlockNode : NodeBase
    {
        /// <summary>
        /// Creates a clause with the span defined by the passed context
        /// </summary>
        /// <param name="context">Parser rule context</param>
        public TestBlockNode(Z80TestParser.TestBlockContext context) : base(context)
        {
            TestKeywordSpan = new TextSpan(context.TEST());
            if (context.IDENTIFIER() != null && context.IDENTIFIER().Length > 0)
            {
                TestIdSpan = new TextSpan(context.IDENTIFIER()[0]);
                TestId = context.IDENTIFIER()[0].GetText();
            }
            if (context.CATEGORY() != null)
            {
                CategoryKeywordSpan = new TextSpan(context.CATEGORY());
                if (context.IDENTIFIER().Length > 1)
                {
                    CategoryIdSpan = new TextSpan(context.IDENTIFIER()?[1]);
                    Category = context.IDENTIFIER()?[1].GetText();
                }
            }
            Cases = new List<TestCaseNode>();
        }

        /// <summary>
        /// The 'test' keyword span
        /// </summary>
        public TextSpan TestKeywordSpan { get; set; }

        /// <summary>
        /// The ID of the test
        /// </summary>
        public string TestId { get; set; }

        /// <summary>
        /// The span of the test ID
        /// </summary>
        public TextSpan TestIdSpan { get; set; }

        /// <summary>
        /// The 'test' keyword span
        /// </summary>
        public TextSpan? CategoryKeywordSpan { get; set; }

        /// <summary>
        /// The category of the test
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The category ID span
        /// </summary>
        public TextSpan? CategoryIdSpan { get; set; }

        /// <summary>
        /// The test options clause
        /// </summary>
        public TestOptionsNode TestOptions { get; set; }

        /// <summary>
        /// The setup clause
        /// </summary>
        public InvokeCodeNode Setup { get; set; }

        /// <summary>
        /// The list of test parameters
        /// </summary>
        public ParamsNode Params { get; set; }

        /// <summary>
        /// The list of test cases
        /// </summary>
        public List<TestCaseNode> Cases { get; }

        /// <summary>
        /// The list of arrange assignments
        /// </summary>
        public AssignmentsNode Arrange { get; set; }

        /// <summary>
        /// The act clause
        /// </summary>
        public InvokeCodeNode Act { get; set; }

        /// <summary>
        /// The breakpoints clause
        /// </summary>
        public BreakpointsNode Breakpoints { get; set; }

        /// <summary>
        /// The list of assert conditions
        /// </summary>
        public AssertNode Assert { get; set; }

        /// <summary>
        /// The cleanup clause
        /// </summary>
        public InvokeCodeNode Cleanup { get; set; }
    }
}