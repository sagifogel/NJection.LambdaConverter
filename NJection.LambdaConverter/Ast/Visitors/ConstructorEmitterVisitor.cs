using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.DynamicProxies.ILEmitters;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.Visitors
{
    internal class ConstructorEmitterVisitor : DepthFirstAstVisitor<ILGenerator, AstNode>
    {
        private ConstructorDeclaration _root = null;
        private ConstructorEmitter _constructorEmitter = null;
        private IInstructionsIndexer _instructionsIndexer = null;
        private List<LocalBuilder> _locals = new List<LocalBuilder>();

        internal ConstructorEmitterVisitor(ConstructorDeclaration constructorDeclaration, IOpCodeIndexer instructionsIndexer) {
            _instructionsIndexer = instructionsIndexer;
            _constructorEmitter = new ConstructorEmitter(this, instructionsIndexer, _locals);
            _root = constructorDeclaration;
        }

        public override AstNode VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, ILGenerator data) {
            return _constructorEmitter.EmitThisReference(constructorDeclaration, data);
        }

        public override AstNode VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILGenerator data) {
            return _constructorEmitter.EmitArrayInitializerExpression(arrayInitializerExpression, data);
        }

        public override AstNode VisitArrayCreateExpression(ArrayCreateExpression arrayObjectCreateExpression, ILGenerator data) {
            return _constructorEmitter.EmitArrayCreationExpression(arrayObjectCreateExpression, data);
        }

        public override AstNode VisitReturnStatement(ReturnStatement returnStatement, ILGenerator data) {
            return _constructorEmitter.EmitReturnStatement(returnStatement, data);
        }

        public override AstNode VisitInvocationExpression(InvocationExpression invocationExpression, ILGenerator data) {
            return _constructorEmitter.EmitInvocationExpression(invocationExpression, data);
        }

        public override AstNode VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, ILGenerator data) {
            return _constructorEmitter.EmitObjectCreation(objectCreateExpression, data);
        }

        public override AstNode VisitAssignmentExpression(AssignmentExpression assignmentExpression, ILGenerator data) {
            return _constructorEmitter.EmitAssignmentExpression(assignmentExpression, data);
        }

        public override AstNode VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, ILGenerator data) {
            return _constructorEmitter.EmitMemberReferenceExpression(memberReferenceExpression, data);
        }

        public override AstNode VisitIdentifierExpression(IdentifierExpression identifierExpression, ILGenerator data) {
            var parameter = _root.Parameters.FirstOrDefault(p => p.Name.Equals(identifierExpression.Identifier));

            if (parameter != null) {
                return _constructorEmitter.EmitArgumentReferenceExpression(identifierExpression, parameter.Annotation<ParameterDefinition>(), data);
            }

            return _constructorEmitter.EmitLocalReferenceExpression(identifierExpression, data);
        }

        public override AstNode VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, ILGenerator data) {
            return _constructorEmitter.EmitPrimitiveExpression(primitiveExpression, data);
        }

        public override AstNode VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, ILGenerator data) {
            return _constructorEmitter.EmitDefaultValueExpression(defaultValueExpression, data);
        }

        public override AstNode VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, ILGenerator data) {
            return _constructorEmitter.EmitNullReferenceExpression(nullReferenceExpression, data);
        }

        public override AstNode VisitCastExpression(CastExpression castExpression, ILGenerator data) {
            return _constructorEmitter.EmitCastExpression(castExpression, data);
        }

        public override AstNode VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, ILGenerator data) {
            Type type = variableDeclarationStatement.Type.GetActualType();

            _locals.Add(data.DeclareLocal(type));

            return base.VisitVariableDeclarationStatement(variableDeclarationStatement, data);
        }

        public override AstNode VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, ILGenerator data) {
            return _constructorEmitter.EmitThisReferenceExpression(thisReferenceExpression, data);
        }

        public override AstNode VisitTypeOfExpression(TypeOfExpression typeOfExpression, ILGenerator data) {
            return _constructorEmitter.EmitTypeOfExpression(typeOfExpression, data);
        }

        public override AstNode VisitAsExpression(AsExpression asExpression, ILGenerator data) {
            return _constructorEmitter.EmitAsExpression(asExpression, data);
        }

        public override AstNode VisitAnonymousTypeCreateExpression(AnonymousTypeCreateExpression anonymousTypeCreateExpression, ILGenerator data) {
            return _constructorEmitter.EmitAnonymousTypeCreationExpression(anonymousTypeCreateExpression, data);
        }

        public override AstNode VisitNamedExpression(NamedExpression namedExpression, ILGenerator data) {
            return _constructorEmitter.EmitNamedExpression(namedExpression, data);
        }

        public override AstNode VisitIndexerExpression(IndexerExpression indexerExpression, ILGenerator data) {
            return _constructorEmitter.EmitIndexerExpression(indexerExpression, data);
        }

        public override AstNode VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, ILGenerator data) {
            return _constructorEmitter.EmitUnaryOperatorExpression(unaryOperatorExpression, data);
        }

        public override AstNode VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, ILGenerator data) {
            return _constructorEmitter.EmitBinaryOperatorExpression(binaryOperatorExpression, data);
        }

        public override AstNode VisitDirectionExpression(DirectionExpression directionExpression, ILGenerator data) {
            return _constructorEmitter.EmitDirectionExpression(directionExpression, data);
        }

        public override AstNode VisitConditionalExpression(ConditionalExpression conditionalExpression, ILGenerator data) {
            return _constructorEmitter.EmitConditionalExpression(conditionalExpression, data);
        }

        public override AstNode VisitIsExpression(IsExpression isExpression, ILGenerator data) {
			return _constructorEmitter.EmitIsExpression(isExpression, data);
        }
    }
}