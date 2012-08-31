using System.Collections.Generic;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal sealed class ConstructorEmitter : DepthFirstAstVisitor<ILGenerator, AstNode>
    {
        private List<LocalBuilder> _locals = null;
        private IOpCodeIndexer _instructionIndexer = null;
        private IAstVisitor<ILGenerator, AstNode> _visitor = null;

        internal ConstructorEmitter(IAstVisitor<ILGenerator, AstNode> visitor, IOpCodeIndexer instructionsIndexer, List<LocalBuilder> locals) {
            _locals = locals;
            _visitor = visitor;
            _instructionIndexer = instructionsIndexer;
        }

        protected override AstNode VisitChildren(AstNode node, ILGenerator data) {
            AstNode nextSibling;

            for (AstNode node3 = node.FirstChild; node3 != null; node3 = nextSibling) {
                nextSibling = node3.NextSibling;
                node3.AcceptVisitor<ILGenerator, AstNode>(_visitor, data);
            }

            return default(AstNode);
        }

        internal AstNode EmitThisReference(ConstructorDeclaration constructorDeclaration, ILGenerator ilGenerator) {
            return new ThisExpressionEmitter(constructorDeclaration, ilGenerator, _instructionIndexer, _visitor).Emit();
        }

        internal AstNode EmitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, ILGenerator ilGenerator) {
            return new ThisExpressionEmitter(thisReferenceExpression, ilGenerator, _instructionIndexer, _visitor).Emit();
        }

        internal AstNode EmitArgumentReferenceExpression(IdentifierExpression identifierExpression, ParameterDefinition parameterDefinition, ILGenerator ilGenerator) {
            return new ArgumentReferenceEmitter(identifierExpression, parameterDefinition, ilGenerator, _instructionIndexer).Emit();
        }

        internal AstNode EmitInvocationExpression(InvocationExpression invocationExpression, ILGenerator ilGenerator) {
            return new MethodReferenceReflectionEmitter(invocationExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitReturnStatement(ReturnStatement returnStatement, ILGenerator ilGenerator) {
            return new ReturnStatementEmitter(returnStatement, ilGenerator).Emit();
        }

        internal AstNode EmitObjectCreation(ObjectCreateExpression objectCreateExpression, ILGenerator ilGenerator) {
            return new NewObjectEmitter(objectCreateExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, ILGenerator ilGenerator) {
            return new MemberReferenceReflectionEmitter(memberReferenceExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitPrimitiveExpression(PrimitiveExpression primitiveExpression, ILGenerator ilGenerator) {
            return new PrimitiveEmitter(primitiveExpression, ilGenerator, _instructionIndexer).Emit();
        }

        internal AstNode EmitDefaultValueExpression(DefaultValueExpression defaultValueExpression, ILGenerator ilGenerator) {
            return new DefaultValueEmitter(defaultValueExpression, ilGenerator, _instructionIndexer).Emit();
        }

        internal AstNode EmitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, ILGenerator ilGenerator) {
            return new NullReferenceEmitter(nullReferenceExpression, ilGenerator, _instructionIndexer).Emit();
        }

        internal AstNode EmitCastExpression(CastExpression castExpression, ILGenerator ilGenerator) {
            return new CastExpressionEmitter(castExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitArrayCreationExpression(ArrayCreateExpression arrayObjectCreateExpression, ILGenerator ilGenerator) {
            return new ArrayCreationEmitter(arrayObjectCreateExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILGenerator ilGenerator) {
            return new ArrayInitializerEmitter(arrayInitializerExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitAssignmentExpression(AssignmentExpression assignmentExpression, ILGenerator ilGenerator) {
            return new AssignmentExpressionEmitter(assignmentExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitLocalReferenceExpression(IdentifierExpression identifierExpressionn, ILGenerator ilGenerator) {
            return new LocalReferenceEmitter(identifierExpressionn, ilGenerator, _instructionIndexer, _locals).Emit();
        }

        internal AstNode EmitTypeOfExpression(TypeOfExpression typeOfExpression, ILGenerator ilGenerator) {
            return new TypeOfExpressionEmitter(typeOfExpression, ilGenerator, _instructionIndexer).Emit();
        }

        internal AstNode EmitAsExpression(AsExpression asExpression, ILGenerator ilGenerator) {
            return new AsExpressionEmitter(asExpression, ilGenerator, _instructionIndexer, _visitor).Emit();
        }

        internal AstNode EmitAnonymousTypeCreationExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, ILGenerator ilGenerator) {
            return new AnonymousTypeEmitter(anonymousTypeCreateExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitNamedExpression(NamedExpression namedExpression, ILGenerator ilGenerator) {
            return new NamedExpressionEmitter(namedExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitIndexerExpression(IndexerExpression indexerExpression, ILGenerator ilGenerator) {
            return new IndexerExpressionEmitter(indexerExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, ILGenerator ilGenerator) {
            return new UnaryExpressionEmitter(unaryOperatorExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorEmitter, ILGenerator ilGenerator) {
            return new BinaryExpressionEmitter(binaryOperatorEmitter, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitDirectionExpression(DirectionExpression directionExpression, ILGenerator ilGenerator) {
            return new DirectionExpressionEmitter(directionExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

        internal AstNode EmitConditionalExpression(ConditionalExpression conditionalExpression, ILGenerator ilGenerator) {
            return new ConditionalExpressionEmitter(conditionalExpression, ilGenerator, _instructionIndexer, _visitor, _locals).Emit();
        }

		internal AstNode EmitIsExpression(IsExpression isExpression, ILGenerator ilGenerator) {
			return new IsExpressionEmitter(isExpression, ilGenerator, _instructionIndexer, _visitor).Emit();
		}
	}
}