using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class AssignmentExpressionEmitter : AbstractDepthFirstVisitorEmitter<AssignmentExpression>
    {
        internal AssignmentExpressionEmitter(AssignmentExpression assignmentExpression,
                                             ILGenerator ilGenerator,
                                             IOpCodeIndexer instructionIndexer,
                                             IAstVisitor<ILGenerator, AstNode> visitor,
                                             List<LocalBuilder> locals)
            : base(assignmentExpression, ilGenerator, instructionIndexer, visitor, locals) { }

        public override AstNode Emit() {
            if (IgnoreLeftOperand()) {
                Node.Right.AcceptVisitor(Visitor, ILGenerator);
            }
            else {
                var memberReference = Node.Left as MemberReferenceExpression;

                if (memberReference != null) {
                    return new MultipuleAssignmentEmitter(memberReference, Node.Right, ILGenerator, InstructionsIndexer, Visitor, Locals).Emit();
                }

                var instruction = InstructionsIndexer.GetLastInstructionInRange(Node);

                Node.Right.AcceptVisitor(Visitor, ILGenerator);
                Node.Left.AcceptVisitor(Visitor, ILGenerator);

                if (instruction.Operand != null) {
                    var methodReference = instruction.Operand as MethodReference;

                    if (methodReference != null) {
                        var methodInfo = methodReference.GetActualMethod<MethodInfo>();
                        ILGenerator.Emit(OpCodes.Call, methodInfo);
                    }
                }
            }

            return new AstNodeDecorator(Node, null);
        }

        private bool IgnoreLeftOperand() {
            var rightOperand = Node.Right;

            return !(Node.Left is MemberReferenceExpression) ||
                   rightOperand is ArrayCreateExpression ||
                   rightOperand is ObjectCreateExpression ||
                   rightOperand is BinaryOperatorExpression;
        }
    }
}