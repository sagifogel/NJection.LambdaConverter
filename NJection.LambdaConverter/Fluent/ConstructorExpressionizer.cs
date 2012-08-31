using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using NJection.LambdaConverter.DynamicProxies;
using NJection.LambdaConverter.Expressions;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.TypeResolvers;
using NJection.LambdaConverter.Visitors;
using Cil = Mono.Cecil.Cil;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Fluent
{
    internal class ConstructorExpressionizer<TDelegate> : Expressionizer<TDelegate>
        where TDelegate : class
    {
        private ConstructorInfo _ctor = null;

        protected override void ResolveMethod() {
            var @delegate = MethodResolver() as Delegate;
            var method = @delegate.Method;
            
            var types = method.GetParameters()
                              .Skip(1)
                              .Select(p => p.ParameterType)
                              .ToArray();

            DeclaringType = method.ReturnType;
            _ctor = DeclaringType.GetConstructor(types);
        }

        public override Expression<TDelegate> ToLambda() {
            DynamicProxy proxy = GetDynamicProxy();
            ConstructorDeclaration constructor = null;
            IEnumerable<Parameter> outParameters = null;
            var typeResolver = DummyArgumentsResolver.Instance;
            IEnumerable<ParameterExpression> parameters = null;
            var visitor = new GenericTypesTransformerVisitor();
            NRefactory.ConstructorDeclaration constructorDeclaration = null;
            MethodDefinition methodDefinition = _ctor.ResolveConstructorDefinition();
            var lambdaExpressionVisitor = new LambdaExpressionVisitor<NRefactory.ConstructorDeclaration>();

            constructorDeclaration = methodDefinition.ResolveMothod<NRefactory.ConstructorDeclaration>();
            constructorDeclaration = constructorDeclaration.AcceptVisitor(visitor, typeResolver) as NRefactory.ConstructorDeclaration;
            constructorDeclaration.AcceptVisitor(new ThisAdditionalReturnStatmentVisitor(), null);
            outParameters = proxy.GetOutParameters(constructorDeclaration);
            constructor = AstExpression.Constructor(constructorDeclaration, proxy.ProxiedType, outParameters, new NRefactoryAstVisitor());
            constructorDeclaration = constructorDeclaration.AcceptVisitor(lambdaExpressionVisitor);
            parameters = constructor.Parameters.Concat(constructor.ContextParameter);

            return Expression.Lambda<TDelegate>(
                        Expression.Block(new[] { constructor.ContextParameter },
                            constructor),
                                constructor.Parameters);
        }

        public override IContextProvider<TDelegate> WithContextOf<TContext>(TContext context) {
            throw new InvalidOperationException("Constructor can not have a context");
        }

        private DynamicProxy GetDynamicProxy() {
            DynamicProxy proxy = DynamicProxyRepository.Get(DeclaringType);

            if (proxy == null) {
                if (IsProxyShouldBeCreated()) {
                    proxy = DynamicProxyRepository.GetOrAdd(DeclaringType);
                }
                else {
                    proxy = DynamicProxyRepository.Cache(DeclaringType);
                }
            }

            return proxy;
        }

        private bool IsProxyShouldBeCreated() {
            var defaultConstructor = DeclaringType.GetConstructor(Type.EmptyTypes);

            if (defaultConstructor != null) {
                MethodDefinition methodDefinition = defaultConstructor.ResolveConstructorDefinition();

                return ConstructorHasStatement(methodDefinition);
            }

            return true;
        }

        private bool ConstructorHasStatement(MethodDefinition methodDefinition) {
            Cil.Instruction instruction;
            var instructions = methodDefinition.Body.Instructions;

            var instructionIndex = instructions.IndexOf(i => {
                instruction = i;

                if (instruction.OpCode == Cil.OpCodes.Call) {
                    var methodReference = instruction.Operand as MethodReference;
                    Type methodDeclaringType = methodReference.DeclaringType.GetActualType();

                    return methodDeclaringType.Equals(DeclaringType.BaseType);
                }

                return false;
            });

            instruction = instructions[instructionIndex + 1];

            do {
                if (instruction.OpCode != Cil.OpCodes.Nop) {
                    return true;
                }

                instructionIndex++;
                instruction = instructions[instructionIndex];
            }
            while (instruction.OpCode != Cil.OpCodes.Ret);

            return false;
        }
    }
}