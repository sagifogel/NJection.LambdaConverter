using NJection.LambdaConverter.Expressions;

namespace NJection.Core
{
    public interface IMethodScope : IBranchingRegistry
    {
        IContext Context { get; }
        IBranchingRegistry BranchingRegistry { get; }
    }
}