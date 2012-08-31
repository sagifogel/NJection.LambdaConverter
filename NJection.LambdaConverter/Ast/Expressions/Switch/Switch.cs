using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Switch : AstExpression
    {
        private NRefactory.SwitchStatement _switchStatement = null;

        protected internal Switch(NRefactory.SwitchStatement switchStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _switchStatement = switchStatement;
            SwitchValue = switchStatement.Expression.AcceptVisitor(Visitor, scope);
            BuildSwitchCases();
            InternalType = TypeSystem.Void;
        }

        public string Name { get; set; }

        public Expression SwitchValue { get; private set; }

        public Expression DefaultBody { get; private set; }

        public IEnumerable<SwitchCase> Cases { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Switch; }
        }

        public override Expression Reduce() {
            SwitchCase[] cases = Cases.ToArray();

            if (DefaultBody != null) {
                return Expression.Switch(SwitchValue, DefaultBody, cases);
            }

            return Expression.Switch(SwitchValue, cases);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitSwitchStatement(this);
        }

        public Expression Update(Expression switchValue, IEnumerable<SwitchCase> cases, Expression defaultBody) {
            if (SwitchValue.Equals(switchValue) && Cases.Equals(cases) && ReferenceEquals(DefaultBody, defaultBody)) {
                return this;
            }

            return AstExpression.Switch(_switchStatement, ParentScope, Visitor);
        }

        private void BuildSwitchCases() {
            Expression body = null;

            Cases = _switchStatement.SwitchSections.Select(section => {
                var testValues = section.CaseLabels.Select(s => s.AcceptVisitor(Visitor, ParentScope));
                body = section.Statements.First().AcceptVisitor(Visitor, ParentScope);

                return Expression.SwitchCase(body, testValues);
            }).ToList();
        }
    }
}