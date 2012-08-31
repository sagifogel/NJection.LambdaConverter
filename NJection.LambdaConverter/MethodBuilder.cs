using System;
using System.Linq.Expressions;
using System.Reflection;
using ICSharpCode.Decompiler;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using AstDecompiler = ICSharpCode.Decompiler.Ast;

using LinqExpressions = System.Linq.Expressions;

namespace NJection.LambdaConverter
{
    internal class MethodBuilder
    {
        private AstDecompiler.AstBuilder _builder = null;
        private MethodDefinition _methodDefinition = null;
        private static Type[] _types = { typeof(MethodDefinition) };
        private static ParameterExpression _builderParameter = null;
        private static ParameterExpression _methodDefinitionParameter = null;
        private static Func<AstDecompiler.AstBuilder, MethodDefinition, AstNode> _methodBuilder = null;
        private static Func<AstDecompiler.AstBuilder, MethodDefinition, AstNode> _cunstructorBuilder = null;

        static MethodBuilder() {
            _builderParameter = LinqExpressions.Expression.Parameter(typeof(AstDecompiler.AstBuilder));
            _methodDefinitionParameter = LinqExpressions.Expression.Parameter(typeof(MethodDefinition));
            _methodBuilder = GetMethodDeclarationDelegate();
            _cunstructorBuilder = GetConstructorDeclarationDelegate();
        }

        internal MethodBuilder(MethodDefinition methodDefinition, ModuleDefinition currentModule) {
            _methodDefinition = methodDefinition;
            _builder = new AstDecompiler.AstBuilder(new DecompilerContext(currentModule) {
                CurrentMethod = methodDefinition,
                CurrentType = methodDefinition.DeclaringType,
                Settings = new DecompilerSettings()
            });
        }

        private static MethodInfo GetMethodInfo(string methodName) {
            return typeof(AstDecompiler.AstBuilder).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic, null, _types, null);
        }

        private static Func<AstDecompiler.AstBuilder, MethodDefinition, AttributedNode> GetMethodDeclarationDelegate() {
            MethodInfo methodInfo = GetMethodInfo("CreateMethod");
            LinqExpressions.MethodCallExpression methodCall = LinqExpressions.Expression.Call(_builderParameter, methodInfo, _methodDefinitionParameter);

            return LinqExpressions.Expression.Lambda<Func<AstDecompiler.AstBuilder, MethodDefinition, AttributedNode>>
                        (methodCall, _builderParameter, _methodDefinitionParameter).Compile();
        }

        private static Func<AstDecompiler.AstBuilder, MethodDefinition, ConstructorDeclaration> GetConstructorDeclarationDelegate() {
            MethodInfo methodInfo = GetMethodInfo("CreateConstructor");
            LinqExpressions.MethodCallExpression methodCall = LinqExpressions.Expression.Call(_builderParameter, methodInfo, _methodDefinitionParameter);

            return LinqExpressions.Expression.Lambda<Func<AstDecompiler.AstBuilder, MethodDefinition, ConstructorDeclaration>>
                        (methodCall, _builderParameter, _methodDefinitionParameter).Compile();
        }

        internal AstNode BuildMethod(MethodDefinition methodDefinition) {
            return _methodBuilder(_builder, methodDefinition);
        }

        internal AstNode BuildConstructor(MethodDefinition methodDefinition) {
            return _cunstructorBuilder(_builder, methodDefinition);
        }
    }
}