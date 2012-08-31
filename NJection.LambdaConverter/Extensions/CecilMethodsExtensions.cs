using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Lambda = NJection.LambdaConverter.Expressions;

namespace NJection.LambdaConverter.Extensions
{
    internal static class CecilMethodsExtensions
    {
        private static readonly Type _methedDeclarationType = null;
        private static readonly Type _constructorDeclaration = null;
        private static readonly List<Type> _ienumeratorTypes = null;
        private static readonly ConcurrentDictionary<string, AssemblyDefinition> _assembliesDefinition = null;

        static CecilMethodsExtensions() {
            _methedDeclarationType = typeof(MethodDeclaration);
            _constructorDeclaration = typeof(ConstructorDeclaration);
            _ienumeratorTypes = new List<Type> { typeof(IEnumerable), typeof(IEnumerator) };
            _assembliesDefinition = new ConcurrentDictionary<string, AssemblyDefinition>();
        }

        internal static T ResolveMothod<T>(this MethodDefinition methodDefinition) where T : AstNode {
            if (!(typeof(T).IsAssignableFrom(_methedDeclarationType) || typeof(T).IsAssignableFrom(_constructorDeclaration))) {
                throw new ArgumentException();
            }

            AssemblyDefinition assemblyDefinition = methodDefinition.GetResolvedAssembly();
            MethodBuilder builder = new MethodBuilder(methodDefinition, assemblyDefinition.MainModule);
            Func<MethodDefinition, AstNode> methodBuilder = methodDefinition.IsConstructor ?
                                                            builder.BuildConstructor :
                                                            (Func<MethodDefinition, AstNode>)builder.BuildMethod;

            return methodBuilder(methodDefinition) as T;
        }

        internal static MethodDefinition ResolveMethodDefinition(this MethodInfo methodInfo) {
            return GetResolvedAssembly(methodInfo).ResolveMethodDefinition(methodInfo);
        }

        internal static MethodDefinition ResolveConstructorDefinition(this ConstructorInfo constructorInfo) {
            return GetResolvedAssembly(constructorInfo).ResolveConstructorDefinition(constructorInfo);
        }

        internal static Type GetReturnType(this MethodDeclaration methodDeclaration) {
            if (methodDeclaration != null) {
                var methodReference = methodDeclaration.Annotation<MethodReference>();

                if (methodReference != null) {
                    return methodReference.ReturnType.GetActualType();
                }
            }

            return TypeSystem.Void;
        }

        internal static Type GetActualType(this ConstructorDeclaration constructorDeclaration) {
            if (constructorDeclaration != null) {
                var methodReference = constructorDeclaration.Annotation<MethodReference>();

                if (methodReference != null) {
                    return methodReference.GetActualMethod().DeclaringType;
                }
            }

            return TypeSystem.Void;
        }

        internal static bool IsCompilerGeneratorEnumerator(this MethodDeclaration methodDeclaration, ref Lambda.Method methodBlock) {
            var methodDefinition = methodDeclaration.Annotation<MethodDefinition>();
            var candidateNewObj = methodDefinition.Body.Instructions[1];

            if (candidateNewObj.OpCode.Equals(OpCodes.Newobj)) {
                var memberReference = candidateNewObj.Operand as Mono.Cecil.MethodReference;

                if (memberReference != null) {
                    var constructorInfo = memberReference.GetActualMethod<ConstructorInfo>();

                    if (constructorInfo != null && constructorInfo.DeclaringType.IsCompilerGenerated() && IsEnumerator(methodDefinition)) {
                        Instruction initializer = methodDefinition.Body.Instructions[0];

                        methodBlock = new Lambda.CompilerGeneratedEnumerator(initializer, candidateNewObj);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsEnumerator(MethodDefinition methodDefinition) {
            Func<Type, bool> any = (type) => _ienumeratorTypes.Any(t => type.Equals(t) || t.IsSubType(type));
            Type actualType = methodDefinition.ReturnType.GetActualType();

            if (actualType.IsGenericType) {
                return actualType.GetInterfaces()
                                 .Any(i => any(i));
            }

            return any(actualType);
        }

        private static AssemblyDefinition GetResolvedAssembly(this MethodBase methodBase) {
            string assemblyName = methodBase.DeclaringType.FullName;

            return _assembliesDefinition.GetOrAdd(assemblyName, key => {
                return ResolveAssemblyDefinition(methodBase.DeclaringType);
            });
        }

        private static AssemblyDefinition GetResolvedAssembly(this MethodDefinition methodDefinition) {
            string assemblyName = methodDefinition.DeclaringType.FullName;

            return _assembliesDefinition.GetOrAdd(assemblyName, key => {
                var declaringType = methodDefinition.DeclaringType.GetActualType();
                return ResolveAssemblyDefinition(declaringType);
            });
        }

        private static AssemblyDefinition ResolveAssemblyDefinition(Type declaringType) {
            string path = declaringType.Assembly.ManifestModule.FullyQualifiedName;
            ReaderParameters parameters = new ReaderParameters() { AssemblyResolver = AssemblyResolver.Instance };

            return AssemblyDefinition.ReadAssembly(path, parameters);
        }

        private static MethodDefinition ResolveConstructorDefinition(this AssemblyDefinition assemblyDefinition, ConstructorInfo constructorInfo) {
            ParameterInfo[] parametersInfo = constructorInfo.GetParameters();
            ModuleDefinition moduleDefinition = assemblyDefinition.MainModule;
            TypeDefinition typeDefinitions = moduleDefinition.Types
                                                             .Union(moduleDefinition.Types.SelectMany(t => t.NestedTypes))
                                                             .First(t => t.FullName.Replace("/", "+").Equals(constructorInfo.DeclaringType.FullName));

            return typeDefinitions.Methods.FirstOrDefault(method => method.CtorMatch(constructorInfo));
        }

        private static MethodDefinition ResolveMethodDefinition(this AssemblyDefinition assemblyDefinition, MethodInfo methodInfo) {
            ParameterInfo[] parametersInfo = methodInfo.GetParameters();
            ModuleDefinition moduleDefinition = assemblyDefinition.MainModule;
            TypeDefinition typeDefinitions = moduleDefinition.Types
                                                             .Union(moduleDefinition.Types.SelectMany(t => t.NestedTypes))
                                                             .First(t => t.FullName.Replace("/", "+").Equals(methodInfo.DeclaringType.FullName));

            return typeDefinitions.Methods.FirstOrDefault(method => method.MethodMatch(methodInfo));
        }
    }
}