using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class MultipuleAssignmentEmitter : MemberReferenceReflectionEmitter
    {
        private Expression _assignorExpression = null;

        internal MultipuleAssignmentEmitter(MemberReferenceExpression memberReferenceExpression,
                                            Expression assignor,
                                            ILGenerator ilGenerator,
                                            IOpCodeIndexer instructionsIndexer,
                                            IAstVisitor<ILGenerator, AstNode> visitor,
                                            List<LocalBuilder> locals)
            : base(memberReferenceExpression, ilGenerator, instructionsIndexer, visitor, locals) {
            _assignorExpression = assignor;
        }

        public override AstNode Emit() {
            LocalBuilder localBuilder = null;
            Action<LocalBuilder> emitAction = null;

            switch (Member.MemberType) {
                case MemberTypes.Event:

                    emitAction = EmitEventRegistration;
                    break;

                case MemberTypes.Field:

                    var fieldInfo = Member as FieldInfo;

                    if (fieldInfo.FieldType.IsDelegate()) {
                        emitAction = EmitEventRegistration;
                    }
                    else {
                        emitAction = fieldInfo.IsPublic ?
                                        EmitPublicFieldAssignment :
                                        (Action<LocalBuilder>)EmitPrivateFieldAssignment;
                    }

                    break;

                case MemberTypes.Property:

                    var propertyInfo = Member as PropertyInfo;

                    emitAction = propertyInfo.GetSetMethod(true).IsPublic ?
                                        EmitPublicPropertyAssignment :
                                        (Action<LocalBuilder>)EmitPrivatePropertyAssignment;
                    break;
            }

            emitAction(localBuilder);

            return new AstNodeDecorator(Node, Type);
        }

        private void EmitPrivateFieldAssignment(LocalBuilder localbuilder) {
            var fieldInfo = Member as FieldInfo;
            var methodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
            var methodBuilder = DynamicTypeBuilder.Current.DefineMethod("EmitPrivateFieldAssignment", methodAttributes);
            var methodBuilderILGenerator = methodBuilder.GetILGenerator();

            Type = fieldInfo.FieldType;
            methodBuilder.SetReturnType(Type);
            _assignorExpression.AcceptVisitor(Visitor, methodBuilderILGenerator);

            methodBuilderILGenerator.DeclareLocal(Type);
            methodBuilderILGenerator.Emit(OpCodes.Stloc_1);
            methodBuilderILGenerator.Emit(OpCodes.Ldtoken, Target);
            methodBuilderILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            methodBuilderILGenerator.Emit(OpCodes.Ldstr, Member.Name);
            methodBuilderILGenerator.EmitPushInteger(Convert.ToInt32(BindingFlags.NonPublic | BindingFlags.Static));
            methodBuilderILGenerator.Emit(OpCodes.Callvirt, typeof(Type).GetMethod("GetField", new Type[] { TypeSystem.String, typeof(BindingFlags) }));
            methodBuilderILGenerator.Emit(OpCodes.Ldnull);
            methodBuilderILGenerator.Emit(OpCodes.Ldloc_1);
            methodBuilderILGenerator.Emit(OpCodes.Callvirt, typeof(FieldInfo).GetMethod("SetValue", new Type[] { TypeSystem.Object, TypeSystem.Object }));
            methodBuilderILGenerator.Emit(OpCodes.Ldloc_1);
            methodBuilderILGenerator.Emit(OpCodes.Ret);

            ILGenerator.Emit(OpCodes.Call, methodBuilder);
        }

        private void EmitPublicFieldAssignment(LocalBuilder localbuilder) {
            var fieldInfo = Member as FieldInfo;

            _assignorExpression.AcceptVisitor(Visitor, ILGenerator);
            ILGenerator.Emit(OpCodes.Dup);
            ILGenerator.Emit(OpCodes.Stsfld, fieldInfo);
        }

        private void EmitPublicPropertyAssignment(LocalBuilder localbuilder) {
            MethodInfo setPropertyMethod = (Member as PropertyInfo).GetSetMethod();

            _assignorExpression.AcceptVisitor(Visitor, ILGenerator);
            ILGenerator.Emit(OpCodes.Dup);
            ILGenerator.Emit(OpCodes.Call, setPropertyMethod);
        }

        private void EmitPrivatePropertyAssignment(LocalBuilder localbuilder) {
            var propertyInfo = Member as PropertyInfo;
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            var methodAttributes = MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig;
            var methodBuilder = DynamicTypeBuilder.Current.DefineMethod("EmitPrivatePropertyAssignment", methodAttributes);
            var methodBuilderILGenerator = methodBuilder.GetILGenerator();

            Type = propertyInfo.PropertyType;
            methodBuilder.SetReturnType(Type);
            _assignorExpression.AcceptVisitor(Visitor, methodBuilderILGenerator);

            methodBuilderILGenerator.DeclareLocal(Type);
            methodBuilderILGenerator.DeclareLocal(typeof(object[]));
            methodBuilderILGenerator.Emit(OpCodes.Stloc_1);
            methodBuilderILGenerator.Emit(OpCodes.Ldtoken, Target);
            methodBuilderILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            methodBuilderILGenerator.Emit(OpCodes.Ldstr, Member.Name);
            methodBuilderILGenerator.EmitPushInteger(Convert.ToInt32(BindingFlags.NonPublic | BindingFlags.Static));
            methodBuilderILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetProperty", new Type[] { TypeSystem.Object, typeof(BindingFlags) }));
            methodBuilderILGenerator.Emit(OpCodes.Ldc_I4_1);
            methodBuilderILGenerator.Emit(OpCodes.Callvirt, typeof(PropertyInfo).GetMethod("GetSetMethod", new Type[] { TypeSystem.Boolean }));
            methodBuilderILGenerator.Emit(OpCodes.Ldnull);
            methodBuilderILGenerator.Emit(OpCodes.Ldc_I4_1);
            methodBuilderILGenerator.Emit(OpCodes.Newarr, TypeSystem.Object);
            methodBuilderILGenerator.Emit(OpCodes.Stloc_2);
            methodBuilderILGenerator.Emit(OpCodes.Ldloc_2);
            methodBuilderILGenerator.Emit(OpCodes.Ldc_I4_0);
            methodBuilderILGenerator.Emit(OpCodes.Ldloc_1);
            methodBuilderILGenerator.Emit(OpCodes.Stelem_Ref);
            methodBuilderILGenerator.Emit(OpCodes.Ldloc_2);
            methodBuilderILGenerator.Emit(OpCodes.Callvirt, typeof(MethodBase).GetMethod("Invoke", new Type[] { TypeSystem.Object, typeof(object[]) }));
            methodBuilderILGenerator.Emit(OpCodes.Pop);
            methodBuilderILGenerator.Emit(OpCodes.Ldloc_1);
            methodBuilderILGenerator.Emit(OpCodes.Ret);

            ILGenerator.Emit(OpCodes.Call, methodBuilder);
        }

        private void EmitEventRegistration(LocalBuilder localbuilder) {
            FieldInfo fieldInfo = Target.GetField(Member.Name, BindingFlags.NonPublic | BindingFlags.Static);
            var dynamicMethod = new DynamicMethod("EmitEventRegistration", TypeSystem.Void, Type.EmptyTypes, true);
            var dynamicMethodILGenerator = dynamicMethod.GetILGenerator();
            var emitter = new FieldReferenceReflectionEmitter(Node, Target, fieldInfo, ILGenerator, InstructionsIndexer, Visitor, Locals);

            _assignorExpression.AcceptVisitor(Visitor, dynamicMethodILGenerator);
            dynamicMethodILGenerator.Emit(OpCodes.Stsfld, fieldInfo);
            dynamicMethodILGenerator.Emit(OpCodes.Ret);
            dynamicMethod.CreateDelegate(typeof(Action)).DynamicInvoke();
            emitter.Emit();
        }
    }
}