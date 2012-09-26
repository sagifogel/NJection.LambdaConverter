using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class CatchClause : Scope
    {
        private NRefactory.CatchClause _catchClause = null;

        protected internal CatchClause(NRefactory.CatchClause catchClause, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _catchClause = catchClause;
            BuildCatchBlock();
            InternalType = Body.Type;
        }

        public Expression Body { get; private set; }

        public Expression Filter { get; private set; }

        public ParameterExpression ExceptionVariable { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.CatchCluase; }
        }

        public override Expression Reduce() {
            return Body;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitCatchClause(this);
        }

        public Expression Update(Expression body, ParameterExpression variable, Expression filter) {
            if (Body.Equals(body) && ReferenceEquals(ExceptionVariable, variable) && ReferenceEquals(Filter, filter)) {
                return this;
            }

            return AstExpression.CatchClause(_catchClause, ParentScope, Visitor);
        }

        private void BuildCatchBlock() {
            Body = _catchClause.Body.AcceptVisitor(Visitor, this);

            if (_catchClause.Type != null) {
                ExceptionVariable = Find(_catchClause.VariableName);
            }
        }
    }
}