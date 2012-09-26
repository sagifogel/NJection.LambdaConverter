using System.Collections.Generic;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static ConstructorBlock ConstructorBlock(NRefactory.BlockStatement blockStatement,
                                                        ParameterExpression contextParameter,
                                                        IScope scope,
                                                        INRefcatoryExpressionVisitor visitor,
                                                        IEnumerable<ParameterExpression> parameters = null,
                                                        IEnumerable<ParameterExpression> baseConstructorParameters = null) {
            return new ConstructorBlock(blockStatement, contextParameter, parameters: parameters, scope: scope, visitor: visitor, baseConstructorParameters: baseConstructorParameters);
        }
    }
}