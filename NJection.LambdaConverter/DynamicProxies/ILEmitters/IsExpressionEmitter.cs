using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
	internal class IsExpressionEmitter : AbstractDepthFirstVisitorEmitter<IsExpression>
    {
		internal IsExpressionEmitter(IsExpression isExpression,
                                     ILGenerator ilGenerator,
                                     IOpCodeIndexer instructionsIndexer,
                                     IAstVisitor<ILGenerator, AstNode> visitor)
            : base(isExpression, ilGenerator, instructionsIndexer, visitor) {
            Type = isExpression.Type.GetActualType();
        }

        public override AstNode Emit() {
            Node.Expression.AcceptVisitor(Visitor, ILGenerator);
            ILGenerator.Emit(OpCodes.Isinst, Type);
			ILGenerator.Emit(OpCodes.Ldnull);
			ILGenerator.Emit(OpCodes.Cgt_Un);

            return base.Emit();
        }
    }
}