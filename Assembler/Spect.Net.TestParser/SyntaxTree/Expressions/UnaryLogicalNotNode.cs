using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents an UNARY logical NOT operation
    /// </summary>
    public sealed class UnaryLogicalNotNode : UnaryExpressionNode
    {
        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <param name="checkOnly"></param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IExpressionEvaluationContext evalContext, bool checkOnly = false)
        {
            // --- Check operand error
            var operandValue = Operand.Evaluate(evalContext);
            if (operandValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = Operand.EvaluationError;
                return ExpressionValue.Error;
            }

            if (checkOnly) return ExpressionValue.NonEvaluated;

            // --- Carry out operation
            switch (operandValue.Type)
            {
                case ExpressionValueType.ByteArray:
                case ExpressionValueType.Bool:
                case ExpressionValueType.Number:
                    return new ExpressionValue(!operandValue.AsBool());
                default:
                    return ExpressionValue.Error;
            }
        }

        public UnaryLogicalNotNode(ParserRuleContext context) : base(context)
        {
        }
    }
}