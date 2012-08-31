using System;
using NJection.LambdaConverter.Expressions;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Visitors
{
    internal class NRefactoryAstVisitor : AbstractNRefcatoryAstVisitor
    {
        public override AstExpression VisitArrayCreateExpression(NRefactory.ArrayCreateExpression arrayCreateExpression, IScope scope) {
            return AstExpression.ArrayCreation(arrayCreateExpression, scope, this);
        }

        public override AstExpression VisitArrayInitializerExpression(NRefactory.ArrayInitializerExpression arrayInitializerExpression, IScope scope) {
            return AstExpression.ArrayInitializer(arrayInitializerExpression, scope, this);
        }

        public override AstExpression VisitAsExpression(NRefactory.AsExpression asExpression, IScope scope) {
            return AstExpression.TypeAs(asExpression, scope, this);
        }

        public override AstExpression VisitAssignmentExpression(NRefactory.AssignmentExpression assignmentExpression, IScope scope) {
            return AstExpression.Assign(assignmentExpression, scope, this);
        }

        public override AstExpression VisitBaseReferenceExpression(NRefactory.BaseReferenceExpression baseReferenceExpression, IScope scope) {
            return AstExpression.Base(baseReferenceExpression, scope, this);
        }

        public override AstExpression VisitBinaryOperatorExpression(NRefactory.BinaryOperatorExpression binaryOperatorExpression, IScope scope) {
            return AstExpression.Binary(binaryOperatorExpression, scope, this);
        }

        public override AstExpression VisitBlockStatement(NRefactory.BlockStatement blockStatement, IScope scope) {
            return AstExpression.Block(blockStatement, scope, this);
        }

        public override AstExpression VisitBreakStatement(NRefactory.BreakStatement breakStatement, IScope scope) {
            return AstExpression.Break(breakStatement, scope, this);
        }

        public override AstExpression VisitCaseLabel(NRefactory.CaseLabel caseLabel, IScope scope) {
            return AstExpression.SwitchCase(caseLabel, scope, this);
        }

        public override AstExpression VisitCastExpression(NRefactory.CastExpression castExpression, IScope scope) {
            return AstExpression.Cast(castExpression, scope, this);
        }

        public override AstExpression VisitCatchClause(NRefactory.CatchClause catchClause, IScope scope) {
            return AstExpression.CatchClause(catchClause, scope, this);
        }

        public override AstExpression VisitComposedType(NRefactory.ComposedType composedType, IScope scope) {
            return AstExpression.TypeExpression(composedType, scope, this);
        }

        public override AstExpression VisitConditionalExpression(NRefactory.ConditionalExpression conditionalExpression, IScope scope) {
            return AstExpression.Condition(conditionalExpression, scope, this);
        }

        public override AstExpression VisitContinueStatement(NRefactory.ContinueStatement continueStatement, IScope scope) {
            return AstExpression.Continue(continueStatement, scope, this);
        }

        public override AstExpression VisitDefaultValueExpression(NRefactory.DefaultValueExpression defaultValueExpression, IScope scope) {
            return AstExpression.Default(defaultValueExpression, scope, this);
        }

        public override AstExpression VisitDirectionExpression(NRefactory.DirectionExpression directionExpression, IScope scope) {
            return AstExpression.Direction(directionExpression, scope, this);
        }

        public override AstExpression VisitEmptyExpression(NRefactory.EmptyExpression emptyExpression, IScope scope) {
            return AstExpression.Empty(emptyExpression, scope, this);
        }

        public override AstExpression VisitExpressionStatement(NRefactory.ExpressionStatement expressionStatement, IScope scope) {
            return expressionStatement.Expression.AcceptVisitor(this, scope);
        }

        public override AstExpression VisitGotoStatement(NRefactory.GotoStatement gotoStatement, IScope scope) {
            return AstExpression.Goto(gotoStatement, scope, this);
        }

        public override AstExpression VisitIdentifierExpression(NRefactory.IdentifierExpression identifierExpression, IScope scope) {
            return AstExpression.Identifer(identifierExpression, scope, this);
        }

        public override AstExpression VisitIfElseStatement(NRefactory.IfElseStatement ifElseStatement, IScope scope) {
            return AstExpression.IfElseCondition(ifElseStatement, scope, this);
        }

        public override AstExpression VisitIndexerExpression(NRefactory.IndexerExpression indexerExpression, IScope scope) {
            return AstExpression.Index(indexerExpression, scope, this);
        }

        public override AstExpression VisitInvocationExpression(NRefactory.InvocationExpression invocationExpression, IScope scope) {
            return AstExpression.Invocation(invocationExpression, scope, this);
        }

        public override AstExpression VisitLabelStatement(NRefactory.LabelStatement labelStatement, IScope scope) {
            return AstExpression.Label(labelStatement, scope, this);
        }

        public override AstExpression VisitMemberReferenceExpression(NRefactory.MemberReferenceExpression memberReferenceExpression, IScope scope) {
            return AstExpression.MemberReference(memberReferenceExpression, scope, this);
        }

        public override AstExpression VisitMemberType(NRefactory.MemberType memberType, IScope scope) {
            return AstExpression.TypeExpression(memberType, scope, this);
        }

        public override AstExpression VisitMethodDeclaration(NRefactory.MethodDeclaration methodDeclaration, IScope scope) {
            throw new NotImplementedException();
        }

        public override AstExpression VisitNamedArgumentExpression(NRefactory.NamedArgumentExpression namedArgumentExpression, IScope scope) {
            return AstExpression.NamedArgument(namedArgumentExpression, scope, this);
        }

        public override AstExpression VisitNullReferenceExpression(NRefactory.NullReferenceExpression nullReferenceExpression, IScope scope) {
            return AstExpression.NullReference(nullReferenceExpression, scope, this);
        }

        public override AstExpression VisitObjectCreateExpression(NRefactory.ObjectCreateExpression objectCreateExpression, IScope scope) {
            return AstExpression.New(objectCreateExpression, scope, this);
        }

        public override AstExpression VisitParameterDeclaration(NRefactory.ParameterDeclaration parameterDeclaration, IScope scope) {
            return AstExpression.Parameter(parameterDeclaration, scope, this);
        }

        public override AstExpression VisitPrimitiveExpression(NRefactory.PrimitiveExpression primitiveExpression, IScope scope) {
            return AstExpression.Primitive(primitiveExpression, scope, this);
        }

        public override AstExpression VisitPrimitiveType(NRefactory.PrimitiveType primitiveType, IScope scope) {
            return AstExpression.TypeExpression(primitiveType, scope, this);
        }

        public override AstExpression VisitReturnStatement(NRefactory.ReturnStatement returnStatement, IScope scope) {
            return AstExpression.Return(returnStatement, scope, this);
        }

        public override AstExpression VisitSimpleType(NRefactory.SimpleType simpleType, IScope scope) {
            return AstExpression.TypeExpression(simpleType, scope, this);
        }

        public override AstExpression VisitSwitchStatement(NRefactory.SwitchStatement switchStatement, IScope scope) {
            return AstExpression.Switch(switchStatement, scope, this);
        }

        public override AstExpression VisitThisReferenceExpression(NRefactory.ThisReferenceExpression thisReferenceExpression, IScope scope) {
            return AstExpression.This(thisReferenceExpression, scope, this);
        }

        public override AstExpression VisitThrowStatement(NRefactory.ThrowStatement throwStatement, IScope scope) {
            return AstExpression.Throw(throwStatement, scope, this);
        }

        public override AstExpression VisitTryCatchStatement(NRefactory.TryCatchStatement tryCatchStatement, IScope scope) {
            return AstExpression.TryCatch(tryCatchStatement, scope, this);
        }

        public override AstExpression VisitTypeOfExpression(NRefactory.TypeOfExpression typeOfExpression, IScope scope) {
            return AstExpression.TypeOf(typeOfExpression, scope, this);
        }

        public override AstExpression VisitTypeReferenceExpression(NRefactory.TypeReferenceExpression typeReferenceExpression, IScope scope) {
            return AstExpression.TypeReference(typeReferenceExpression, scope, this);
        }

        public override AstExpression VisitUnaryOperatorExpression(NRefactory.UnaryOperatorExpression unaryOperatorExpression, IScope scope) {
            return AstExpression.Unary(unaryOperatorExpression, scope, this);
        }

        public override AstExpression VisitVariableDeclarationStatement(NRefactory.VariableDeclarationStatement variableDeclarationStatement, IScope scope) {
            return AstExpression.Variable(variableDeclarationStatement, scope, this);
        }

        public override AstExpression VisitVariableInitializer(NRefactory.VariableInitializer variableInitializer, IScope scope) {
            return AstExpression.Init(variableInitializer, scope, this);
        }

        public override AstExpression VisitWhileStatement(NRefactory.WhileStatement whileStatement, IScope scope) {
            return AstExpression.While(whileStatement, scope, this);
        }

        public override AstExpression VisitAnonymousTypeCreateExpression(NRefactory.AnonymousTypeCreateExpression anonymousTypeCreateExpression, IScope scope) {
            return AstExpression.AnonymousType(anonymousTypeCreateExpression, scope, this);
        }

        public override AstExpression VisitNamedExpression(NRefactory.NamedExpression namedExpression, IScope scope) {
            return AstExpression.NamedExpression(namedExpression, scope, this);
        }
    }
}