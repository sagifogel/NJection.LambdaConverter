namespace NJection.LambdaConverter
{
    public interface IScopeChild
    {
        IScope ParentScope { get; }
        IMethodScope RootScope { get; }
    }
}