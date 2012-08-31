using System;
using System.Linq.Expressions;

namespace NJection.LambdaConverter.Expressions
{
    public class ParameterContext : IContext
    {
        public ParameterContext(ParameterExpression expression) {
            Type = expression.Type;
            Expression = expression;
        }

        public Type Type { get; private set; }
        public object Value { get; private set; }
        public Expression Expression { get; private set; }
    }
}