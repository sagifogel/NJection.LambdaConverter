using System;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class AstNodeDecorator : AstNode, IReflectionType
    {
        internal AstNodeDecorator(AstNode node, Type type = null) {
            Type = type;
            Node = node;
        }

        public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data) {
            throw new NotSupportedException();
        }

        protected override bool DoMatch(AstNode other, Match match) {
            return false;
        }

        public override NodeType NodeType {
            get { return Node.NodeType; }
        }

        public Type Type { get; private set; }

        public AstNode Node { get; private set; }
    }
}