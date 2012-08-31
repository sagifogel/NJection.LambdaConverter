using NJection.LambdaConverter.Expressions;

namespace NJection.LambdaConverter
{
    public interface IMethodScope : IInstructionsIndexer, IBranchingRegistry
    {
        IContext Context { get; }
        IBranchingRegistry BranchingRegistry { get; }
    }
}