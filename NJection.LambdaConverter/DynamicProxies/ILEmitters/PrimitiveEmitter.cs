using System;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Mappers;
using Cil = Mono.Cecil.Cil;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class PrimitiveEmitter : AbstractILEmitter
    {
        protected Cil.Instruction PrimitiveInstruction = null;
        protected PrimitiveExpression PrimitiveExpression = null;

        internal PrimitiveEmitter(PrimitiveExpression primitiveExpression, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer)
            : base(ilGenerator, instructionsIndexer) {
            
            PrimitiveExpression = primitiveExpression;
            Type = PrimitiveExpression.Value.GetType();
            PrimitiveInstruction = InstructionsIndexer.GetInstruction(PrimitiveExpression);
        }

        public override AstNode Emit() {
            Action _emitAction = null;

            switch (Type.Name) {
                case "Decimal":

                    _emitAction = EmitDecimal;
                    break;

                case "String":

                    _emitAction = EmitString;
                    break;

                default:

                    _emitAction = EmitPrimitive;
                    break;
            }

            _emitAction();

            return new AstNodeDecorator(PrimitiveExpression, Type);
        }

        private void EmitDecimal() {
            var primitiveOperand = PrimitiveInstruction.Operand;
            var methodReference = primitiveOperand as MethodReference;
            var ctor = methodReference.GetActualMethod<ConstructorInfo>();
            var types = ctor.GetParameters();
            var instructions = InstructionsIndexer.GetPrevoiusRangeOfInstructions(PrimitiveInstruction, types.Length);

            instructions.ForEach((instruction, i) => {
                var mapped = OpCodesMapper.Map(instruction.OpCode);
                var operand = instruction.Operand;

                if (operand == null) {
                    ILGenerator.Emit(mapped);
                }
                else {
                    var typeCode = Type.GetTypeCode(operand.GetType());
                    ILGenerator.EmitPrimitiveByTypeCode(operand, typeCode);
                }
            });

            ILGenerator.Emit(OpCodes.Newobj, ctor);
        }

        private void EmitString() {
            string actualValue = PrimitiveInstruction.Operand as string;

            ILGenerator.Emit(OpCodes.Ldstr, actualValue);
        }

        private void EmitPrimitive() {
            var value = PrimitiveExpression.Value;
            var typeCode = Type.GetTypeCode(value.GetType());

            ILGenerator.EmitPrimitiveByTypeCode(value, typeCode);
        }
    }
}