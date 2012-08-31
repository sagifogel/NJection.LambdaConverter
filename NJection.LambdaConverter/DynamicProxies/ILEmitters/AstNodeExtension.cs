using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal static class AstNodeExtension
    {
        internal static AstNodeDecorator AcceptVisitor<T, S>(this AstNode node, T data, IAstVisitor<T, S> visitor) {
            return node.AcceptVisitor(visitor, data) as AstNodeDecorator;
        }
    }
}