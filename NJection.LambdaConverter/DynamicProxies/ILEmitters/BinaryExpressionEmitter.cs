using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using System.Linq;
using NJection.LambdaConverter.Extensions;
using ICSharpCode.Decompiler.Ast.Transforms;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class BinaryExpressionEmitter : AbstractDepthFirstVisitorEmitter<BinaryOperatorExpression>
    {
        private Type _leftType = null;
        private Type _rightType = null;
        private Action _emitAction = null;
        private bool _storeArgumentShouldBeEmitted = false;
        private BinaryNumericPromotionDecision _promotionDecisions = null;

        internal BinaryExpressionEmitter(BinaryOperatorExpression binaryOperatorExpression,
                                         ILGenerator ilGenerator,
                                         IOpCodeIndexer instructionIndexer,
                                         IAstVisitor<ILGenerator, AstNode> visitor,
                                         List<LocalBuilder> locals)
            : base(binaryOperatorExpression, ilGenerator, instructionIndexer, visitor, locals) {

            var leftTypeInformation = Node.Left.Annotation<TypeInformation>();
            var rightTypeInformation = Node.Right.Annotation<TypeInformation>();

            if (leftTypeInformation != null && rightTypeInformation != null) {
                _leftType = leftTypeInformation.InferredType.GetActualType();
                _rightType = rightTypeInformation.InferredType.GetActualType();
                _promotionDecisions = BinaryNumericPromotionDecision.Decide(_leftType, _rightType);
                Type = Node.GetBinaryOperationType(_promotionDecisions, _leftType);
                _emitAction = EmitBinaryExpression;
            }
            else {
                _emitAction = EmitTernaryConditionalExpression;
            }
        }

        public void EmitTernaryConditionalExpression() {
            Node.Left.AcceptVisitor(Visitor, ILGenerator);
        }

        public override AstNode Emit() {
            _emitAction();

            return base.Emit();
        }

        public void EmitBinaryExpression() {
            AstNodeDecorator node = null;
            var checkedUnchecked = Node.Annotations.First();
            var isChecked = checkedUnchecked == AddCheckedBlocks.CheckedAnnotation;
            var leftDecision = _promotionDecisions.LeftType;
            var rightDecision = _promotionDecisions.RightType;

            node = Node.Left.AcceptVisitor(ILGenerator, Visitor);

            if (!node.Type.Equals(leftDecision.To) && leftDecision.IsPromoted) {
                ILGenerator.EmitConversion(leftDecision.To, leftDecision.From);
            }

            if (Node.Operator != BinaryOperatorType.ConditionalAnd &&
                Node.Operator != BinaryOperatorType.ConditionalOr &&
                Node.Operator != BinaryOperatorType.NullCoalescing) {

                node = Node.Right.AcceptVisitor(ILGenerator, Visitor);

                if (!(Node.Right is NullReferenceExpression) && !node.Type.Equals(rightDecision.To) && rightDecision.IsPromoted) {
                    ILGenerator.EmitConversion(rightDecision.To, rightDecision.From);
                }
            }

            switch (Node.Operator) {

                case BinaryOperatorType.BitwiseAnd:

                    ILGenerator.Emit(OpCodes.And);
                    break;

                case BinaryOperatorType.BitwiseOr:

                    ILGenerator.Emit(OpCodes.Or);
                    break;

                case BinaryOperatorType.ConditionalAnd: {
                        var falseLabel = ILGenerator.DefineLabel();
                        var trueLabel = ILGenerator.DefineLabel();

                        ILGenerator.Emit(OpCodes.Brfalse_S, falseLabel);
                        Node.Right.AcceptVisitor(Visitor, ILGenerator);
                        ILGenerator.Emit(OpCodes.Br_S, trueLabel);
                        ILGenerator.MarkLabel(falseLabel);
                        ILGenerator.Emit(OpCodes.Ldc_I4_0);
                        ILGenerator.MarkLabel(trueLabel);
                        break;
                    }
                case BinaryOperatorType.ConditionalOr: {
                        var trueLabel = ILGenerator.DefineLabel();
                        var falseLabel = ILGenerator.DefineLabel();

                        ILGenerator.Emit(OpCodes.Brtrue_S, trueLabel);
                        Node.Right.AcceptVisitor(Visitor, ILGenerator);
                        ILGenerator.Emit(OpCodes.Br_S, falseLabel);
                        ILGenerator.MarkLabel(trueLabel);
                        ILGenerator.Emit(OpCodes.Ldc_I4_1);
                        ILGenerator.MarkLabel(falseLabel);
                        break;
                    }
                case BinaryOperatorType.ExclusiveOr:

                    ILGenerator.Emit(OpCodes.Xor);
                    break;

                case BinaryOperatorType.Equality:

                    ILGenerator.Emit(OpCodes.Ceq);
                    break;

                case BinaryOperatorType.InEquality:

                    ILGenerator.Emit(OpCodes.Ceq);
                    ILGenerator.Emit(OpCodes.Ldc_I4_0);
                    ILGenerator.Emit(OpCodes.Ceq);
                    break;

                case BinaryOperatorType.LessThan:

                    ILGenerator.EmitLessThen(_leftType);
                    break;

                case BinaryOperatorType.LessThanOrEqual:

                    ILGenerator.EmitLessThanOrEqual(_leftType, _rightType);
                    break;

                case BinaryOperatorType.GreaterThan:
                    ILGenerator.EmitGreaterThan(_leftType);
                    break;

                case BinaryOperatorType.GreaterThanOrEqual:

                    ILGenerator.EmitGreaterThanOrEqual(_leftType, _rightType);
                    break;

                case BinaryOperatorType.Add:

                    ILGenerator.EmitAddition(_leftType, _rightType, isChecked);

                    if (IsPreCondition(Node.Right)) {
                        ILGenerator.Emit(OpCodes.Dup);
                        _storeArgumentShouldBeEmitted = true;
                    }

                    break;

                case BinaryOperatorType.Subtract: {
                        if (isChecked && IsNegation(Node.Left)) {
                            if (rightDecision.IsPromoted) {
                                ILGenerator.EmitConversion(rightDecision.To, rightDecision.From);
                                ILGenerator.Emit(OpCodes.Sub_Ovf);
                            }
                        }
                        else {
                            ILGenerator.EmitSubtraction(_leftType, _rightType, isChecked);

                            if (IsPreCondition(Node.Right)) {
                                ILGenerator.Emit(OpCodes.Dup);
                                _storeArgumentShouldBeEmitted = true;
                            }
                        }

                        break;
                    }
                case BinaryOperatorType.Multiply:

                    ILGenerator.EmitMultiplication(_leftType, _rightType, isChecked);
                    break;

                case BinaryOperatorType.Divide:

                    ILGenerator.EmitDivision(_leftType);
                    break;

                case BinaryOperatorType.Modulus:

                    ILGenerator.EmitModulu(_leftType);
                    break;

                case BinaryOperatorType.ShiftLeft:
                case BinaryOperatorType.ShiftRight: {
                        int result = _leftType.Equals(TypeSystem.Int) ? 0x1f : 0x3f;
                        ILGenerator.Emit(OpCodes.Ldc_I4_S, result);
                        ILGenerator.Emit(OpCodes.And);

                        if (Node.Operator == BinaryOperatorType.ShiftRight) {
                            ILGenerator.EmitRightShift(_leftType);
                        }
                        else {
                            ILGenerator.Emit(OpCodes.Shl);
                        }
                        break;
                    }
                case BinaryOperatorType.NullCoalescing: {

                        var trueLabel = ILGenerator.DefineLabel();

                        ILGenerator.Emit(OpCodes.Dup);
                        ILGenerator.Emit(OpCodes.Brtrue_S, trueLabel);
                        ILGenerator.Emit(OpCodes.Pop);
                        Node.Right.AcceptVisitor(Visitor, ILGenerator);
                        ILGenerator.MarkLabel(trueLabel);
                        break;
                    }
            }

            if (_storeArgumentShouldBeEmitted) {
                var memberReference = Node.Left as MemberReferenceExpression;

                EmitStoreByArgumentIfNeeded(memberReference);
            }
        }

        private bool IsNegation(Expression expression) {
            return EqualsPrimitiveValue(expression, 0);
        }

        private bool IsPreCondition(Expression expression) {
            return EqualsPrimitiveValue(expression, 1);
        }

        private bool EqualsPrimitiveValue(Expression expression, int value) {
            int primitiveValue;
            var primitive = expression as PrimitiveExpression;

            if (primitive == null) {
                return false;
            }

            if (int.TryParse(primitive.Value.ToString(), out primitiveValue)) {
                return primitiveValue == value;
            }

            return false;
        }

        private void EmitStoreByArgumentIfNeeded(MemberReferenceExpression memberReference) {
            if (memberReference != null) {
                new MemberReferenceReflectionEmitter(memberReference,
                                                     ILGenerator,
                                                     InstructionsIndexer,
                                                     Visitor,
                                                     Locals,
                                                     true).Emit();
            }
            else {
                var variable = Node.Left.Annotation<ILVariable>();
                ILGenerator.EmitStoreArgument(variable.OriginalParameter.Sequence);
            }
        }
    }
}