using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil.Cil;

namespace NJection.LambdaConverter
{
    public interface IInstructionsIndexer
    {
        Instruction GetInstruction(AstNode node);
        Instruction GetLastInstructionInRange(AstNode node);
        Instruction GetNextInstruction(Instruction instruction);
        bool TryGetInstruction(AstNode node, OpCode opCode, out Instruction instruction);
    }
}