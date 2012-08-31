using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil.Cil;

namespace NJection.LambdaConverter
{
    public sealed class InstructionsIndexer : IOpCodeIndexer
    {
        private List<Instruction> _instructions = null;
        private static OpCode[] _callOpCodes = new OpCode[] { OpCodes.Callvirt, OpCodes.Call, OpCodes.Calli };
        private static OpCode[] _storeFieldOpCodes = new OpCode[] { OpCodes.Stfld, OpCodes.Stsfld };
        private static OpCode[] _loadFieldOpCodes = new OpCode[] { OpCodes.Ldfld, OpCodes.Ldsfld, OpCodes.Ldflda, OpCodes.Ldsflda };
        private static OpCode[] _convertOpCodes = new OpCode[] { OpCodes.Unbox_Any, OpCodes.Unbox, OpCodes.Conv_R8, OpCodes.Conv_R4, OpCodes.Conv_I, OpCodes.Conv_I1, OpCodes.Conv_I2, OpCodes.Conv_I4, OpCodes.Conv_I8, OpCodes.Conv_Ovf_I, OpCodes.Conv_Ovf_I_Un, OpCodes.Conv_Ovf_I1, OpCodes.Conv_Ovf_I1_Un, OpCodes.Conv_Ovf_I2, OpCodes.Conv_Ovf_I2_Un, OpCodes.Conv_Ovf_I4, OpCodes.Conv_Ovf_I4_Un, OpCodes.Conv_Ovf_I8, OpCodes.Conv_Ovf_I8_Un, OpCodes.Conv_Ovf_U, OpCodes.Conv_Ovf_U_Un, OpCodes.Conv_Ovf_U1, OpCodes.Conv_Ovf_U1_Un, OpCodes.Conv_Ovf_U2, OpCodes.Conv_Ovf_U2_Un, OpCodes.Conv_Ovf_U4, OpCodes.Conv_Ovf_U4_Un, OpCodes.Conv_Ovf_U8, OpCodes.Conv_Ovf_U8_Un, OpCodes.Conv_R_Un };

        public InstructionsIndexer(List<Instruction> instructions) {
            _instructions = instructions;
        }

        public Instruction GetInstruction(AstNode node) {
            var range = node.Annotation<List<ILRange>>();

            if (range != null && range.Count > 0) {
                return _instructions.First(i => i.Offset == range[0].From);
            }

            return null;
        }

        public bool TryGetInstruction(AstNode node, OpCode opCode, out Instruction instruction) {
            List<ILRange> ilRanges = node.Annotation<List<ILRange>>();

            return TryGetInstruction(ilRanges, opCode, out instruction);
        }

        private Instruction InstructionAt(int offset) {
            return _instructions.FirstOrDefault(i => i.Offset == offset);
        }

        public bool TryGetLoadFieldInstruction(AstNode node, out Instruction instruction) {
            return TryGetOpCode(node, _loadFieldOpCodes, out instruction);
        }

        public bool TryGetStoreFieldInstruction(AstNode node, out Instruction instruction) {
            return TryGetOpCode(node, _storeFieldOpCodes, out instruction);
        }

        public bool TryGetConvertInstruction(CastExpression node, Type type, out Instruction instruction) {
            return TryGetOpCode(node, _convertOpCodes, out instruction);
        }

        public bool TryGetCallInstruction(AstNode node, out Instruction instruction) {
            return TryGetOpCode(node, _callOpCodes, out instruction);
        }

        public bool TryGetOpCode(AstNode node, OpCode[] opCodes, out Instruction instruction) {
            List<ILRange> ilRanges = node.Annotation<List<ILRange>>();

            instruction = null;

            foreach (var opCode in opCodes) {
                if (TryGetInstruction(ilRanges, opCode, out instruction)) {
                    return true;
                }
            }

            return false;
        }

        private bool TryGetInstruction(List<ILRange> ilRanges, OpCode opCode, out Instruction instruction) {
            int fromOffset = 0;
            int toOffset = 0;

            if (ilRanges == null || ilRanges.Count == 0) {
                instruction = null;
                return false;
            }

            fromOffset = ilRanges[0].From;
            toOffset = ilRanges[0].To;
            instruction = InstructionAt(fromOffset);

            do {
                if (instruction.OpCode == opCode) {
                    return true;
                }

                instruction = instruction.Next;
                fromOffset = instruction.Offset;
            }
            while (fromOffset != toOffset);

            return instruction.OpCode == opCode;
        }

        public bool TryGetNewObjectInstruction(AstNode node, Type type, out Instruction instruction) {
            OpCode opCode = default(OpCode);
            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode) {
                case TypeCode.Boolean:

                    break;

                case TypeCode.Byte:

                    break;

                case TypeCode.Char:

                    break;

                case TypeCode.DBNull:

                    break;

                case TypeCode.DateTime:

                    break;

                case TypeCode.Decimal:

                    break;

                case TypeCode.Double:

                    opCode = OpCodes.Conv_R8;
                    break;

                case TypeCode.Empty:

                    break;

                case TypeCode.Int16:

                    break;

                case TypeCode.Int32:

                    break;

                case TypeCode.Int64:

                    opCode = OpCodes.Conv_I8;
                    break;

                case TypeCode.Object:

                    break;

                case TypeCode.SByte:

                    break;

                case TypeCode.Single:

                    break;

                case TypeCode.String:

                    break;

                case TypeCode.UInt16:

                    break;

                case TypeCode.UInt32:

                    break;

                case TypeCode.UInt64:

                    break;
            }

            return TryGetInstruction(node, opCode, out instruction);
        }

        public bool TryGetNewObjectInstruction(AstNode node, out Instruction instruction) {
            return TryGetInstruction(node, OpCodes.Newobj, out instruction);
        }

        public Instruction GetNextInstruction(Instruction instruction) {
            return InstructionAt(instruction.Offset).Next;
        }

        public Instruction GetLastInstructionInRange(AstNode node) {
            List<ILRange> ilRanges = node.Annotation<List<ILRange>>();

            if (ilRanges != null && ilRanges.Count > 0) {
                var lastRange = ilRanges[ilRanges.Count - 1];
                var instruction = InstructionAt(lastRange.To);

                if (instruction != null) {
                    return instruction.Previous;
                }
            }

            return null;
        }

        public IEnumerable<Instruction> GetRangeOfInstructions(Instruction startFrom, int count) {
            var index = _instructions.IndexOf(startFrom);

            return GetRangeOfInstuction(index, count);
        }

        public IEnumerable<Instruction> GetPrevoiusRangeOfInstructions(Instruction startFrom, int count) {
            var index = _instructions.IndexOf(startFrom) - count;

            return GetRangeOfInstuction(index, count);
        }

        private IEnumerable<Instruction> GetRangeOfInstuction(int startFrom, int count) {
            for (int i = startFrom; i < startFrom + count; i++) {
                yield return _instructions[i];
            }
        }
    }
}