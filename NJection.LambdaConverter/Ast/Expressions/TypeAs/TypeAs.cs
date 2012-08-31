using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class TypeAs : AstExpression
    {
        private NRefactory.AsExpression _asExpression = null;

        protected internal TypeAs(NRefactory.AsExpression asExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _asExpression = asExpression;
            Expression = asExpression.Expression.AcceptVisitor(Visitor, ParentScope);
            InternalType = asExpression.Type.AcceptVisitor(Visitor, ParentScope).Type;
        }

        public Expression Expression { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.TypeAs; }
        }

        public override Expression Reduce() {
            return Expression.TypeAs(Expression, Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitTypeAsExpression(this);
        }

        public Expression Update(Expression expression, Type type) {
            if (Expression.Equals(expression) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.TypeAs(_asExpression, ParentScope, Visitor);
        }
    }
}