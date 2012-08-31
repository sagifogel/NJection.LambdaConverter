using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class AsExpressionEmitter : AbstractDepthFirstVisitorEmitter<AsExpression>
    {
        internal AsExpressionEmitter(AsExpression asExpression,
                                     ILGenerator ilGenerator,
                                     IOpCodeIndexer instructionsIndexer,
                                     IAstVisitor<ILGenerator, AstNode> visitor)
            : base(asExpression, ilGenerator, instructionsIndexer, visitor) {
            Type = asExpression.Type.GetActualType();
        }

        public override AstNode Emit() {
            Node.Expression.AcceptVisitor(Visitor, ILGenerator);
            ILGenerator.Emit(OpCodes.Isinst, Type);

            return base.Emit();
        }
    }
}