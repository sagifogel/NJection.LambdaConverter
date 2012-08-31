using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NJection.LambdaConverter.Fluent
{
    public class ConstructorResolver : IConstructorResolver
    {
        public ConstructorResolver(ConstructorInfo ctor) {
            Ctor = ctor;
        }

        public ConstructorInfo Ctor { get; private set; }

        public Func<TDelegate> Resolve<TDelegate>() {
            var parameters = Ctor.GetParameters()
                                 .Select(p => Expression.Parameter(p.ParameterType))
                                 .ToArray();

            var @delegate = Expression.Lambda<TDelegate>(
                                Expression.New(Ctor, parameters), parameters);

            return Expression.Lambda<Func<TDelegate>>(@delegate).Compile();
        }
    }
}