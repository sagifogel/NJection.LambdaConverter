using System.Collections.Generic;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal abstract class AbstractReflectionEmitter : AbstractILEmitter
    {
        protected bool NonPublic = false;
        protected List<LocalBuilder> Locals = null;
        protected IAstVisitor<ILGenerator, AstNode> Visitor = null;

        internal AbstractReflectionEmitter(ILGenerator ilGenerator,
                                           IOpCodeIndexer instructionsIndexer,
                                           IAstVisitor<ILGenerator, AstNode> visitor,
                                           List<LocalBuilder> locals)
            : base(ilGenerator, instructionsIndexer) {
            Visitor = visitor;
            Locals = locals;
        }

        protected internal abstract void EmitPrivateReference();
    }
}