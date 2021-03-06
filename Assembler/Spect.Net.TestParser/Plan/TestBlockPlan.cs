using System.Collections.Generic;
using Spect.Net.TestParser.SyntaxTree;
using Spect.Net.TestParser.SyntaxTree.Expressions;

namespace Spect.Net.TestParser.Plan
{
    /// <summary>
    /// Respresent a test block plan
    /// </summary>
    public class TestBlockPlan: IExpressionEvaluationContext
    {
        /// <summary>
        /// Machine context for evaluation
        /// </summary>
        public IMachineContext MachineContext { get; set; }

        /// <summary>
        /// Test block parameter names
        /// </summary>
        public List<string> ParameterNames { get; } = new List<string>();

        /// <summary>
        /// Parent test set
        /// </summary>
        public TestSetPlan TestSet { get; }

        /// <summary>
        /// ID of the test block
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The span of the test block
        /// </summary>
        public TextSpan Span { get; }

        /// <summary>
        /// ID of the test category
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Disable the interrupt when running test code
        /// </summary>
        public bool DisableInterrupt { get; set; } = false;

        /// <summary>
        /// Test timeout in milliseconds
        /// </summary>
        public int TimeoutValue { get; set; } = 100;

        /// <summary>
        /// Test case plans 
        /// </summary>
        public List<TestCasePlan> TestCases { get; } = new List<TestCasePlan>();

        /// <summary>
        /// The index of the currently running test case
        /// </summary>
        public int CurrentTestCaseIndex { get; set; }

        /// <summary>
        /// The Arrange assignments of this test block
        /// </summary>
        public List<RunTimeAssignmentPlanBase> ArrangeAssignments { get; } = new List<RunTimeAssignmentPlanBase>();

        /// <summary>
        /// Setup plan
        /// </summary>
        public InvokePlanBase Setup { get; set; }

        /// <summary>
        /// Act of the test block
        /// </summary>
        public InvokePlanBase Act { get; set; }

        /// <summary>
        /// List of breakpoints
        /// </summary>
        public List<ExpressionNode> Breakpoints { get; } = new List<ExpressionNode>();

        /// <summary>
        /// List of assertions
        /// </summary>
        public List<ExpressionNode> Assertions { get; } = new List<ExpressionNode>();

        /// <summary>
        /// Cleanup plan
        /// </summary>
        public InvokePlanBase Cleanup { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TestBlockPlan(TestSetPlan testSet, string id, string category,
            TextSpan span)
        {
            Id = id;
            Category = category;
            Span = span;
            TestSet = testSet;
            CurrentTestCaseIndex = -1;
        }

        /// <summary>
        /// Checks if this testblock contains the specified parameter
        /// </summary>
        /// <param name="param">Parameter name</param>
        /// <returns>Trui, if the test block contains the parameter; otherwise, false</returns>
        public bool ContainsParameter(string param) => ParameterNames.Contains(param.ToUpperInvariant());

        /// <summary>
        /// Adds the specified parameter to the test block parameters
        /// </summary>
        /// <param name="param"></param>
        public void AddParameter(string param) => ParameterNames.Add(param.ToUpperInvariant());

        /// <summary>
        /// Gets the value of the specified symbol
        /// </summary>
        /// <param name="symbol">Symbol name</param>
        /// <returns>
        /// Null, if the symbol cannot be found; otherwise, the symbol's value
        /// </returns>
        public ExpressionValue GetSymbolValue(string symbol)
        {
            if (MachineContext == null || MachineContext.IsCompileTimeContext)
            {
                // --- We return a fake value for compile time check, provided
                // --- we know the parameter name
                if (TestCases.Count > 0 && ParameterNames.Contains(symbol.ToUpperInvariant()))
                {
                    return ExpressionValue.NonEvaluated;
                }
            }

            // --- During run time we use the standard symbol name resolution
            return CurrentTestCaseIndex >= 1 && CurrentTestCaseIndex <= TestCases.Count
                ? TestCases[CurrentTestCaseIndex].GetSymbolValue(symbol)
                : TestSet.GetSymbolValue(symbol);
        }

        /// <summary>
        /// Gets the machine context to evaluate registers, flags, and memory
        /// </summary>
        /// <returns></returns>
        public IMachineContext GetMachineContext() => MachineContext;
    }
}