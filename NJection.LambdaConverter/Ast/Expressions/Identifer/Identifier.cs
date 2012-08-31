using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Identifier : AstExpression
    {
        private NRefactory.IdentifierExpression _identifierExpression = null;

        protected internal Identifier(NRefactory.IdentifierExpression identifierExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _identifierExpression = identifierExpression;
            Expression = scope.Find(_identifierExpression.Identifier);
            InternalType = Expression.Type;
            Name = _identifierExpression.Identifier;
        }

        public ParameterExpression Expression { get; private set; }

        public string Name { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Identifer; }
        }

        public override Expression Reduce() {
            return Expression;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitIdentifier(this);
        }

        public Expression Update(Expression expression) {
            if (Expression.Equals(expression)) {
                return this;
            }

            return AstExpression.Identifer(_identifierExpression, ParentScope, Visitor);
        }
    }
}