using Antlr4.Runtime;

namespace Spect.Net.TestParser.SyntaxTree.Expressions
{
    /// <summary>
    /// This class represents the shift right? operation
    /// </summary>
    public sealed class ShiftRightOperationNode : BinaryOperationNode
    {
        /// <summary>
        /// Calculates the result of the binary operation.
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Result of the operation</returns>
        public override ExpressionValue Calculate(IExpressionEvaluationContext evalContext)
        {
            // --- Check operands for errors
            var leftValue = LeftOperand.Evaluate(evalContext);
            if (leftValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = LeftOperand.EvaluationError;
                return ExpressionValue.Error;
            }
            var rightValue = RightOperand.Evaluate(evalContext);
            if (rightValue.Type == ExpressionValueType.Error)
            {
                EvaluationError = RightOperand.EvaluationError;
                return ExpressionValue.Error;
            }

            // --- Test for incompatible types
            if (leftValue.Type == ExpressionValueType.ByteArray || rightValue.Type == ExpressionValueType.ByteArray)
            {
                EvaluationError = "'>>' operator cannot be applied on a byte array";
                return ExpressionValue.Error;
            }

            // --- Numeric operands
            return new ExpressionValue(leftValue.AsNumber() >> (ushort)rightValue.AsNumber());
        }

        public ShiftRightOperationNode(ParserRuleContext context) : base(context)
        {
        }
    }
}