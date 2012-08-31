using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil.Cil;

namespace NJection.LambdaConverter
{
    internal interface IOpCodeIndexer : IInstructionsIndexer
    {
        bool TryGetStoreFieldInstruction(AstNode node, out Instruction instruction);
        bool TryGetLoadFieldInstruction(AstNode node, out Instruction instruction);
        bool TryGetConvertInstruction(CastExpression node, Type type, out Instruction instruction);
        bool TryGetNewObjectInstruction(AstNode node, out Instruction instruction);
        bool TryGetCallInstruction(AstNode node, out Instruction instruction);
        IEnumerable<Instruction> GetRangeOfInstructions(Instruction startFrom, int count);
        IEnumerable<Instruction> GetPrevoiusRangeOfInstructions(Instruction startFrom, int count);
    }
}