using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static Assign Assign(NRefactory.AssignmentExpression assignmentExpression, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new Assign(assignmentExpression, scope, visitor);
        }
    }
}