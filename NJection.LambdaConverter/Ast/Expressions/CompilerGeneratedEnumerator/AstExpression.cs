using Mono.Cecil.Cil;
using NJection.LambdaConverter.Visitors;

namespace NJection.LambdaConverter.Expressions
{
    public abstract partial class AstExpression
    {
        public static CompilerGeneratedEnumerator CompilerGeneratedEnumerator(Instruction stateInitializer, Instruction newObj, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return new CompilerGeneratedEnumerator(stateInitializer, newObj);
        }
    }
}