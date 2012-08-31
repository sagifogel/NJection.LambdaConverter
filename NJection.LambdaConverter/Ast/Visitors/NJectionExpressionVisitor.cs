using System.Linq.Expressions;
using NJection.LambdaConverter.Expressions;

namespace NJection.LambdaConverter.Visitors
{
    public class NJectionExpressionVisitor : ExpressionVisitor
    {
        public Expression Visit(AstExpression node) {
            if (node == null) {
                return node;
            }

            return node.Accept(this);
        }

        protected override Expression VisitExtension(Expression node) {
            var astCompiler = node as AstExpression;

            if (astCompiler != null) {
                return Visit(astCompiler);
            }

            return base.VisitExtension(node);
        }

        protected internal Expression VisitBlock(Block block) {
            return block.Update(block.Variables, block.Expressions);
        }

        protected internal Expression VisitMethodDeclarationBlock(MethodDeclaration methodBlock) {
            return methodBlock.Update(methodBlock.Body, methodBlock.Parameters);
        }

        protected internal Expression VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration) {
            return constructorDeclaration.Update(constructorDeclaration.Body, constructorDeclaration.Parameters);
        }

        protected internal Expression VisitInvocationExpression(Invocation invocation) {
            return invocation.Update(invocation.Member, invocation.MethodArguments, invocation.Instance);
        }

        protected internal Expression VisitSwitchStatement(Switch switchStatement) {
            return switchStatement.Update(switchStatement.SwitchValue, switchStatement.Cases, switchStatement.DefaultBody);
        }

        protected internal Expression VisitSwitchCase(Case switchCase) {
            return switchCase.Update(switchCase.TestValue);
        }

        protected internal Expression VisitReturnExpression(Return returnExpression) {
            return returnExpression.Update(returnExpression.Kind, returnExpression.Target, returnExpression.Value);
        }

        protected internal Expression VisitCaseLabel(CaseLabel label) {
            return label.Update(label.Target, label.DefaultValue);
        }

        protected internal Expression VisitMemberReference(MemberReference memberReference) {
            return memberReference.Update(memberReference.Member, memberReference.Expression);
        }

        protected internal Expression VisitVaraibale(VariableDeclaration varaibale) {
            return varaibale.Update(varaibale.Type, varaibale.Name);
        }

        protected internal Expression VisitParameter(ParameterDecalration parameter) {
            return parameter.Update(parameter.Type, parameter.Name);
        }

        protected internal Expression VisitAssign(Assign assign) {
            return assign.Update(assign.Left, assign.Right);
        }

        protected internal Expression VisitTryCatch(TryCatch tryCatch) {
            return tryCatch.Update(tryCatch.Body, tryCatch.Handlers, tryCatch.Finally);
        }

        protected internal Expression VisitBreak(Break @break) {
            return @break.Update(@break.Target, @break.Type, @break.Value);
        }

        protected internal Expression VisitInit(Init init) {
            return init.Update(init.Target, init.Type, init.Value);
        }

        protected internal Expression VisitIdentifier(Identifier identifer) {
            return identifer.Update(identifer.Expression);
        }

        protected internal Expression VisitPrimitive(Primitive primitive) {
            return primitive.Update(primitive.Value, primitive.Type);
        }

        protected internal Expression VisitDirection(Direction direction) {
            return direction.Update(direction.IdentifierParameter, direction.OutParameter);
        }

        protected internal Expression VisitCondition(Conditional conditional) {
            return conditional.Update(conditional.Test, conditional.IfTrue, conditional.IfFalse);
        }

        protected internal Expression VisitUnary(Unary unary) {
            return unary.Update(unary.Operand, unary.Type);
        }

        protected internal Expression VisitBinary(Binary binary) {
            return binary.Update(binary.Left, binary.Right);
        }

        protected internal Expression VisitForEach(ForEach forEach) {
            return forEach.Update(forEach.Body, forEach.BreakExpression, forEach.ContinueExpression);
        }

        protected internal Expression VisitTypeReference(TypeReference typeReference) {
            return typeReference.Update(typeReference.Type);
        }

        protected internal Expression VisitTypeOf(TypeOf typeOf) {
            return typeOf.Update(typeOf.Type);
        }

        protected internal Expression VisitType(TypeExpression type) {
            return type.Update(type.Type);
        }

        protected internal Expression VisitNew(New newExpression) {
            return newExpression.Update(newExpression.Expression);
        }

        protected internal Expression VisitWhile(While @while) {
            return @while.Update(@while.Body, @while.ConditionExpression);
        }

        protected internal Expression VisitTypeAsExpression(TypeAs typeAsExpression) {
            return typeAsExpression.Update(typeAsExpression.Expression, typeAsExpression.Type);
        }

        protected internal Expression VisitNullReference(NullReference nullReference) {
            return nullReference.Update(nullReference.Expression);
        }

        protected internal Expression VisitCast(Cast cast) {
            return cast.Update(cast.Operand, cast.Type);
        }

        protected internal Expression VisitArrayInitializer(ArrayInitializer arrayInitializer) {
            return arrayInitializer.Update(arrayInitializer.Initializers);
        }

        protected internal Expression VisitCompilerGeneratedEnumerator(CompilerGeneratedEnumerator enumerator) {
            return enumerator.Update(enumerator.Enumerator);
        }

        protected internal Expression VisitArrayCreation(ArrayCreation arrayCreation) {
            return arrayCreation.Update(arrayCreation.Initializers, arrayCreation.Bounds);
        }

        protected internal Expression VisitIndex(Index index) {
            return index.Update(index.Arguments);
        }

        protected internal Expression VisitCatchClause(CatchClause catchClause) {
            return catchClause.Update(catchClause.Body, catchClause.ExceptionVariable, catchClause.Filter);
        }

        protected internal Expression VisitThrowStatement(Throw throwStatement) {
            return throwStatement.Update(throwStatement.Value, throwStatement.Type);
        }

        protected internal Expression VisitEmpty(Empty empty) {
            return empty.Update();
        }

        protected internal Expression VisitIfElseCondition(IfElseCondition ifElseCondition) {
            return ifElseCondition.Update(ifElseCondition.Test, ifElseCondition.IfTrue, ifElseCondition.IfFalse);
        }

        protected internal Expression VisitNamedArgument(NamedArgument namedArgument) {
            return namedArgument.Update(namedArgument.Argument);
        }

        protected internal Expression VisitDefault(Default @default) {
            return @default.Update(@default.Type);
        }

        protected internal Expression VisitGoto(Goto @goto) {
            return @goto.Update(@goto.LabelTarget, @goto.Type);
        }

        protected internal Expression VisitContinue(Continue @continue) {
            return @continue.Update(@continue.LabelTarget, @continue.Type);
        }

        protected internal Expression VisitLabel(Label label) {
            return label.Update(label.LabelTarget, label.Type);
        }

        protected internal Expression VisitThis(This @this) {
            return @this.Update(@this.Context.Expression, @this.Type);
        }

        protected internal Expression VisitBase(Base @base) {
            return @base.Update(@base.Context, @base.Type);
        }

        protected internal Expression VisitLambda(Lambda lambda) {
            return lambda.Update(lambda.Body, lambda.Type, lambda.Parameters);
        }

        protected internal Expression VisitEvent(Event @event) {
            return @event.Update(@event.Target, @event.Type, @event.Operator);
        }

        protected internal Expression VisitAnonymousTypeCreation(AnonymousType anonymousType) {
            return anonymousType.Update(anonymousType.Type);
        }

        protected internal Expression VisitNamedExpression(NamedExpression namedExpression) {
            return namedExpression.Update(namedExpression.Expression);
        }

        protected internal Expression VisitMethodOf(MethodOf methodOf) {
            return methodOf.Update(methodOf.Member, methodOf.Type);
        }
    }
}