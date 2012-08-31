using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using System.Reflection.Emit;
using ICSharpCode.Decompiler.Ast;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class ConditionalExpressionEmitter : AbstractDepthFirstVisitorEmitter<ConditionalExpression>
    {
        private Label _conditionLabel;

        internal ConditionalExpressionEmitter(ConditionalExpression conditionalExpression,
                                              ILGenerator ilGenerator,
                                              IOpCodeIndexer instructionIndexer,
                                              IAstVisitor<ILGenerator, AstNode> visitor,
                                              List<LocalBuilder> locals)
            : base(conditionalExpression, ilGenerator, instructionIndexer, visitor, locals) {

            _conditionLabel = ILGenerator.DefineLabel();

            Type = conditionalExpression.Annotation<TypeInformation>()
                                        .InferredType
                                        .GetActualType();
        }

        public override AstNode Emit() {
            var falseLabel = ILGenerator.DefineLabel();
            var trueLabel = ILGenerator.DefineLabel();
            AstNodeDecorator falseExpressionDecorator = null;

            Node.Condition.AcceptVisitor(ILGenerator, Visitor);
            ILGenerator.Emit(OpCodes.Brtrue_S, trueLabel);
            falseExpressionDecorator = Node.FalseExpression.AcceptVisitor(ILGenerator, Visitor);
            ILGenerator.EmitCastIfNeeded(falseExpressionDecorator.Type, Type);
            ILGenerator.Emit(OpCodes.Br_S, falseLabel);
            ILGenerator.MarkLabel(trueLabel);
            Node.TrueExpression.AcceptVisitor(ILGenerator, Visitor);
            ILGenerator.MarkLabel(falseLabel);

            return base.Emit();
        }
    }
}
