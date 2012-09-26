using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Label : AstExpression
    {
        private NRefactory.LabelStatement _labelStatement = null;

        protected internal Label(NRefactory.LabelStatement labelStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _labelStatement = labelStatement;
            InternalType = TypeSystem.Void;
            LabelTarget = RootScope.RegisterLabel(InternalType, labelStatement.Label);
        }

        public string Name { get; set; }

        public LabelTarget LabelTarget { get; set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Label; }
        }

        public override Expression Reduce() {
            return Expression.Label(LabelTarget);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitLabel(this);
        }

        public Expression Update(LabelTarget labelTarget, Type type) {
            if (LabelTarget.Equals(labelTarget) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.Label(_labelStatement, ParentScope, Visitor);
        }
    }
}