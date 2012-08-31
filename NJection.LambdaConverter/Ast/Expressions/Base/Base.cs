using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Base : AstExpression
    {
        private NRefactory.BaseReferenceExpression _baseReferenceExpression = null;

        protected internal Base(NRefactory.BaseReferenceExpression baseReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            var memberReference = baseReferenceExpression.Parent.Annotation<Mono.Cecil.MemberReference>();

            _baseReferenceExpression = baseReferenceExpression;
            Context = RootScope.Context.Expression;
            InternalType = memberReference != null ? memberReference.DeclaringType.GetActualType() : Context.Type;
        }

        public Expression Context { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Base; }
        }

        public override Expression Reduce() {
            return Context;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitBase(this);
        }

        public Expression Update(Expression context, Type type) {
            if (Context.Equals(context) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.Base(_baseReferenceExpression, ParentScope, Visitor);
        }
    }
}