using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class MethodOf : AstExpression
    {
        private NRefactory.InvocationExpression _invocationExpression = null;

        protected internal MethodOf(NRefactory.InvocationExpression invocationExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            var memberReference = invocationExpression.Arguments.First() as NRefactory.MemberReferenceExpression;
            var loadMethodTokenInvocation = memberReference.Target as NRefactory.InvocationExpression;
            var methodReference = loadMethodTokenInvocation.Arguments.First().Annotation<MethodReference>();

            _invocationExpression = invocationExpression;
            Member = methodReference.GetActualMethod<MethodInfo>();
            InternalType = typeof(RuntimeMethodHandle);
        }

        public MethodInfo Member { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.MethodOf; }
        }

        public override Expression Reduce() {
            return Expression.Constant(Member.MethodHandle);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitMethodOf(this);
        }

        public Expression Update(MethodInfo method, Type type) {
            if (Member.Equals(method) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.MethodOf(_invocationExpression, ParentScope, Visitor);
        }
    }
}