using System;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.DynamicProxies;
using System.Collections.Generic;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Method Method(NRefactory.MethodDeclaration methodDeclaration, object context, INRefcatoryExpressionVisitor visitor, IScope scope = null) {
            Method methodBlock = null;

            if (!methodDeclaration.IsCompilerGeneratorEnumerator(ref methodBlock)) {
                methodBlock = new MethodDeclaration(methodDeclaration, context, scope, visitor);
            }

            return methodBlock;
        }

        public static ConstructorDeclaration Constructor(NRefactory.ConstructorDeclaration constructorDeclaration, Type proxy, IEnumerable<Parameter> outParameters, INRefcatoryExpressionVisitor visitor, IScope scope = null) {
            return new ConstructorDeclaration(constructorDeclaration, proxy, outParameters, scope, visitor);
        }
    }
}