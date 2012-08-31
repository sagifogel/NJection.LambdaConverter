using System;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class DefaultValueEmitter : AbstractILEmitter
    {
        private DefaultValueExpression _defaultValueExpression = null;

        internal DefaultValueEmitter(DefaultValueExpression defaultValueExpression, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer)
            : base(ilGenerator, instructionsIndexer) {
            _defaultValueExpression = defaultValueExpression;
        }

        public override AstNode Emit() {
            Type = _defaultValueExpression.Type.GetActualType();

            if (Type.IsPrimitive && Type != TypeSystem.IntPtr) {
                object value = Activator.CreateInstance(Type);
                ILGenerator.EmitPrimitiveByTypeCode(value, Type.GetTypeCode(Type));
            }
            else if (Type.IsByRef) {
            }
            else if (Type.IsValueType || Type.IsGenericParameter) {
                var local = ILGenerator.DeclareLocal(Type);

                ILGenerator.Emit(OpCodes.Ldloca_S, local);
                ILGenerator.Emit(OpCodes.Initobj, Type);
                ILGenerator.Emit(OpCodes.Ldloc, local);
            }
            else if (Type.IsClass || Type.IsInterface) {
                ILGenerator.Emit(OpCodes.Ldnull);
            }

            return new AstNodeDecorator(_defaultValueExpression, Type);
        }
    }
}