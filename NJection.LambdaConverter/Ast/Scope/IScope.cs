using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NJection.LambdaConverter
{
    public interface IScope : IScopeChild
    {
        ParameterExpression Find(string name);
        Variable FindVariable(string name);
        IEnumerable<ParameterExpression> Variables { get; }
    }
}