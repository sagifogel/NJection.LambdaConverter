using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class ForEach : Scope
    {
        private NRefactory.ForeachStatement _foreachStatement = null;

        protected internal ForEach(NRefactory.ForeachStatement foreachStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _foreachStatement = foreachStatement;
        }

        public Expression Body { get; private set; }

        public LabelExpression ContinueExpression { get; private set; }

        public LabelExpression BreakExpression { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.ForEach; }
        }

        public override Expression Reduce() {
            return null;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitForEach(this);
        }

        public Expression Update(Expression body, LabelExpression @break, LabelExpression @continue) {
            if (Body.Equals(body) && BreakExpression == @break && ContinueExpression == @continue) {
                return this;
            }

            return AstExpression.ForEach(_foreachStatement, this, Visitor);
        }
    }
}