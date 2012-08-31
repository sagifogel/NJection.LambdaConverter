using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Mappers;

namespace NJection.LambdaConverter.Extensions
{
    internal static class NRefactoryExpressionsExtensions
    {
        public static Type GetBinaryOperationType(this BinaryOperatorExpression binaryOperatorExpression, BinaryNumericPromotionDecision decision, Type fallbackType) {
            BinaryOperatorType binaryOperatorType = binaryOperatorExpression.Operator;

            bool booleanCondition = binaryOperatorType.Is(BinaryOperatorType.GreaterThan) ||
                                    binaryOperatorType.Is(BinaryOperatorType.GreaterThanOrEqual) ||
                                    binaryOperatorType.Is(BinaryOperatorType.LessThan) ||
                                    binaryOperatorType.Is(BinaryOperatorType.LessThanOrEqual) ||
                                    binaryOperatorType.Is(BinaryOperatorType.Equality) ||
                                    binaryOperatorType.Is(BinaryOperatorType.InEquality);

            if (booleanCondition) {
                return TypeSystem.Boolean;
            }

            if (!decision.IsNotNumericPromotion) {
                return decision.LeftType.IsPromoted ? decision.LeftType.To : decision.LeftType.From;
            }

            return fallbackType;
        }

        private static bool Is(this BinaryOperatorType @operator, BinaryOperatorType comparedOperator) {
            return @operator == comparedOperator;
        }
    }
}
