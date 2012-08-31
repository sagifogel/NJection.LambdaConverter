namespace NJection.LambdaConverter.ArrayIndexes
{
    internal class MultiDimensionalArrayIndexes : AbstractNode, IRootNode
    {
        internal MultiDimensionalArrayIndexes() {
            Root = this;
        }

        public int Rank { get; set; }
    }
}