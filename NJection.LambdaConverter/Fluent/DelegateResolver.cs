using System;
using System.Linq.Expressions;
using System.Reflection;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.Fluent
{
    public class DelegateResolver : IDelegateResolver
    {
        public DelegateResolver(Delegate @delegate) {
            Context = @delegate.Target;
            Method = @delegate.Method;
        }

        public object Context { get; private set; }
        public MethodInfo Method { get; private set; }

        public Func<TDelegate> Resolve<TDelegate>() {
            Delegate @delegate = Method.CreateDelegate(@this: Context);

            return Expression.Lambda<Func<TDelegate>>(
                      Expression.Constant(@delegate)).Compile();
        }
    }
}