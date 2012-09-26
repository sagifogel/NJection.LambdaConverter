using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static TryCatch TryCatch(NRefactory.TryCatchStatement tryCatchStatement, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new TryCatch(tryCatchStatement, scope, visitor);
        }
    }
}