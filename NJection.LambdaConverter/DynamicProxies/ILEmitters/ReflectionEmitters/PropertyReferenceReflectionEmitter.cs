using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;
using Cecil = Mono.Cecil;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class PropertyReferenceReflectionEmitter : AbstractMemberReferenceEmitter
    {
        private bool _isSetter = false;
        private Action _emitPrivateAction = null;
        private MethodInfo _propertyMethod = null;
        private Cecil.PropertyDefinition _propertyDefinition = null;

        public PropertyReferenceReflectionEmitter(MemberReferenceExpression memberReferenceExpression,
                                                  Type target,
                                                  MemberInfo member,
                                                  ILGenerator ilGenerator,
                                                  IOpCodeIndexer instructionsIndexer,
                                                  IAstVisitor<ILGenerator, AstNode> visitor,
                                                  List<LocalBuilder> locals,
                                                  bool isSetter = false)
            : base(memberReferenceExpression, target, member, ilGenerator, instructionsIndexer, visitor, locals) {

            var propertyInfo = Member as PropertyInfo;

            _isSetter = isSetter;
            _propertyDefinition = MemberReference.Annotation<Cecil.PropertyDefinition>();
            NonPublic = !_propertyDefinition.GetMethod.IsPublic;
            Type = _propertyDefinition.PropertyType.GetActualType();

            if (isSetter) {
                _propertyMethod = propertyInfo.GetSetMethod(NonPublic);
                _emitPrivateAction = EmitPrivateStorePropertyReference;
            }
            else {
                _propertyMethod = propertyInfo.GetGetMethod(NonPublic);
                _emitPrivateAction = EmitPrivateLoadPropertyReference;
            }
        }

        public override AstNode Emit() {
            if (NonPublic) {
                EmitPrivateReference();
            }
            else {
                bool hasThis = (CallingConventions.HasThis & _propertyMethod.CallingConvention) == CallingConventions.HasThis;
                OpCode callOpCode = OpCodes.Call;

                if (hasThis) {
                    MemberReference.Target.AcceptVisitor(Visitor, ILGenerator);
                    callOpCode = OpCodes.Callvirt;
                }

                ILGenerator.Emit(callOpCode, _propertyMethod);
            }

            return new AstNodeDecorator(MemberReference, Type);
        }

        protected internal override void EmitPrivateReference() {
            _emitPrivateAction();
        }

        private void EmitPrivateStorePropertyReference() {
            var methodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
            var methodBuilder = DynamicTypeBuilder.Current.DefineMethod("EmitPrivateStorePropertyReference", methodAttributes);
            var methodBuilderILGenerator = methodBuilder.GetILGenerator();
            var localArray = methodBuilderILGenerator.DeclareLocal(TypeSystem.Object.MakeArrayType());

            methodBuilder.SetParameters(new Type[] { TypeSystem.Int });
            methodBuilder.SetReturnType(TypeSystem.Void);

            methodBuilderILGenerator.Emit(OpCodes.Ldtoken, Target);
            methodBuilderILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            methodBuilderILGenerator.Emit(OpCodes.Ldstr, _propertyDefinition.Name);
            methodBuilderILGenerator.EmitPushInteger(Convert.ToInt32(BindingFlags.NonPublic | BindingFlags.Static));
            methodBuilderILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetProperty", new Type[] { TypeSystem.String, typeof(BindingFlags) }));
            methodBuilderILGenerator.Emit(OpCodes.Ldc_I4_1);
            methodBuilderILGenerator.Emit(OpCodes.Callvirt, typeof(PropertyInfo).GetMethod("GetSetMethod", new Type[] { TypeSystem.Boolean }));
            methodBuilderILGenerator.Emit(OpCodes.Ldnull);
            methodBuilderILGenerator.Emit(OpCodes.Ldc_I4_1);
            methodBuilderILGenerator.Emit(OpCodes.Newarr, TypeSystem.Object);
            methodBuilderILGenerator.Emit(OpCodes.Stloc_S, localArray);
            methodBuilderILGenerator.Emit(OpCodes.Ldloc_S, localArray);
            methodBuilderILGenerator.Emit(OpCodes.Ldc_I4_0);
            methodBuilderILGenerator.Emit(OpCodes.Ldarg_0);
            methodBuilderILGenerator.Emit(OpCodes.Box, TypeSystem.Int);
            methodBuilderILGenerator.Emit(OpCodes.Stelem_Ref);
            methodBuilderILGenerator.Emit(OpCodes.Ldloc_S, localArray);
            methodBuilderILGenerator.Emit(OpCodes.Callvirt, typeof(MethodBase).GetMethod("Invoke", new Type[] { TypeSystem.Object, typeof(object[]) }));
            methodBuilderILGenerator.Emit(OpCodes.Pop);
            methodBuilderILGenerator.Emit(OpCodes.Ret);

            ILGenerator.Emit(OpCodes.Call, methodBuilder);
        }

        private void EmitPrivateLoadPropertyReference() {
            ILGenerator.DeclareLocal(typeof(MethodInfo));
            ILGenerator.Emit(OpCodes.Ldtoken, Target);
            ILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            ILGenerator.Emit(OpCodes.Ldstr, _propertyDefinition.Name);
            ILGenerator.EmitPushInteger(Convert.ToInt32(BindingFlags.NonPublic | BindingFlags.Static));
            ILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetProperty", new Type[] { TypeSystem.String, typeof(BindingFlags) }));
            ILGenerator.Emit(OpCodes.Ldc_I4_1);
            ILGenerator.Emit(OpCodes.Callvirt, typeof(PropertyInfo).GetMethod("GetGetMethod", new Type[] { TypeSystem.Boolean }));
            ILGenerator.Emit(OpCodes.Stloc_0);
            ILGenerator.Emit(OpCodes.Ldloc_0);
            ILGenerator.Emit(OpCodes.Ldnull);
            ILGenerator.Emit(OpCodes.Ldnull);
            ILGenerator.Emit(OpCodes.Callvirt, typeof(MethodBase).GetMethod("Invoke", new Type[] { TypeSystem.Object, typeof(object[]) }));
            ILGenerator.Emit(OpCodes.Unbox_Any, Type);
        }
    }
}