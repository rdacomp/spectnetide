namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents an UNARY + operation
    /// </summary>
    public sealed class UnaryPlusNode : UnaryExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            SuggestTypeOf(Operand);
            return Operand.Evaluate(evalContext);
        }

        /// <summary>
        /// Operator token
        /// </summary>
        public override string Operator => "+";
    }
}