using System.Collections.Generic;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;

namespace NJection.LambdaConverter.Expressions
{
    public abstract class Method : Scope
    {
        public Method(INRefcatoryExpressionVisitor visitor, IScope scope = null)
            : base(scope, visitor) { }

        public Expression Body { get; protected set; }

        public IEnumerable<ParameterExpression> Parameters { get; protected set; }
    }
}