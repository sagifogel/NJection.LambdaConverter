using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class NamedArgument : AstExpression
    {
        private NRefactory.NamedArgumentExpression _namedArgumentExpression = null;

        protected internal NamedArgument(NRefactory.NamedArgumentExpression namedArgumentExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _namedArgumentExpression = namedArgumentExpression;
            Argument = namedArgumentExpression.Expression.AcceptVisitor(Visitor, ParentScope);
            InternalType = Argument.Type;
        }

        public Expression Argument { get; set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.NamedArgument; }
        }

        public override Expression Reduce() {
            return Argument;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitNamedArgument(this);
        }

        public Expression Update(Expression argument) {
            if (Argument.Equals(argument)) {
                return this;
            }

            return AstExpression.NamedArgument(_namedArgumentExpression, ParentScope, Visitor);
        }
    }
}