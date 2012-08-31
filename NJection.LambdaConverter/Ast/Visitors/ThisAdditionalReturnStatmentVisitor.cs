using System;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Visitors
{
    internal class ThisAdditionalReturnStatmentVisitor : DepthFirstAstVisitor<object, AstNode>
    {
        public override AstNode VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data) {
            ThisReferenceExpression thisExpression = new ThisReferenceExpression();
            ReturnStatement statement = new ReturnStatement(thisExpression);
            Role<Statement> role = new Role<Statement>("Statement");

            Statement returnStatement = statement.Clone();
            constructorDeclaration.Body.Add(statement);
            constructorDeclaration.AddChild(returnStatement, role);

            return base.VisitConstructorDeclaration(constructorDeclaration, data);
        }

        public override AstNode VisitExpressionStatement(ExpressionStatement expressionStatement, object data) {
            var expression = expressionStatement.Expression;
            var invocation = expression as InvocationExpression;

            if (invocation != null) {
                var memberReference = invocation.Target as MemberReferenceExpression;

                if (memberReference != null) {
                    if (memberReference.MemberName.Equals(".ctor", StringComparison.OrdinalIgnoreCase)) {
                        invocation.Parent.Remove();
                    }
                }
            }

            return expressionStatement;
        }
    }
}