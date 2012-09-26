using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Goto : AstExpression
    {
        private NRefactory.GotoStatement _gotoStatement = null;

        protected internal Goto(NRefactory.GotoStatement gotoStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _gotoStatement = gotoStatement;
            InternalType = TypeSystem.Void;
            LabelTarget = RootScope.RegisterLabel(InternalType, _gotoStatement.Label);
        }

        public Expression Value { get; private set; }

        public LabelTarget LabelTarget { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Goto; }
        }

        public override Expression Reduce() {
            return Expression.Goto(LabelTarget);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitGoto(this);
        }

        public Expression Update(LabelTarget labelTarget, Type type) {
            if (LabelTarget.Equals(labelTarget) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.Goto(_gotoStatement, this as IScope, Visitor);
        }
    }
}