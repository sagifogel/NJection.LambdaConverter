namespace NJection.LambdaConverter.ArrayIndexes
{
    internal interface IChildNode : INode
    {
        int Index { get; set; }
        INode ParentNode { get; set; }
    }
}