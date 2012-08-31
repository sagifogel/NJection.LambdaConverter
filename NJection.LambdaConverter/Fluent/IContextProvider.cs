namespace NJection.LambdaConverter.Fluent
{
    public interface IContextProvider<TDelegate> : IMethodCompiler<TDelegate>
    {
        IContextProvider<TDelegate> WithContextOf<TContext>(TContext context);
    }
}