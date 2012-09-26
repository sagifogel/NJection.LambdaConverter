using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class NullReference : AstExpression
    {
        private NRefactory.NullReferenceExpression _nullReferenceExpression = null;
        private static readonly Expression _expression = Expression.Constant(null);

        protected internal NullReference(NRefactory.NullReferenceExpression nullReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _nullReferenceExpression = nullReferenceExpression;
            InternalType = _expression.Type;
        }

        public Expression Expression {
            get { return _expression; }
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.NullReference; }
        }

        public override Expression Reduce() {
            return Expression;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitNullReference(this);
        }

        public Expression Update(Expression expression) {
            if (Expression.Equals(expression)) {
                return this;
            }

            return AstExpression.NullReference(_nullReferenceExpression, ParentScope, Visitor);
        }
    }
}