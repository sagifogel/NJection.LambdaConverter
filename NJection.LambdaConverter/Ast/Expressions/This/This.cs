using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class This : AstExpression
    {
        private NRefactory.ThisReferenceExpression _thisReferenceExpression = null;

        protected internal This(NRefactory.ThisReferenceExpression thisReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _thisReferenceExpression = thisReferenceExpression;
            Context = RootScope.Context;
            InternalType = Context.Type;
        }

        internal IContext Context { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.This; }
        }

        public override Expression Reduce() {
            return Context.Expression;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitThis(this);
        }

        public Expression Update(Expression context, Type type) {
            if (Context.Equals(context) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.This(_thisReferenceExpression, ParentScope, Visitor);
        }
    }
}