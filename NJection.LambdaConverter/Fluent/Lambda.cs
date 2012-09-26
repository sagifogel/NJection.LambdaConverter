using System;
using System.Reflection;
using NJection.Core;

namespace NJection.LambdaConverter.Fluent
{
    public static class Lambda
    {
        public static IMethodTranformer<TDelegate> TransformMethodTo<[DelegateConstraint]TDelegate>() where TDelegate : class {
            return new DelegateExpressionizer<TDelegate>();
        }

        public static IContextProvider<TDelegate> ResolveMethodTo<[DelegateConstraint]TDelegate>(IDelegateResolver methodResolver) where TDelegate : class {
            Func<TDelegate> resolved = methodResolver.Resolve<TDelegate>();

            return new DelegateExpressionizer<TDelegate>().From(resolved);
        }

        public static IMethodCompiler<TDelegate> ResolveConstructorTo<[DelegateConstraint]TDelegate>(IConstructorResolver constructorInfoResolver) where TDelegate : class {
            Func<TDelegate> resolved = constructorInfoResolver.Resolve<TDelegate>();

            return new ConstructorExpressionizer<TDelegate>().From(resolved);
        }

        public static IContextProvider<TDelegate> ResolveMethodTo<[DelegateConstraint]TDelegate>(this Delegate @delegate, IDelegateResolver methodInfoResolver) where TDelegate : class {
            return ResolveMethodTo<TDelegate>(methodInfoResolver);
        }

        public static IMethodCompiler<TDelegate> ResolveConstructorTo<[DelegateConstraint]TDelegate>(this ConstructorInfo ctor, IConstructorResolver constructorInfoResolver) where TDelegate : class {
            return ResolveConstructorTo<TDelegate>(constructorInfoResolver);
        }
    }
}