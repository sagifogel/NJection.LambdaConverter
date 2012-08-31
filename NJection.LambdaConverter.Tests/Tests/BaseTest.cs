using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using NJection.LambdaConverter.Fluent;

namespace NJection.LambdaConverter.Tests
{
#if DEBUG
    public class BaseTest
    {
        ///Dummy for loading Microsoft.CSharp.dll
        protected Func<CSharpBinderFlags, Type, Type, CallSiteBinder> Convert = Microsoft.CSharp.RuntimeBinder.Binder.Convert;



        public TDelegate ExecuteLambda<TDelegate>(TDelegate func) where TDelegate : class {
            return Lambda.TransformMethodTo<TDelegate>()
                         .From(() => func)
                         .ToLambda()
                         .Compile();
        }

        public TDelegate ExecuteLambdaWithContext<TDelegate, TContext>(TDelegate func, TContext context) where TDelegate : class {
            return Lambda.TransformMethodTo<TDelegate>()
                         .From(() => func)
                         .WithContextOf<TContext>(context)
                         .ToLambda()
                         .Compile();
        }



        public TDelegate ExecuteLambda<TDelegate>(Delegate func) where TDelegate : class {
            var resolver = new DelegateResolver(func);

            return func.ResolveMethodTo<TDelegate>(resolver)
                       .ToLambda()
                       .Compile();
        }

        public TDelegate ExecuteLambdaWithContext<TDelegate, TContext>(Delegate func, TContext context) where TDelegate : class {
            var resolver = new DelegateResolver(func);

            return func.ResolveMethodTo<TDelegate>(resolver)
                       .WithContextOf<TContext>(context)
                       .ToLambda()
                       .Compile();
        }

        public TDelegate ExecuteConstructorLambda<TDelegate>(ConstructorInfo ctor) where TDelegate : class {
            var resolver = new ConstructorResolver(ctor);

            return Lambda.ResolveConstructorTo<TDelegate>(resolver)
                         .ToLambda()
                         .Compile();
        }
    }
#endif
}