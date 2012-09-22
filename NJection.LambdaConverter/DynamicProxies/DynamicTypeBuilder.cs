using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Mono.Collections.Generic;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using Cecil = Mono.Cecil;
using Cil = Mono.Cecil.Cil;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies
{
    internal static class DynamicTypeBuilder
    {
        private static TypeBuilder _current = null;
        private static CallingConventions _callingConventions = CallingConventions.Standard | CallingConventions.HasThis;
        private static TypeAttributes _typeAttributes = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable;

        internal static TypeBuilder Current {
            get { return _current; }
        }

        internal static DynamicProxy CreateProxy(this Type type) {
            ILGenerator ilGenerator = null;
            ConstructorInfo[] constructors = null;
            Type[] parameterTypes = Type.EmptyTypes;
            MappedInvocation mappedInvocation = null;
            List<Cil.Instruction> instructions = null;
            ConstructorBuilder constructorBuilder = null;
            List<MappedInvocation> mappedInvocations = null;
            Cecil.MethodDefinition baseCtorInvocation = null;
            Cecil.MethodDefinition constructorDefinition = null;
            NRefactory.ConstructorDeclaration resolvedCtor = null;
            string typeName = string.Format("{0}.{1}", DynamicAssemblyBuilder.AssemblyName, type.FullName);
            TypeBuilder typeBuilder = DynamicModuleBuilder.Instance.DefineType(typeName, _typeAttributes, type);

            Interlocked.Exchange<TypeBuilder>(ref _current, typeBuilder);

            if (type.IsGenericType) {
                MakeGenericType(type, typeBuilder);
            }

            constructors = type.GetConstructors();
            mappedInvocations = new List<MappedInvocation>(constructors.Length);

            foreach (var constructor in constructors) {
                parameterTypes = constructor.GetParameters()
                                            .Select(p => p.ParameterType)
                                            .ToArray();

                constructorDefinition = constructor.ResolveConstructorDefinition();
                resolvedCtor = constructorDefinition.ResolveMothod<NRefactory.ConstructorDeclaration>();
                instructions = FilterInstructions(constructorDefinition.Body.Instructions, type.BaseType);
                baseCtorInvocation = instructions[instructions.Count - 2].Operand as Cecil.MethodDefinition;
                mappedInvocation = new MappedInvocation(resolvedCtor, baseCtorInvocation);
                mappedInvocations.Add(mappedInvocation);
                constructorBuilder = typeBuilder.DefineConstructor(constructor.Attributes, _callingConventions, mappedInvocation.Parameters);
                ilGenerator = constructorBuilder.GetILGenerator();
                resolvedCtor = ResolveConstructor(resolvedCtor, type.BaseType, ilGenerator, instructions);
            }

            return new DynamicProxy(typeBuilder.CreateType(), type, mappedInvocations);
        }

        private static void MakeGenericType(Type baseType, TypeBuilder typeBuilder) {
            Type[] genericArguments = baseType.GetGenericArguments();
            string[] genericArgumentNames = genericArguments.Select(g => g.Name).ToArray();
            var genericTypeParameterBuilder = typeBuilder.DefineGenericParameters(genericArgumentNames);

            typeBuilder.MakeGenericType(genericTypeParameterBuilder);
        }

        private static NRefactory.ConstructorDeclaration ResolveConstructor(NRefactory.ConstructorDeclaration constructorDeclaration, Type baseType, ILGenerator iLGenerator, List<Cil.Instruction> instructions) {
            ConstructorEmitterVisitor constructorEmitter = null;

            constructorDeclaration = FiltertConstructorStatements(constructorDeclaration, baseType);
            constructorEmitter = new ConstructorEmitterVisitor(constructorDeclaration, new InstructionsIndexer(instructions));
            constructorDeclaration.AcceptVisitor(constructorEmitter, iLGenerator);

            return constructorDeclaration;
        }

        private static NRefactory.ConstructorDeclaration FiltertConstructorStatements(NRefactory.ConstructorDeclaration constructorDeclaration, Type baseType) {
            var block = new NRefactory.BlockStatement();

            foreach (var statement in constructorDeclaration.Body.Statements) {
                var expression = statement as NRefactory.ExpressionStatement;

                if (expression != null) {
                    if (expression.Expression is NRefactory.AssignmentExpression) {
                        continue;
                    }

                    var invocation = expression.Expression as NRefactory.InvocationExpression;

                    block.Add(statement.Clone());

                    if (invocation != null && invocation.HasAnnotationOf<Cecil.MethodReference>()) {
                        var methodReference = invocation.Annotation<Cecil.MethodReference>();

                        if (methodReference.Name.Equals(".ctor") && methodReference.DeclaringType.GetActualType().Equals(baseType)) {
                            block.AddReturnStatement(new NRefactory.ThisReferenceExpression());
                            constructorDeclaration.Body = block;
                            break;
                        }
                    }
                }
                else {
                    block.Add(statement.Clone());
                }
            }

            return constructorDeclaration;
        }

        private static List<Cil.Instruction> FilterInstructions(Collection<Cil.Instruction> instructions, Type baseType) {
            Cil.Instruction returnStatement = instructions[instructions.Count - 1];
            List<Cil.Instruction> filteredInstructions = new List<Cil.Instruction>();

            foreach (var instruction in instructions) {
                filteredInstructions.Add(instruction);

                if (instruction.OpCode == Cil.OpCodes.Call) {
                    var methodReference = instruction.Operand as Cecil.MethodReference;
                    Type declaringType = methodReference.DeclaringType.GetActualType();

                    if (declaringType.Equals(baseType)) {
                        filteredInstructions.Add(returnStatement);
                        break;
                    }
                }
            }

            return filteredInstructions;
        }
    }
}