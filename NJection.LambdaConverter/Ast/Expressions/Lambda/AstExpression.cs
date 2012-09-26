using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Lambda Lambda(Type lambdaType, Expression body, IEnumerable<ParameterExpression> parameterExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Lambda(lambdaType, body, parameterExpression, scope, visitor);
        }
    }
}