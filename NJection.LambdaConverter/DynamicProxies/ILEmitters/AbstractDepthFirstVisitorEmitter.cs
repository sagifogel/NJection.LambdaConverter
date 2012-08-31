using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal abstract class AbstractDepthFirstVisitorEmitter<TAstNode> : DepthFirstAstVisitor<ILGenerator, AstNode>, ILEmitter
        where TAstNode : AstNode
    {
        internal AbstractDepthFirstVisitorEmitter(TAstNode node,
                                                  ILGenerator ilGenerator,
                                                  IOpCodeIndexer instructionsIndexer,
                                                  IAstVisitor<ILGenerator, AstNode> visitor,
                                                  List<LocalBuilder> locals = null) {
            Node = node;
            Visitor = visitor;
            Locals = locals;
            ILGenerator = ilGenerator;
            InstructionsIndexer = instructionsIndexer;
        }

        public virtual AstNode Emit() {
            return new AstNodeDecorator(Node, Type);
        }

        public Type Type { get; protected set; }
        protected TAstNode Node { get; private set; }
        public ILGenerator ILGenerator { get; private set; }
        protected List<LocalBuilder> Locals { get; private set; }
        public IOpCodeIndexer InstructionsIndexer { get; private set; }
        protected IAstVisitor<ILGenerator, AstNode> Visitor { get; private set; }
    }
}