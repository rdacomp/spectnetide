using Spect.Net.Assembler.Generated;

namespace Spect.Net.Assembler.SyntaxTree.Expressions
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
        public override bool ReadyToEvaluate(IEvaluationContext evalContext)
            => Condition.ReadyToEvaluate(evalContext)
                && TrueExpression.ReadyToEvaluate(evalContext)
                && FalseExpression.ReadyToEvaluate(evalContext);

        /// <summary>
        /// Retrieves the value of the expression
        /// </summary>
        /// <param name="evalContext">Evaluation context</param>
        /// <returns>Evaluated expression value</returns>
        public override ExpressionValue Evaluate(IEvaluationContext evalContext)
        {
            var cond = Condition.Evaluate(evalContext);
            if (Condition.EvaluationError != null)
            {
                EvaluationError = Condition.EvaluationError;
                return ExpressionValue.Error;
            }
            if (cond.AsBool())
            {
                var trueValue = TrueExpression.Evaluate(evalContext);
                if (TrueExpression.EvaluationError == null)
                {
                    return trueValue;
                }
                EvaluationError = TrueExpression.EvaluationError;
                return ExpressionValue.Error;
            }

            var falseValue = FalseExpression.Evaluate(evalContext);
            if (FalseExpression.EvaluationError == null)
            {
                return falseValue;
            }
            EvaluationError = FalseExpression.EvaluationError;
            return ExpressionValue.Error;
        }

        /// <summary>
        /// Indicates if this expression has a macro parameter
        /// </summary>
        public override bool HasMacroParameter
            => (Condition?.HasMacroParameter ?? false)
               || (TrueExpression?.HasMacroParameter ?? false)
               || (FalseExpression?.HasMacroParameter ?? false);

        public ConditionalExpressionNode(Z80AsmParser.TernaryExprContext context, Z80AsmVisitor visitor)
            : base(context)
        {
            var exprs = context.expr();
            if (exprs.Length > 0)
            {
                Condition = (ExpressionNode)visitor.VisitExpr(exprs[0]);
            }
            if (exprs.Length > 1)
            {
                TrueExpression = (ExpressionNode)visitor.VisitExpr(exprs[1]);
            }
            if (exprs.Length > 2)
            {
                FalseExpression = (ExpressionNode)visitor.VisitExpr(exprs[2]);
            }
        }
    }
}