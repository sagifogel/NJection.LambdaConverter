using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression : Expression, IScopeChild
    {
        protected internal AstExpression(IScope scope, INRefcatoryExpressionVisitor visitor) {
            Visitor = visitor;

            if (scope != null) {
                ParentScope = scope;
                RootScope = scope.RootScope;
            }
        }

        protected Type InternalType { get; set; }

        public IScope ParentScope { get; protected set; }

        public IMethodScope RootScope { get; protected set; }

        public override ExpressionType NodeType {
            get { return ExpressionType.Extension; }
        }

        public override bool CanReduce {
            get { return true; }
        }

        public override Type Type {
            get { return InternalType; }
        }

        protected INRefcatoryExpressionVisitor Visitor { get; set; }

        #region Abstract

        public abstract AstExpressionType AstNodeType { get; }

        public abstract Expression Accept(NJectionExpressionVisitor visitor);

        #endregion Abstract
    }
}