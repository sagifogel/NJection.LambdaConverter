namespace NJection.Scope
{
    public interface IScopeChild
    {
        IScope ParentScope { get; }
        IRootScope RootScope { get; }
    }
}