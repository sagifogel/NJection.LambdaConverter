using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Direction : AstExpression
    {
        private ParameterExpression _outParameter = null;
        private NRefactory.DirectionExpression _directionExpression = null;

        protected internal Direction(NRefactory.DirectionExpression directionExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _directionExpression = directionExpression;
            IdentifierParameter = directionExpression.Expression.AcceptVisitor(Visitor, scope) as Identifier;
            _outParameter = ParentScope.Find(IdentifierParameter.Name);
            InternalType = IdentifierParameter.Type;
        }

        public ParameterExpression OutParameter {
            get { return _outParameter; }
        }

        public Identifier IdentifierParameter { get; set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Direction; }
        }

        public override Expression Reduce() {
            return OutParameter;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitDirection(this);
        }

        public Expression Update(Identifier Expression, ParameterExpression directionExpression) {
            if (IdentifierParameter.Equals(Expression) && OutParameter.Equals(directionExpression)) {
                return this;
            }

            return AstExpression.Direction(_directionExpression, ParentScope, Visitor);
        }
    }
}