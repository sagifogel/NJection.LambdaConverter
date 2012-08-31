using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Throw : AstExpression
    {
        private NRefactory.ThrowStatement _throwStatement = null;

        protected internal Throw(NRefactory.ThrowStatement throwStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _throwStatement = throwStatement;
            Value = throwStatement.Expression.AcceptVisitor(Visitor, ParentScope);
            InternalType = TypeSystem.Void;
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Throw; }
        }

        public Expression Value { get; private set; }

        public override Expression Reduce() {
            return Expression.Throw(Value);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitThrowStatement(this);
        }

        public Expression Update(Expression value, Type type) {
            if (Value.Equals(value) && Type == type) {
                return this;
            }

            return AstExpression.Throw(_throwStatement, ParentScope, Visitor);
        }
    }
}