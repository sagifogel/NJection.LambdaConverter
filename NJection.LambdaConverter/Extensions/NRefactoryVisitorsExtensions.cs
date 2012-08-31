using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Extensions
{
    public static class NRefactoryVisitorsExtensions
    {
        public static MethodDeclaration AcceptVisitor<T>(this MethodDeclaration methodDeclaration, T data, IAstVisitor<T, AstNode> visitor) {
            return visitor.VisitMethodDeclaration(methodDeclaration, data) as MethodDeclaration;
        }
    }
}