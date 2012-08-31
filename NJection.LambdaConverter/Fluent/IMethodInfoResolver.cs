using System.Reflection;

namespace NJection.LambdaConverter.Fluent
{
    public interface IDelegateResolver : IMethodResolver
    {
        object Context { get; }
        MethodInfo Method { get; }
    }
}