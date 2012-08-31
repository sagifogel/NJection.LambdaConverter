using System;
using NJection.LambdaConverter.Extensions;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter
{
    internal class UnaryNumericPromotionDecision
    {
        internal static UnaryNumericPromotionDecision NonNumericPromotion = new UnaryNumericPromotionDecision();
        internal TypePromotionDecision Type { get; private set; }

        private static UnaryOperatorType[] _unaryOperators = new UnaryOperatorType[] { UnaryOperatorType.Plus, 
                                                                                       UnaryOperatorType.Minus, 
                                                                                       UnaryOperatorType.BitNot };

        private static Type[] _convertableTypes = new Type[] { TypeSystem.Sbyte, 
                                                               TypeSystem.Byte, 
                                                               TypeSystem.Short, 
                                                               TypeSystem.Ushort, 
                                                               TypeSystem.Char };

        internal static UnaryNumericPromotionDecision Decide(Type type, UnaryOperatorType @operator) {
            if (!IsNumericPromotion(type, @operator)) {
                return NonNumericPromotion;
            }

            return new UnaryNumericPromotionDecision(type, @operator);
        }

        private UnaryNumericPromotionDecision() {
            Type = new TypePromotionDecision(null);
        }

        private UnaryNumericPromotionDecision(Type type, UnaryOperatorType @operator) {
            switch (@operator) {
                case UnaryOperatorType.BitNot:
                case UnaryOperatorType.Minus:
                case UnaryOperatorType.Plus:
                    if (Array.IndexOf(_convertableTypes, type) > -1) {
                        Type = new TypePromotionDecision(type, TypeSystem.Int);
                        return;
                    }

                    if (@operator == UnaryOperatorType.Minus && type.Equals(TypeSystem.Uint)) {
                        Type = new TypePromotionDecision(type, TypeSystem.Long);
                        return;
                    }

                    break;
            }

            Type = new TypePromotionDecision(type); 
        }

        internal bool IsNotNumericPromotion {
            get { return this.Equals(NonNumericPromotion); }
        }

        private static bool IsNumericPromotion(Type type, UnaryOperatorType @operator) {
            if (!type.IsNumeric() ||
                type.Equals(TypeSystem.Boolean) ||
                Array.IndexOf(_unaryOperators, @operator) == -1) {
                return false;
            }

            return true;
        }
    }
}
