using System;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using NJection.LambdaConverter.Expressions;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.TypeResolvers;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Fluent
{
    internal class DelegateExpressionizer<TDelegate> : Expressionizer<TDelegate>
        where TDelegate : class
    {
        private MethodInfo _methodInfo = null;

        protected override void ResolveMethod() {
            var @delegate = MethodResolver() as Delegate;
            _methodInfo = @delegate.Method;
            DeclaringType = _methodInfo.DeclaringType;
        }

        public override Expression<TDelegate> ToLambda() {
            Method method = null;
            ICecilArgumentsResolver typeResolver = null;
            NRefactory.MethodDeclaration methodDeclaration = null;
			var genericTypesTransformerVisitor = new GenericTypesTransformerVisitor();
			var lambdaExpressionVisitor = new LambdaExpressionVisitor<NRefactory.MethodDeclaration>();
            MethodDefinition methodDefinition = _methodInfo.ResolveMethodDefinition();

            if (methodDefinition.HasGenericParameters) {
                var types = _methodInfo.GetGenericArguments();
                typeResolver = new MethodDefinitionTypesResolver(methodDefinition, types);
            }
            else {
                typeResolver = DummyArgumentsResolver.Instance;
            }

            methodDeclaration = methodDefinition.ResolveMothod<NRefactory.MethodDeclaration>();
			methodDeclaration = methodDeclaration.AcceptVisitor(typeResolver, genericTypesTransformerVisitor);
            methodDeclaration = methodDeclaration.AcceptVisitor(lambdaExpressionVisitor);
            method = AstExpression.Method(methodDeclaration, Context, new NRefactoryAstVisitor());

            return Expression.Lambda<TDelegate>(method, method.Parameters);
        }
    }
}