using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Empty : AstExpression
    {
        private NRefactory.EmptyExpression _emptyExpression = null;

        protected internal Empty(NRefactory.EmptyExpression emptyExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _emptyExpression = emptyExpression;
            InternalType = TypeSystem.Void;
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Empty; }
        }

        public override Expression Reduce() {
            return Expression.Empty();
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitEmpty(this);
        }

        public Expression Update() {
            return this;
        }
    }
}