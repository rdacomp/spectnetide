using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents a conditional (?:) operation
    /// </summary>
    public class ConditionalExpressionNode : ExpressionNode
    {
        /// <summary>
        /// Condition of the expression
        /// </summary>
        public ExpressionNode Condition { get; set; }

        /// <summary>
        /// Value when the expression is true
        /// </summary>
        public ExpressionNode TrueExpression { get; set; }

        /// <summary>
        /// Value when the expression is false
        /// </summary>
        public ExpressionNode FalseExpression { get; set; }

        /// <summary>
        /// This property signs if an expression is ready to be evaluated,
        /// namely, all subexpression values are known
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>True, if the expression is ready; otherwise, false</returns>
        public override bool ReadyToEvaluate(IExpressionEvaluationContext evalContext)
            => Condition.ReadyToEvaluate(evalContext)
                && TrueExpression.ReadyToEvaluate(evalContext)
                && FalseExpression.ReadyToEvaluate(evalContext);

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <param name="checkOnly"></param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext, bool checkOnly = false)
        {
            // --- Check for condition expression errors
            var cond = Condition.Evaluate(evalContext);
            if (Condition.EvaluationError != null)
            {
                EvaluationError = Condition.EvaluationError;
                return ExpressionValue.Error;
            }

            if (cond.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "A byte array cannot be used as a condition";
                return ExpressionValue.Error;
            }

            if (checkOnly) return ExpressionValue.NonEvaluated;

            if (cond.AsBool())
            {
                // --- Condition is true
                var trueValue = TrueExpression.Evaluate(evalContext);
                if (TrueExpression.EvaluationError == null)
                {
                    return trueValue;
                }
                EvaluationError = TrueExpression.EvaluationError;
                return ExpressionValue.Error;
            }

            // --- Condition is false
            var falseValue = FalseExpression.Evaluate(evalContext);
            if (FalseExpression.EvaluationError == null)
            {
                return falseValue;
            }
            EvaluationError = FalseExpression.EvaluationError;
            return ExpressionValue.Error;
        }

        public ConditionalExpressionNode(ParserRuleContext context) : base(context)
        {
        }
    }
}