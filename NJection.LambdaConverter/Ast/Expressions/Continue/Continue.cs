using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Continue : AstExpression
    {
        private NRefactory.ContinueStatement _continueStatement = null;

        protected internal Continue(NRefactory.ContinueStatement continueStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _continueStatement = continueStatement;
            InternalType = TypeSystem.Void;
            LabelTarget = Expression.Label();
        }

        public LabelTarget LabelTarget { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Continue; }
        }

        public override Expression Reduce() {
            return Expression.Continue(LabelTarget);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitContinue(this);
        }

        public Expression Update(LabelTarget label, Type type) {
            if (LabelTarget.Equals(label) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.Continue(_continueStatement, this as IScope, Visitor);
        }
    }
}