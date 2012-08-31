using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Return : AstExpression
    {
        private NRefactory.ReturnStatement _returnStatement = null;
        private static readonly Type typeOfVoid = TypeSystem.Void;

        protected internal Return(NRefactory.ReturnStatement returnStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _returnStatement = returnStatement;
            Value = returnStatement.Expression.AcceptVisitor(Visitor, ParentScope);

            if (Value != null) {
                InternalType = Value.Type;
                Target = RootScope.BranchingRegistry.RegisterReturnStatementLabel(InternalType);
            }
        }

        public Expression Value { get; private set; }

        public LabelTarget Target { get; private set; }

        public GotoExpressionKind Kind { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Return; }
        }

        public override Expression Reduce() {
            if (Value == null) {
                return Expression.Return(Target, typeOfVoid);
            }

            return Expression.Return(Target, Value, Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitReturnExpression(this);
        }

        public Expression Update(GotoExpressionKind kind, LabelTarget target, Expression value) {
            if (Target.Equals(target) && ReferenceEquals(Value, value) && Kind == kind) {
                return this;
            }

            return AstExpression.Return(_returnStatement, ParentScope, Visitor);
        }
    }
}