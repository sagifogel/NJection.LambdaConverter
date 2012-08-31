using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class TypeOfExpressionEmitter : AbstractILEmitter
    {
        private TypeOfExpression _typeOfExpression = null;

        internal TypeOfExpressionEmitter(TypeOfExpression typeOfExpression,
                                         ILGenerator ilGenerator,
                                         IOpCodeIndexer instructionsIndexer)
            : base(ilGenerator, instructionsIndexer) {
            _typeOfExpression = typeOfExpression;
        }

        public override AstNode Emit() {
            Type = _typeOfExpression.Type.GetActualType();
            ILGenerator.Emit(OpCodes.Ldtoken, Type);

            return new AstNodeDecorator(_typeOfExpression, Type);
        }
    }
}