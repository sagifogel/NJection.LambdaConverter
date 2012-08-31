using System.Linq.Expressions;
using System.Reflection;
using NJection.LambdaConverter.Visitors;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Event Event(Expression target, EventInfo eventInfo, ExpressionType @operator, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Event(target, eventInfo, @operator, scope, visitor);
        }
    }
}