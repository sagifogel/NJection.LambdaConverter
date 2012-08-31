using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class NullReferenceEmitter : AbstractILEmitter
    {
        private NullReferenceExpression _nullReferenceExpression = null;

        internal NullReferenceEmitter(NullReferenceExpression nullReferenceExpression, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer)
            : base(ilGenerator, instructionsIndexer) {
            Type = null;
            _nullReferenceExpression = nullReferenceExpression;
        }

        public override AstNode Emit() {
            ILGenerator.Emit(OpCodes.Ldnull);

            return new AstNodeDecorator(_nullReferenceExpression, Type);
        }
    }
}