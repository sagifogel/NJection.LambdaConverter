using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class EventReferenceReflectionEmitter : FieldReferenceReflectionEmitter
    {
        internal EventReferenceReflectionEmitter(MemberReferenceExpression memberReferenceExpression,
                                                 Type target,
                                                 MemberInfo member,
                                                 ILGenerator ilGenerator,
                                                 IOpCodeIndexer instructionsIndexer,
                                                 IAstVisitor<ILGenerator, AstNode> visitor,
                                                 List<LocalBuilder> locals)
            : base(memberReferenceExpression, target, member, ilGenerator, instructionsIndexer, visitor, locals) { }

        public override AstNode Emit() {
            EmitPrivateReference();

            return new AstNodeDecorator(MemberReference, Type);
        }

        protected internal override void EmitPrivateReference() {
            Type fieldInfoType = typeof(FieldInfo);
            Type typeOfSystemType = typeof(Type);
            var localBuilder =  ILGenerator.DeclareLocal(fieldInfoType);

            Locals.Add(localBuilder);
            ILGenerator.Emit(OpCodes.Ldtoken, Target);
            ILGenerator.Emit(OpCodes.Call, typeOfSystemType.GetMethod("GetTypeFromHandle"));
            ILGenerator.Emit(OpCodes.Ldstr, FieldReference.Name);
            ILGenerator.EmitPushInteger(Convert.ToInt32(BindingFlags.NonPublic | BindingFlags.Static));
            ILGenerator.Emit(OpCodes.Callvirt, typeOfSystemType.GetMethod("GetField", new Type[] { TypeSystem.String, typeof(BindingFlags) }));
            ILGenerator.EmitStoreLocal(localBuilder);
            ILGenerator.EmitLoadLocal(localBuilder);
            ILGenerator.Emit(OpCodes.Ldnull);
            ILGenerator.Emit(OpCodes.Callvirt, fieldInfoType.GetMethod("GetValue", new Type[] { TypeSystem.Object }));
        }
    }
}