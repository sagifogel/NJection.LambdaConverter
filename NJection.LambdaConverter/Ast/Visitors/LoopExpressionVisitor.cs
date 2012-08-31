using System.Linq.Expressions;

namespace NJection.LambdaConverter.Visitors
{
    internal class LoopExpressionVisitor : ExpressionVisitor
    {
        private LabelTarget _break = null;
        private LabelTarget _continue = null;

        internal LoopExpressionVisitor(LabelTarget @break, LabelTarget @continue) {
            _break = @break;
            _continue = @continue;
        }

        protected override Expression VisitGoto(GotoExpression node) {
            if (node.Kind == GotoExpressionKind.Break) {
                if (node.Value != null) {
                    return Expression.Goto(_break, node.Value, node.Type);
                }

                return Expression.Goto(_break, node.Type);
            }
            else if (node.Kind == GotoExpressionKind.Continue) {
                return Expression.Continue(_continue, _continue.Type);
            }

            return node;
        }
    }
}