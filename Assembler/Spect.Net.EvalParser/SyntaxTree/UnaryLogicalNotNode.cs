namespace Spect.Net.EvalParser.SyntaxTree
{
    /// <summary>
    /// This class represents an UNARY - operation
    /// </summary>
    public sealed class UnaryLogicalNotNode : UnaryExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext)
        {
            var operand = Operand.Evaluate(evalContext);
            SuggestType(ExpressionValueType.Bool);
            return new ExpressionValue((uint)(operand.Value == 0 ? 0 : 1));
        }

        /// <summary>
        /// Operator token
        /// </summary>
        public override string Operator => "!";
    }
}