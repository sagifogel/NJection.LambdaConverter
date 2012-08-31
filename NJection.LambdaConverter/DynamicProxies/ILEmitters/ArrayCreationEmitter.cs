using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using Cil = Mono.Cecil.Cil;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class ArrayCreationEmitter : AbstractDepthFirstVisitorEmitter<ArrayCreateExpression>
    {
        internal ArrayCreationEmitter(ArrayCreateExpression arrayObjectCreateExpression,
                                      ILGenerator ilGenerator,
                                      IOpCodeIndexer instructionIndexer,
                                      IAstVisitor<ILGenerator, AstNode> visitor,
                                      List<LocalBuilder> locals)
            : base(arrayObjectCreateExpression, ilGenerator, instructionIndexer, visitor, locals) {
            Type = arrayObjectCreateExpression.GetArrayType();
        }

        public override AstNode Emit() {
            if (!Node.Initializer.IsNull) {
                EmitArrayCreationWithInitializer();
            }
            else {
                EmitEmptyArrayCreation();
            }

            return new AstNodeDecorator(Node, Type);
        }

        private void EmitArrayCreationWithInitializer() {
            var localBuilder = ILGenerator.DeclareLocal(Type);

            Locals.Add(localBuilder);
            Node.Initializer.AcceptVisitor(Visitor, ILGenerator);
            Node.Arguments.ForEach(arg => arg.AcceptVisitor(this, ILGenerator));

            if (TryEmitNewObject()) {
                ILGenerator.EmitStoreLocal(localBuilder);
            }
        }

        private void EmitEmptyArrayCreation() {
            Node.Arguments.ForEach(arg => arg.AcceptVisitor(this, ILGenerator));

            if (!TryEmitNewObject()) {
                ILGenerator.Emit(OpCodes.Newarr, Type.GetElementType());
            }
        }

        private bool TryEmitNewObject() {
            Cil.Instruction instruction;

            if (InstructionsIndexer.TryGetNewObjectInstruction(Node, out instruction)) {
                var methodReference = instruction.Operand as MethodReference;
                var constructorInfo = methodReference.GetActualMethod<ConstructorInfo>();

                ILGenerator.Emit(OpCodes.Newobj, constructorInfo);
                return true;
            }

            return false;
        }

        public override AstNode VisitExpressionStatement(ExpressionStatement expressionStatement, ILGenerator data) {
            return base.VisitExpressionStatement(expressionStatement, data);
        }

        public override AstNode VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, ILGenerator data) {
            return primitiveExpression.AcceptVisitor(Visitor, data);
        }

        public override AstNode VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILGenerator data) {
            return base.VisitArrayInitializerExpression(arrayInitializerExpression, data);
        }

        public override AstNode VisitArrayCreateExpression(ArrayCreateExpression arrayObjectCreateExpression, ILGenerator data) {
            return base.VisitArrayCreateExpression(arrayObjectCreateExpression, data);
        }

        public override AstNode VisitArraySpecifier(ArraySpecifier arraySpecifier, ILGenerator data) {
            return base.VisitArraySpecifier(arraySpecifier, data);
        }

        public override AstNode VisitAssignmentExpression(AssignmentExpression assignmentExpression, ILGenerator data) {
            return base.VisitAssignmentExpression(assignmentExpression, data);
        }

        public override AstNode VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, ILGenerator data) {
            return base.VisitVariableDeclarationStatement(variableDeclarationStatement, data);
        }
    }
}