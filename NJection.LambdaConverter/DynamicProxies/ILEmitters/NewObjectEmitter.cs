using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using Cil = Mono.Cecil.Cil;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class NewObjectEmitter : DepthFirstAstVisitor<ILGenerator, AstNode>, ILEmitter
    {
        private List<LocalBuilder> _locals = null;
        private IAstVisitor<ILGenerator, AstNode> _visitor = null;
        private ObjectCreateExpression _objectCreateExpression = null;

        internal NewObjectEmitter(ObjectCreateExpression objectCreateExpression, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer, IAstVisitor<ILGenerator, AstNode> visitor, List<LocalBuilder> locals) {
            _locals = locals;
            _visitor = visitor;
            ILGenerator = ilGenerator;
            InstructionsIndexer = instructionsIndexer;
            _objectCreateExpression = objectCreateExpression;
            Type = _objectCreateExpression.Type.GetActualType();
        }

        public Type Type { get; private set; }
        public ILGenerator ILGenerator { get; private set; }
        public IOpCodeIndexer InstructionsIndexer { get; private set; }

        public AstNode Emit() {
            if (Type.IsDelegate()) {
                EmitDelegate();
            }
            else if (Type.IsValueType) {
                EmitNewStruct();
            }
            else {
                EmitNewObject();
            }

            return new AstNodeDecorator(_objectCreateExpression, Type);
        }

        private void EmitNewObject() {
            LocalBuilder localBuilder = null;
            var methodReference = _objectCreateExpression.Annotation<MethodReference>();
            var ctor = methodReference.GetActualMethod<ConstructorInfo>();

            _objectCreateExpression.Arguments.ForEach(arg => arg.AcceptVisitor(_visitor, ILGenerator));

            if (!_objectCreateExpression.Initializer.IsNull) {
                localBuilder = ILGenerator.DeclareLocal(Type);

                _locals.Add(localBuilder);
                ILGenerator.Emit(OpCodes.Newobj, ctor);
                ILGenerator.EmitStoreLocal(localBuilder);
                _objectCreateExpression.Initializer.AcceptVisitor(_visitor, ILGenerator);
            }
            else {
                ILGenerator.Emit(OpCodes.Newobj, ctor);
            }
        }

        private void EmitNewStruct() {
            LocalBuilder localBuilder = null;

            if (_objectCreateExpression.Arguments.Count > 0) {
                var methodReference = _objectCreateExpression.Annotation<MethodReference>();
                var ctor = methodReference.GetActualMethod<ConstructorInfo>();

                if (!_objectCreateExpression.Initializer.IsNull) {
                    localBuilder = ILGenerator.DeclareLocal(Type);
                    _locals.Add(localBuilder);
                    ILGenerator.EmitLoadLocal(localBuilder);
                    _objectCreateExpression.Arguments.ForEach(arg => arg.AcceptVisitor(_visitor, ILGenerator));
                    ILGenerator.Emit(OpCodes.Call, ctor);
                    _objectCreateExpression.Initializer.AcceptVisitor(_visitor, ILGenerator);
                }
                else {
                    _objectCreateExpression.Arguments.ForEach(arg => arg.AcceptVisitor(_visitor, ILGenerator));
                    ILGenerator.Emit(OpCodes.Newobj, ctor);
                }
            }
            else {
                localBuilder = ILGenerator.DeclareLocal(Type);
                _locals.Add(localBuilder);
                ILGenerator.EmitLoadLocal(localBuilder);
                ILGenerator.Emit(OpCodes.Initobj, Type);
                _objectCreateExpression.Initializer.AcceptVisitor(_visitor, ILGenerator);
            }
        }

        private void EmitDelegate() {
            FieldInfo fieldInfo = null;
            Cil.Instruction instruction;
            ConstructorInfo constructor = null;
            FieldReference fieldReference = null;
            Label label = ILGenerator.DefineLabel();

            InstructionsIndexer.TryGetLoadFieldInstruction(_objectCreateExpression, out instruction);
            fieldReference = instruction.Operand as FieldReference;
            fieldInfo = fieldReference.GetActualField();
            Type = fieldInfo.FieldType;
            constructor = Type.GetConstructor(new Type[] { TypeSystem.Object, TypeSystem.IntPtr });

            ILGenerator.Emit(OpCodes.Ldsfld, fieldInfo);
            ILGenerator.Emit(OpCodes.Brtrue_S, label);
            _objectCreateExpression.Arguments.ForEach(arg => arg.AcceptVisitor(_visitor, ILGenerator));
            ILGenerator.Emit(OpCodes.Newobj, constructor);
            ILGenerator.Emit(OpCodes.Stsfld, fieldInfo);
            ILGenerator.MarkLabel(label);
            ILGenerator.Emit(OpCodes.Ldsfld, fieldInfo);
        }
    }
}