using System.Linq.Expressions;
using System;

namespace NJection.Core
{
    public class Variable
    {   
        public Variable(ParameterExpression expression) {
            Expression = expression;
        }

        public ParameterExpression Expression { get; private set; }

        public Type Type { 
            get { return Expression.Type;} 
        }

        public string Name {
            get { return Expression.Name; }
        }
    }
}