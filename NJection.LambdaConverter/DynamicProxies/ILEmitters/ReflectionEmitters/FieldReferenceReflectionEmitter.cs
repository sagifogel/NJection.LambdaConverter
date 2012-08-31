using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using Cil = Mono.Cecil.Cil;

using Ref = System.Reflection;
using NJection.LambdaConverter.Mappers;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class FieldReferenceReflectionEmitter : AbstractMemberReferenceEmitter
    {
        protected bool IsByRef = false;
        private Action _emitAction = null;
        protected FieldInfo FieldInfo = null;
        private Action _emitPrivateAction = null;
        protected FieldReference FieldReference = null;
        private delegate bool FieldFunction(AstNode node, out Cil.Instruction instruction);
        private static Ref.FieldAttributes _publicFieldAttributes = Ref.FieldAttributes.FamANDAssem | Ref.FieldAttributes.Family;

        internal FieldReferenceReflectionEmitter(MemberReferenceExpression memberReferenceExpression,
                                                 Type target,
                                                 MemberInfo member,
                                                 ILGenerator ilGenerator,
                                                 IOpCodeIndexer instructionsIndexer,
                                                 IAstVisitor<ILGenerator, AstNode> visitor,
                                                 List<LocalBuilder> locals,
                                                 bool isSetter = false)
            : base(memberReferenceExpression, target, member, ilGenerator, instructionsIndexer, visitor, locals) {

            FieldReference = MemberReference.Annotation<FieldReference>();
            IsByRef = MemberReference.Parent is DirectionExpression;
            Type = FieldReference.FieldType.GetActualType();
            FieldInfo = FieldReference.GetActualField();
            NonPublic = !((FieldInfo.Attributes & (_publicFieldAttributes)) == _publicFieldAttributes);

            if (isSetter) {
                _emitAction = EmitStoreFieldReference;
                _emitPrivateAction = EmitPrivateStoreFieldReference;
            }
            else {
                _emitAction = EmitLoadFieldReference;
                _emitPrivateAction = EmitPrivateLoadFieldReference;
            }
        }

        public override AstNode Emit() {
            _emitAction();

            return new AstNodeDecorator(MemberReference, Type);
        }

        private void EmitLoadFieldReference() {
            var instruction = InstructionsIndexer.GetInstruction(MemberReference);

            if (instruction == null || !IsPrimitive()) {
                if (NonPublic) {
                    EmitPrivateReference();
                }
                else {
                    var opCode = GetLoadFieldOpCode();
                    ILGenerator.Emit(opCode, Member as FieldInfo);
                }

                MemberReference.Children.ForEach(c => c.AcceptVisitor(ILGenerator, Visitor));
            }
            else {
                EmitPrimitive(ILGenerator, instruction);
            }
        }

        private void EmitStoreFieldReference() {
            if (NonPublic) {
                EmitPrivateReference();
            }
            else {
                var opCode = GetStoreFieldOpCode();
                ILGenerator.Emit(opCode, Member as FieldInfo);
            }
        }

        private bool IsPrimitive() {
            return (Type.IsPrimitive || Type.IsEnum) && !Type.Equals(TypeSystem.IntPtr);
        }

        private OpCode GetLoadFieldOpCode() {
            OpCode opCode;
            bool isStatic = FieldInfo.IsStatic;

            if (IsByRef) {
                opCode = isStatic ? OpCodes.Ldsflda : OpCodes.Ldflda;
            }
            else {
                opCode = isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld;
            }

            return opCode;
        }

        private OpCode GetStoreFieldOpCode() {
            return FieldInfo.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld;
        }

        protected internal override void EmitPrivateReference() {
            _emitPrivateAction();
        }

        private void EmitPrivateLoadFieldReference() {
            Type fieldInfoType = typeof(FieldInfo);

            ILGenerator.DeclareLocal(fieldInfoType);
            ILGenerator.Emit(OpCodes.Ldtoken, Target);
            ILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            ILGenerator.Emit(OpCodes.Ldstr, FieldReference.Name);
            ILGenerator.EmitPushInteger(Convert.ToInt32(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));
            ILGenerator.Emit(OpCodes.Callvirt, typeof(Type).GetMethod("GetField", new Type[] { TypeSystem.String, typeof(BindingFlags) }));
            ILGenerator.Emit(OpCodes.Ldnull);
            ILGenerator.Emit(OpCodes.Callvirt, fieldInfoType.GetMethod("GetValue", new Type[] { TypeSystem.Object }));

            if (!IsByRef) {
                ILGenerator.Emit(OpCodes.Unbox_Any, Type);
            }
            else {
                ILGenerator.Emit(OpCodes.Castclass, Type);
            }
        }

        private void EmitPrivateStoreFieldReference() {
            Type fieldInfoType = typeof(FieldInfo);

            ILGenerator.DeclareLocal(fieldInfoType);
            ILGenerator.Emit(OpCodes.Ldtoken, Target);
            ILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            ILGenerator.Emit(OpCodes.Ldstr, FieldReference.Name);
            ILGenerator.EmitPushInteger(Convert.ToInt32(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance));
            ILGenerator.Emit(OpCodes.Callvirt, typeof(Type).GetMethod("GetField", new Type[] { TypeSystem.String, typeof(BindingFlags) }));
            ILGenerator.Emit(OpCodes.Ldnull);
            ILGenerator.Emit(OpCodes.Callvirt, fieldInfoType.GetMethod("GetValue", new Type[] { TypeSystem.Object }));
            ILGenerator.Emit(OpCodes.Unbox_Any, Type);
        }

        private void EmitPrimitive(ILGenerator ILGenerator, Cil.Instruction instruction) {
            TypeCode typeCode;
            OpCode opCode = OpCodesMapper.Map(instruction.OpCode);

            if (instruction.Operand == null) {
                ILGenerator.Emit(opCode);
                return;
            }

            typeCode = opCode.OpCodeType == OpCodeType.Objmodel ?
                            TypeCode.String :
                            opCode.OpCodeType.GetTypeCode();

            if (typeCode == TypeCode.Int32) {
                switch (opCode.StackBehaviourPush) {
                    case StackBehaviour.Pushi8:

                        typeCode = TypeCode.Int64;
                        break;

                    case StackBehaviour.Pushr4:

                        typeCode = TypeCode.Single;
                        break;

                    case StackBehaviour.Pushr8:

                        typeCode = TypeCode.Double;
                        break;
                }
            }

            ILGenerator.EmitPrimitiveByTypeCode(instruction.Operand, typeCode);
        }
    }
}