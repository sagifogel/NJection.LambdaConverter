using System.Collections.ObjectModel;

namespace NJection.LambdaConverter.ArrayIndexes
{
    internal interface INode
    {
        IRootNode Root { get; set; }
        Collection<INode> Nodes { get; set; }
    }
}