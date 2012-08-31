using System;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal abstract class AbstractILEmitter : ILEmitter
    {
        internal AbstractILEmitter(ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer) {
            ILGenerator = ilGenerator;
            InstructionsIndexer = instructionsIndexer;
        }

        public abstract AstNode Emit();
        public Type Type { get; protected set; }
        public ILGenerator ILGenerator { get; private set; }
        public IOpCodeIndexer InstructionsIndexer { get; private set; }
    }
}