using System;
using System.Linq.Expressions;

namespace NJection.LambdaConverter.Expressions
{
    public class ConstantContext : IContext
    {
        public ConstantContext(object value) {
            Type = value.GetType();
            Expression = Expression.Constant(value);
            Value = value;
        }

        public Type Type { get; private set; }
        public object Value { get; private set; }
        public Expression Expression { get; private set; }
    }
}