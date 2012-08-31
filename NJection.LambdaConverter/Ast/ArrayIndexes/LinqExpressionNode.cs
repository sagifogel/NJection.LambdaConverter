using System.Linq.Expressions;

namespace NJection.LambdaConverter.ArrayIndexes
{
    internal class LinqExpressionNode : ChildNode, ILeafNode<Expression>
    {
        public Expression Value { get; set; }
    }
}