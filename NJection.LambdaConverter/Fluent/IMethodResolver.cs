using System;

namespace NJection.LambdaConverter.Fluent
{
    public interface IMethodResolver
    {
        Func<TDelegate> Resolve<TDelegate>();
    }
}