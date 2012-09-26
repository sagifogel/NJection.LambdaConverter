using System.Collections.Generic;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Block Block(NRefactory.BlockStatement blockStatement, IScope scope, INRefcatoryExpressionVisitor visitor, IEnumerable<ParameterExpression> parameters = null) {
            return new Block(blockStatement, parameters: parameters, visitor: visitor, scope: scope);
        }
    }
}