using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class ArgumentReferenceEmitter : AbstractILEmitter
    {
        private ParameterDefinition _parameterDefinition = null;
        private IdentifierExpression _identifierExpression = null;

        internal ArgumentReferenceEmitter(IdentifierExpression identifierExpression, ParameterDefinition parameterDefinition, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer)
            : base(ilGenerator, instructionsIndexer) {

            _parameterDefinition = parameterDefinition;
            _identifierExpression = identifierExpression;
            Type = parameterDefinition.ParameterType.GetActualType();
        }

        public override AstNode Emit() {
            int index = _parameterDefinition.Sequence;

            switch (index) {
                case 1:

                    ILGenerator.Emit(OpCodes.Ldarg_1);
                    break;

                case 2:

                    ILGenerator.Emit(OpCodes.Ldarg_2);
                    break;

                case 3:

                    ILGenerator.Emit(OpCodes.Ldarg_3);
                    break;

                default:

                    ILGenerator.Emit(OpCodes.Ldarg_S, index);
                    break;
            }

            return new AstNodeDecorator(_identifierExpression, Type);
        }
    }
}