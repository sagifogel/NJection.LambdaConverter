using System.Linq.Expressions;

namespace NJection.LambdaConverter.Visitors
{
    internal class ConvertCheckedVisitor : ExpressionVisitor
    {
        protected override Expression VisitUnary(UnaryExpression node) {
            if (node.NodeType == ExpressionType.Convert) {
                return Expression.ConvertChecked(node.Operand, node.Type);
            }

            return base.VisitUnary(node);
        }
    }
}