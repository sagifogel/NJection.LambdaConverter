using System;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter
{
    internal class BinaryNumericPromotionDecision
    {
        internal static BinaryNumericPromotionDecision NonNumericPromotion = new BinaryNumericPromotionDecision();
        public TypePromotionDecision LeftType { get; private set; }
        public TypePromotionDecision RightType { get; private set; }

        private static Type[] _comparands = new Type[]{ TypeSystem.Decimal, TypeSystem.Double, 
                                                        TypeSystem.Float, TypeSystem.Ulong, 
                                                        TypeSystem.Long };

        internal static BinaryNumericPromotionDecision Decide(Type leftType, Type rightType) {
            if (!IsNumericPromotion(leftType, rightType)) {
                return NonNumericPromotion;
            }

            return new BinaryNumericPromotionDecision(leftType, rightType);
        }

        private static bool IsNumericPromotion(Type leftType, Type rightType) {
            if (!leftType.IsNumeric() || !rightType.IsNumeric() || leftType.Equals(TypeSystem.Boolean)) {
                return false;
            }

            return true;
        }

        private BinaryNumericPromotionDecision() {
            LeftType = new TypePromotionDecision(null);
            RightType = new TypePromotionDecision(null);
        }

        private BinaryNumericPromotionDecision(Type leftType, Type rightType) {
            if (TryMakeDecisionByMatchingOneType(leftType, rightType)) {
                return;
            }
            else if (leftType.Equals(TypeSystem.Uint)) {
                if (TypesShouldBePromotedToLong(rightType)) {
                    PromoteBothTypesToLong(leftType, rightType);
                }
                else {
                    LeftType = new TypePromotionDecision(leftType);
                    RightType = new TypePromotionDecision(rightType, TypeSystem.Uint);
                }

                return;
            }
            else if (rightType.Equals(TypeSystem.Uint)) {
                if (TypesShouldBePromotedToLong(leftType)) {
                    PromoteBothTypesToLong(leftType, rightType);
                }
                else {
                    LeftType = new TypePromotionDecision(leftType, TypeSystem.Uint);
                    RightType = new TypePromotionDecision(rightType);
                }

                return;
            }

            LeftType = GetDecision(leftType, TypeSystem.Int, true);
            RightType = GetDecision(rightType, TypeSystem.Int, true);
        }

        internal bool IsNotNumericPromotion {
            get {
                return this.Equals(NonNumericPromotion);
            }
        }

        private bool TryMakeDecisionByMatchingOneType(Type leftType, Type rightType) {
            foreach (var comparand in _comparands) {
                if (TryMakeDecision(leftType, rightType, comparand)) {
                    return true;
                }
            }

            return false;
        }

        private bool TryMakeDecision(Type leftType, Type rightType, Type comparand) {
            if (OneIsOfType(leftType, rightType, comparand)) {
                LeftType = GetDecision(leftType, comparand);
                RightType = GetDecision(rightType, comparand);

                return true;
            }

            return false;
        }

        private void PromoteBothTypesToLong(Type leftType, Type rightType) {
            LeftType = GetDecision(leftType, TypeSystem.Long, true);
            RightType = GetDecision(rightType, TypeSystem.Long, true);
        }

        private static bool OneIsOfType(Type leftType, Type rightType, Type comparand) {
            return leftType.Equals(comparand) || rightType.Equals(comparand);
        }

        private static TypePromotionDecision GetDecision(Type type, Type comparand, bool forcePromotion = false) {
            if (!forcePromotion && type.Equals(comparand)) {
                return new TypePromotionDecision(type);
            }

            return new TypePromotionDecision(type, comparand);
        }

        private static bool TypesShouldBePromotedToLong(Type nonUintType) {
            return nonUintType.Equals(TypeSystem.Sbyte) ||
                   nonUintType.Equals(TypeSystem.Short) ||
                   nonUintType.Equals(TypeSystem.Int);
        }
    }
}
