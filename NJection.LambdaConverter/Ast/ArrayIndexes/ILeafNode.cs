namespace NJection.LambdaConverter.ArrayIndexes
{
    internal interface ILeafNode<T> : IChildNode
    {
        T Value { get; set; }
    }
}