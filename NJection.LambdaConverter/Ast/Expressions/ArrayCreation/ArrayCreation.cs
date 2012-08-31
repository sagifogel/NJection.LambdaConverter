using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NJection.LambdaConverter.ArrayIndexes;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class ArrayCreation : AstExpression
    {
        private Type _baseType = null;
        private bool _isJaggeedArray = false;
        private bool _isVector = false;
        private Expression _newArrayCreation = null;
        private NRefactory.ArrayCreateExpression _arrayCreateExpression = null;

        protected internal ArrayCreation(NRefactory.ArrayCreateExpression arrayCreateExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _arrayCreateExpression = arrayCreateExpression;
            _isJaggeedArray = IsJaggedArray();
            _isVector = IsVector();
            InternalType = GetArrayType();

            if (InternalType.GetArrayRank() == 1 && !_isJaggeedArray) {
                if (TryGetOneDimensionalArrayBounds()) {
                    BuildEmptyOneDimensionalArray();
                }
                else {
                    BuildOneDimensionalArray();
                }
            }
            else {
                if (TryGetBounds()) {
                    BuildEmptyMultiDimensionalArray();
                }
                else {
                    BuildMultiDimensionalArrayAccess();
                }
            }
        }

        public IEnumerable<Expression> Bounds { get; private set; }

        public IEnumerable<Expression> Initializers { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.ArrayCreation; }
        }

        public override Expression Reduce() {
            return _newArrayCreation;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitArrayCreation(this);
        }

        public Expression Update(IEnumerable<Expression> initializers, IEnumerable<Expression> bounds) {
            if (ReferenceEquals(Initializers, initializers) && ReferenceEquals(Bounds, bounds)) {
                return this;
            }

            return AstExpression.ArrayCreation(_arrayCreateExpression, ParentScope, Visitor);
        }

        private bool IsJaggedArray() {
            return _arrayCreateExpression.Initializer == NRefactory.ArrayInitializerExpression.Null ||
                   _arrayCreateExpression.Initializer.FirstChild is NRefactory.ArrayCreateExpression;
        }

        private bool IsVector() {
            return _arrayCreateExpression.Arguments.Count == 1;
        }

        private Type GetArrayType() {
            var type = _baseType = _arrayCreateExpression.Type.AcceptVisitor(Visitor, ParentScope).Type;
            var arguments = _arrayCreateExpression.Arguments;
            var additionalSpecifiers = _arrayCreateExpression.AdditionalArraySpecifiers;
            var specifiers = additionalSpecifiers.Reverse()
                                                 .Select(a => a.Dimensions);

            if (arguments.Count > 0) {
                specifiers = specifiers.Concat(arguments.Count);
            }

            specifiers.ForEach(rank => {
                type = type.MakeSafeArrayType(rank);
            });

            _baseType = type.GetElementType();

            return type;
        }

        private void BuildOneDimensionalArray() {
            BuildOneDimensionalInitializers(_arrayCreateExpression.Initializer);
            _newArrayCreation = Expression.NewArrayInit(_baseType, Initializers);
        }

        private bool TryGetOneDimensionalArrayBounds() {
            var bounds = _arrayCreateExpression.Arguments
                                               .Select(b => b.AcceptVisitor(Visitor, ParentScope))
                                               .Where(b => b.AstNodeType != AstExpressionType.Empty)
                                               .ToList();

            if (bounds.Count == 1) {
                Bounds = bounds;
                return true;
            }

            return false;
        }

        private bool TryGetBounds() {
            var bounds = _arrayCreateExpression.Arguments
                                               .Select(b => b.AcceptVisitor(Visitor, ParentScope))
                                               .Where(b => b.AstNodeType != AstExpressionType.Empty)
                                               .ToList();

            Bounds = bounds;
            return bounds.Count > 0;
        }

        private void BuildEmptyOneDimensionalArray() {
            _newArrayCreation = Expression.NewArrayBounds(_baseType, Bounds);
        }

        private void BuildEmptyMultiDimensionalArray() {
            _newArrayCreation = Expression.NewArrayBounds(_baseType, Bounds);
        }

        private void BuildOneDimensionalInitializers(NRefactory.ArrayInitializerExpression initializer) {
            Initializers = initializer.Elements
                                      .Select(e => e.AcceptVisitor(Visitor, ParentScope));
        }

        private void BuildIndexes(NRefactory.ArrayInitializerExpression initializer, INode currentNodeIndex) {
            initializer.Elements.ForEach(e => {
                var childInitializer = e as NRefactory.ArrayInitializerExpression;

                if (childInitializer != null) {
                    var newIndex = new ChildNode { ParentNode = currentNodeIndex };

                    newIndex.Root = currentNodeIndex.Root;
                    newIndex.Index = currentNodeIndex.Nodes.Count;
                    currentNodeIndex.Nodes.Add(newIndex);
                    BuildIndexes(childInitializer, newIndex);
                }
                else {
                    var newIndex = new LinqExpressionNode { ParentNode = currentNodeIndex };

                    newIndex.Root = currentNodeIndex.Root;
                    newIndex.Index = currentNodeIndex.Nodes.Count;
                    currentNodeIndex.Nodes.Add(newIndex);
                    newIndex.Value = e.AcceptVisitor(Visitor, ParentScope);
                }
            });
        }

        private void BuildMultiDimensionalArrayAccess() {
            Func<INode, IEnumerable<int>> getBounds = null;

            getBounds = (INode node) => {
                List<int> bounds = new List<int>();
                int nodeCount = node.Nodes.Count;

                if (nodeCount != 0) {
                    bounds.Add(nodeCount);
                    bounds.AddRange(getBounds(node.Nodes[0]));
                }

                return bounds;
            };

            var root = new MultiDimensionalArrayIndexes() { Rank = InternalType.GetArrayRank() };
            var indexExpressions = new List<Tuple<Expression, IEnumerable<Expression>>>();

            BuildIndexes(_arrayCreateExpression.Initializer, root);

            root.Nodes.Cast<IChildNode>().ForEach((node, i) => {
                var indexes = new List<int>() { i };

                if (root.Rank > 1) {
                    BuildArrayAccess(node, indexes, indexExpressions);
                }
                else {
                    indexExpressions.Add(CreateIndexExpression(node as ILeafNode<Expression>, indexes));
                }
            });

            var arrayBounds = getBounds(root).Select(i => Expression.Constant(i, TypeSystem.Int));
            var arrayCreation = Expression.NewArrayBounds(_baseType, arrayBounds);
            var array = Expression.Parameter(InternalType);

            var arrayInitializers = indexExpressions.Select(tuple => {
                return Expression.Assign(
                            Expression.ArrayAccess(array, tuple.Item2),
                                tuple.Item1);
            });

            _newArrayCreation = Expression.Invoke(
                                    Expression.Lambda(
                                       Expression.GetFuncType(new[] { InternalType }),
                                            Expression.Block(new[] { array },
                                                new Expression[] {
                                                Expression.Assign(array, arrayCreation) }
                                                    .Concat(arrayInitializers)
                                                    .Concat(array))));
        }

        private void BuildArrayAccess(IChildNode currentNodeIndex, List<int> indexes, List<Tuple<Expression, IEnumerable<Expression>>> expressions) {
            foreach (IChildNode item in currentNodeIndex.Nodes) {
                indexes.Add(item.Index);

                if (item.Nodes.Count > 0) {
                    BuildArrayAccess(item, indexes, expressions);
                }
                else {
                    var leaf = item as ILeafNode<Expression>;
                    var tuple = CreateIndexExpression(leaf, indexes);

                    expressions.Add(tuple);
                }

                indexes.RemoveAt(indexes.Count - 1);
            }
        }

        private Tuple<Expression, IEnumerable<Expression>> CreateIndexExpression(ILeafNode<Expression> leaf, List<int> indexes) {
            var indexExpressions = indexes.Select(i => Expression.Constant(i, TypeSystem.Int));

            return Tuple.Create<Expression, IEnumerable<Expression>>(leaf.Value, indexExpressions.ToList());
        }
    }
}