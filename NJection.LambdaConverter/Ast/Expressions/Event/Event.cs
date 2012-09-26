using System;
using System.Linq.Expressions;
using System.Reflection;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;

namespace NJection.LambdaConverter.Expressions
{
    public class Event : AstExpression
    {
        private EventInfo _eventInfo = null;

        protected internal Event(Expression target, EventInfo eventInfo, ExpressionType @operator, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
           
            _eventInfo = eventInfo;
            Operator = @operator;
            Target = target;
            InternalType = Expression.GetActionType(_eventInfo.EventHandlerType);
        }

        public Expression Target { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Event; }
        }

        public ExpressionType Operator { get; private set; }

        public override Expression Reduce() {
            var thisExpression = Target as This;
            var @this = thisExpression != null ? thisExpression.Context.Value : thisExpression;
            var method = Operator == ExpressionType.Add ? _eventInfo.GetAddMethod(true) : _eventInfo.GetRemoveMethod(true);
            var @delegate = method.CreateDelegate(@this: @this);

            return Expression.Constant(@delegate);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitEvent(this);
        }

        public Expression Update(Expression target, Type type, ExpressionType @operator) {
            if (Type.Equals(type) && ReferenceEquals(Target, target) && Operator == @operator) {
                return this;
            }

            return AstExpression.Event(Target, _eventInfo, Operator, ParentScope, Visitor);
        }
    }
}