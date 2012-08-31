using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Case : AstExpression
    {
        private NRefactory.CaseLabel _caseLabel = null;

        protected internal Case(NRefactory.CaseLabel caseLabel, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _caseLabel = caseLabel;
            TestValue = caseLabel.Expression.AcceptVisitor(Visitor, scope);
            InternalType = TestValue.Type;
        }

        public string Name { get; set; }

        public Expression TestValue { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Case; }
        }

        public override Expression Reduce() {
            return TestValue;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitSwitchCase(this);
        }

        public Expression Update(Expression testValue) {
            if (TestValue.Equals(testValue)) {
                return this;
            }

            return AstExpression.SwitchCase(_caseLabel, ParentScope, Visitor);
        }
    }
}