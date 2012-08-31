using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Break : AstExpression
    {
        private NRefactory.BreakStatement _breakStatement = null;

        protected internal Break(NRefactory.BreakStatement breakStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _breakStatement = breakStatement;
            Target = Expression.Label();
            InternalType = TypeSystem.Void;
        }

        public Expression Value { get; private set; }

        public LabelTarget Target { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Break; }
        }

        public override Expression Reduce() {
            if (Value == null) {
                return Expression.Break(Target, Type);
            }

            return Expression.Break(Target, Value, Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitBreak(this);
        }

        public Expression Update(LabelTarget target, Type type, Expression value) {
            if (Target.Equals(target) && Type.Equals(type) && Value.Equals(value)) {
                return this;
            }

            return AstExpression.Break(_breakStatement, ParentScope, Visitor);
        }
    }
}