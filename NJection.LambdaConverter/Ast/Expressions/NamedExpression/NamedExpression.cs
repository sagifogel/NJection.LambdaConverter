using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class NamedExpression : AstExpression
    {
        private NRefactory.NamedExpression _namedExpression = null;

        protected internal NamedExpression(NRefactory.NamedExpression namedExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _namedExpression = namedExpression;
            Expression = _namedExpression.Expression.AcceptVisitor(Visitor, ParentScope);
            InternalType = Expression.Type;
        }

        public Expression Expression { get; set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.NamedExpression; }
        }

        public override Expression Reduce() {
            return Expression;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitNamedExpression(this);
        }

        public Expression Update(Expression argument) {
            if (Expression.Equals(argument)) {
                return this;
            }

            return AstExpression.NamedExpression(_namedExpression, ParentScope, Visitor);
        }
    }
}