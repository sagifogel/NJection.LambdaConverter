using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal interface ILEmitter : IReflectionType
    {
        AstNode Emit();
        ILGenerator ILGenerator { get; }
        IOpCodeIndexer InstructionsIndexer { get; }
    }
}