using System;
using System.Linq;
using System.Linq.Expressions;
using ICSharpCode.Decompiler.Ast.Transforms;
using NJection.LambdaConverter.Expressions.Arithmetic;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Binary : AstExpression
    {
        private ExpressionType _binaryOperationType;
        private BinaryNumericPromotionDecision _promotionDecisions = null;
        private NRefactory.BinaryOperatorExpression _binaryOperatorExpression = null;
        private static readonly CheckedUncheckedExpressions _checkedUncheckedExpressions = null;

        static Binary() {
            _checkedUncheckedExpressions = new CheckedUncheckedExpressions();
        }

        protected internal Binary(NRefactory.BinaryOperatorExpression binaryOperatorExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {

            _binaryOperatorExpression = binaryOperatorExpression;
            Left = binaryOperatorExpression.Left.AcceptVisitor(Visitor, ParentScope);
            Right = binaryOperatorExpression.Right.AcceptVisitor(Visitor, ParentScope);
            _binaryOperationType = GetBinaryOperator(binaryOperatorExpression.Operator);
            _promotionDecisions = BinaryNumericPromotionDecision.Decide(Left.Type, Right.Type);
            InternalType = binaryOperatorExpression.GetBinaryOperationType(_promotionDecisions, Left.Type);
        }

        public Expression Left { get; private set; }

        public Expression Right { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Binary; }
        }

        public override Expression Reduce() {
            ExpressionType type;
            bool isChecked = false;
            bool shouldConvertBack = false;
            Expression expression = null;
            CheckedUncheckedEntry checkedUncheckedEntry = null;
            NRefactory.BinaryOperatorType @operator = _binaryOperatorExpression.Operator;

            shouldConvertBack = NotifyTypeConversion();

            if (_checkedUncheckedExpressions.TryGetValue(@operator, out checkedUncheckedEntry)) {
                Func<Expression, Expression, BinaryExpression> binaryMethod = null;
                var checkedAnnotation = _binaryOperatorExpression.Annotations.First();

                isChecked = checkedAnnotation == AddCheckedBlocks.CheckedAnnotation;
                binaryMethod = checkedUncheckedEntry[isChecked];
                expression = binaryMethod(Left, Right);
            }
            else {
                type = GetBinaryOperator(@operator);
                expression = Expression.MakeBinary(type, Left, Right);
            }

            if (shouldConvertBack) {
                var conversionFunction = isChecked ?
                                          Expression.ConvertChecked :
                                          (Func<Expression, Type, UnaryExpression>)Expression.Convert;

                expression = conversionFunction(expression, InternalType);
            }

            return expression;
        }

        private bool NotifyTypeConversion() {
            if (!TypesArePrimitiveAndConvertible(Left.Type, Right.Type)) {
                TypeCode leftTypeCode = Type.GetTypeCode(Left.Type);
                TypeCode rightTypeCode = Type.GetTypeCode(Right.Type);

                if (leftTypeCode == rightTypeCode && leftTypeCode != TypeCode.Object) {
                    Type typeOfInt = TypeSystem.Int;

                    Right = Expression.Convert(Right, typeOfInt);
                    Left = Expression.Convert(Left, typeOfInt);
                    return true;
                }
            }

            return false;
        }

        private bool TypesArePrimitiveAndConvertible(Type source, Type dest) {
            Type nonNullableSourceType = source.GetNonNullableType();
            Type nonNullableDestinationType = dest.GetNonNullableType();

            return (!(source.IsEnum || dest.IsEnum)) && (source.IsEquivalentTo(dest) || ((source.IsNullableType() && dest.IsEquivalentTo(nonNullableSourceType)) || ((dest.IsNullableType() && source.IsEquivalentTo(nonNullableDestinationType)) ||
                   ((source.IsNumeric() && dest.IsNumeric()) && (nonNullableDestinationType != TypeSystem.Boolean)))));
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitBinary(this);
        }

        public Expression Update(Expression left, Expression right) {
            if (Left.Equals(left) && Right.Equals(right)) {
                return this;
            }

            return AstExpression.Binary(_binaryOperatorExpression, ParentScope, Visitor);
        }

        private ExpressionType GetBinaryOperator(NRefactory.BinaryOperatorType @operator) {
            ExpressionType type;

            if (!Enum.TryParse<ExpressionType>(@operator.ToString(), true, out type)) {
                switch (@operator) {
                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.Any:

                        return ExpressionType.Add;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.BitwiseAnd:

                        return ExpressionType.And;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.BitwiseOr:

                        return ExpressionType.Or;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.ConditionalAnd:

                        return ExpressionType.AndAlso;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.ConditionalOr:

                        return ExpressionType.OrElse;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.Divide:

                        return ExpressionType.Divide;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.Equality:

                        return ExpressionType.Equal;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.InEquality:

                        return ExpressionType.NotEqual;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.Modulus:

                        return ExpressionType.Modulo;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.Multiply:

                        return ExpressionType.Multiply;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.NullCoalescing:

                        return ExpressionType.Coalesce;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.ShiftLeft:

                        return ExpressionType.LeftShift;

                    case ICSharpCode.NRefactory.CSharp.BinaryOperatorType.ShiftRight:

                        return ExpressionType.RightShift;
                }
            }

            return type;
        }
    }
}