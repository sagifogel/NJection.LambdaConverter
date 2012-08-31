using System;

namespace NJection.LambdaConverter.Fluent
{
    public interface IMethodTranformer<TDelegate>
    {
        IContextProvider<TDelegate> From(Func<TDelegate> methodResolver);
    }
}