using System.Linq.Expressions;

namespace NJection.LambdaConverter.Fluent
{
    public interface IMethodCompiler<TDelegate>
    {
        TDelegate Compile();
        Expression<TDelegate> ToLambda();
    }
}