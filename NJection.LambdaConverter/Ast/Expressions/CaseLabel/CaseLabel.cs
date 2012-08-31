using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class CaseLabel : AstExpression
    {
        private NRefactory.CaseLabel _caseLabel = null;

        protected internal CaseLabel(NRefactory.CaseLabel caseLabel, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            var expression = caseLabel.Expression;
            _caseLabel = caseLabel;
            DefaultValue = expression.AcceptVisitor(Visitor, ParentScope);
            InternalType = DefaultValue.Type;
        }

        public LabelTarget Target { get; private set; }

        public Expression DefaultValue { get; private set; }

        public string Name { get; set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.CaseLabel; }
        }

        public override Expression Reduce() {
            return Expression.Label(
                        Expression.Label(InternalType));
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitCaseLabel(this);
        }

        public Expression Update(LabelTarget target, Expression defaultValue) {
            if (Target.Equals(target) && DefaultValue == defaultValue) {
                return this;
            }

            return AstExpression.Label(_caseLabel, ParentScope, Visitor);
        }
    }
}