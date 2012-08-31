using System.Collections.ObjectModel;

namespace NJection.LambdaConverter.ArrayIndexes
{
    internal abstract class AbstractNode : INode
    {
        internal AbstractNode() {
            Nodes = new Collection<INode>();
        }

        public IRootNode Root { get; set; }
        public Collection<INode> Nodes { get; set; }
    }
}