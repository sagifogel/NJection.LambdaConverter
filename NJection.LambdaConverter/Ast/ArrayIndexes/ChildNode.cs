namespace NJection.LambdaConverter.ArrayIndexes
{
    internal class ChildNode : AbstractNode, IChildNode
    {
        public int Index { get; set; }
        public INode ParentNode { get; set; }
    }
}