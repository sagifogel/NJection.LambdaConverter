using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Emit;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;
using ICSharpCode.Decompiler.Ast.Transforms;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class UnaryExpressionEmitter : AbstractDepthFirstVisitorEmitter<UnaryOperatorExpression>
    {
        private bool _storeArgumentShouldBeEmitted = false;
        private UnaryNumericPromotionDecision _unaryNumericPromotionDecision = null;

        internal UnaryExpressionEmitter(UnaryOperatorExpression unaryOperatorExpression,
                                        ILGenerator ilGenerator,
                                        IOpCodeIndexer instructionIndexer,
                                        IAstVisitor<ILGenerator, AstNode> visitor,
                                        List<LocalBuilder> locals)
            : base(unaryOperatorExpression, ilGenerator, instructionIndexer, visitor, locals) {

            var typeInformation = Node.Annotation<TypeInformation>() ??
                                  Node.Expression.Annotation<TypeInformation>();

            Type = typeInformation.InferredType.GetActualType();
            _unaryNumericPromotionDecision = UnaryNumericPromotionDecision.Decide(Type, unaryOperatorExpression.Operator);
        }

        public override AstNode Emit() {
            var nodeDecorator = Node.Expression.AcceptVisitor(ILGenerator, Visitor);
            var checkedUnchecked = Node.Annotations.FirstOrDefault();
            var isChecked = checkedUnchecked == AddCheckedBlocks.CheckedAnnotation; 
            var memberReference = nodeDecorator.Node as MemberReferenceExpression;
            var decision = _unaryNumericPromotionDecision.Type;

            if (!_unaryNumericPromotionDecision.IsNotNumericPromotion) {
                if (_unaryNumericPromotionDecision.Type.IsPromoted) {
                    Type = decision.To;
                    ILGenerator.EmitConversion(decision.From, decision.To);
                }
            }

            switch (Node.Operator) {
                case UnaryOperatorType.Not:

                    ILGenerator.Emit(OpCodes.Ldc_I4_0);
                    ILGenerator.Emit(OpCodes.Ceq);
                    break;

                case UnaryOperatorType.BitNot:

                    ILGenerator.Emit(OpCodes.Not);
                    break;

                case UnaryOperatorType.Minus:

                    ILGenerator.Emit(OpCodes.Neg);
                    break;

                case UnaryOperatorType.PostIncrement:

                    ILGenerator.EmitPostIncrement(Type, isChecked);
                    _storeArgumentShouldBeEmitted = true;
                    break;

                case UnaryOperatorType.PostDecrement:

                    ILGenerator.EmitPostDecrement(Type, isChecked);
                    _storeArgumentShouldBeEmitted = true;
                    break;

                case UnaryOperatorType.Await:

                    throw new NotSupportedException();
            }

            if (_storeArgumentShouldBeEmitted) {
                EmitStoreByArgumentIfNeeded(memberReference);
            }

            return base.Emit();
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
                var variable = Node.Expression.Annotation<ILVariable>();
                ILGenerator.EmitStoreArgument(variable.OriginalParameter.Sequence);
            }
        }
    }
}