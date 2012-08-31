using System.Reflection;

namespace NJection.LambdaConverter.Fluent
{
    public interface IConstructorResolver : IMethodResolver
    {
        ConstructorInfo Ctor { get; }
    }
}