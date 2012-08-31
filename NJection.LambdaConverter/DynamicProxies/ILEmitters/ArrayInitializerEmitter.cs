using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using Cil = Mono.Cecil.Cil;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class ArrayInitializerEmitter : AbstractDepthFirstVisitorEmitter<ArrayInitializerExpression>
    {
        private Type _typeToEmit = null;
        private bool _isPrimitive = false;
        private LocalBuilder _localBuilder = null;
        private Func<AstNode> _emitFunction = null;
        private Action<AstNode, int> _emitElementAction = null;

        internal ArrayInitializerEmitter(ArrayInitializerExpression arrayInitializerExpression,
                                         ILGenerator ilGenerator,
                                         IOpCodeIndexer instructionIndexer,
                                         IAstVisitor<ILGenerator, AstNode> visitor,
                                         List<LocalBuilder> locals)
            : base(arrayInitializerExpression, ilGenerator, instructionIndexer, visitor, locals) {
            _localBuilder = Locals[Locals.Count - 1];
            Type = _localBuilder.LocalType;

            if (Type.IsArray) {
                _typeToEmit = Type.GetElementType();

                if (Type.GetArrayRank() > 1) {
                    _emitFunction = EmitMultiDimensionalArrayInitializer;
                }
                else {
                    _emitFunction = EmitArrayInitializer;
                }
            }
            else {
                _typeToEmit = Type;
                _emitFunction = EmitNewObjectInitializer;
            }

            _isPrimitive = _typeToEmit.IsPrimitive();
            _emitElementAction = _isPrimitive ?
                                    EmitPrimitiveElement :
                                    (Action<AstNode, int>)EmitElementRef;
        }

        public override AstNode Emit() {
            return _emitFunction();
        }

        private AstNode EmitArrayInitializer() {
            ILGenerator.EmitPushInteger(Node.Elements.Count);
            ILGenerator.Emit(OpCodes.Newarr, _typeToEmit);
            ILGenerator.EmitStoreLocal(_localBuilder);

            Node.Elements.ForEach((node, i) => {
                _emitElementAction(node, i);
            });

            ILGenerator.EmitLoadLocal(_localBuilder, true);

            return new AstNodeDecorator(Node, Type);
        }

        private AstNode EmitMultiDimensionalArrayInitializer() {
            List<int> dimensions = null;
            MethodInfo arraySetMethod = null;
            AstNodeDecorator nodeDecorator = null;
            var arrayCreation = Node.Parent as ArrayCreateExpression;
            var specifier = arrayCreation.AdditionalArraySpecifiers.First();
            Type[] types = new Type[specifier.Dimensions + 1];
            Type[] constructorTypes = new Type[specifier.Dimensions];

            Enumerable.Range(0, specifier.Dimensions)
                      .ForEach(i => types[i] = constructorTypes[i] = TypeSystem.Int);

            types[types.Length - 1] = _typeToEmit;
            arraySetMethod = Type.GetMethod("Set", types);
            dimensions = BuildDimsnsions(arrayCreation);
            dimensions.ForEach(d => ILGenerator.EmitPushInteger(d));
            ILGenerator.Emit(OpCodes.Newobj, Type.GetConstructor(constructorTypes));
            ILGenerator.EmitStoreLocal(_localBuilder);

            BuildIndexesRecursively(Node, new List<int>()).ForEach((tuple, i) => {
                ILGenerator.EmitLoadLocal(_localBuilder);
                tuple.Item2.ForEach(index => ILGenerator.EmitPushInteger(index));
                nodeDecorator = tuple.Item1.AcceptVisitor(ILGenerator, Visitor);
                ILGenerator.EmitCastIfNeeded(nodeDecorator.Type, _typeToEmit);
                ILGenerator.Emit(OpCodes.Call, arraySetMethod);
            });

            ILGenerator.EmitLoadLocal(_localBuilder, true);

            return new AstNodeDecorator(Node, Type);
        }

        private List<int> BuildDimsnsions(ArrayCreateExpression arrayCreation) {
            List<int> ints = new List<int>();

            var arrayInitializer = arrayCreation.Initializer;

            while (arrayInitializer != null) {
                ints.Add(arrayInitializer.Elements.Count);
                arrayInitializer = arrayInitializer.Elements.First() as ArrayInitializerExpression;
            }

            return ints;
        }

        private List<Tuple<Expression, List<int>>> BuildIndexesRecursively(ArrayInitializerExpression arrayInitializer, List<int> indexes) {
            List<Tuple<Expression, List<int>>> expressions = new List<Tuple<Expression, List<int>>>();

            arrayInitializer.Elements.ForEach((e, i) => {
                var childInitializer = e as ArrayInitializerExpression;

                if (childInitializer != null) {
                    indexes.Add(i);
                    expressions.AddRange(BuildIndexesRecursively(childInitializer, indexes));
                }
                else {
                    indexes.Add(i);
                    expressions.Add(Tuple.Create(e, indexes.ToList()));
                }

                indexes.RemoveAt(indexes.Count - 1);
            });

            return expressions;
        }

        private void EmitPrimitiveElement(AstNode node, int index) {
            AstNodeDecorator astDecorator = null;

            ILGenerator.EmitLoadLocal(_localBuilder);
            ILGenerator.EmitPushInteger(index);
            ILGenerator.EmitLoadElementArrayIfNeeded(_typeToEmit);
            astDecorator = node.AcceptVisitor(ILGenerator, Visitor);
            ILGenerator.EmitStoreElementByType(astDecorator.Type);
        }

        private void EmitElementRef(AstNode node, int index) {
            AstNodeDecorator nodeDecorator = null;

            ILGenerator.EmitLoadLocal(_localBuilder);
            ILGenerator.EmitPushInteger(index);
            nodeDecorator = node.AcceptVisitor(ILGenerator, Visitor);
            ILGenerator.EmitCastIfNeeded(nodeDecorator.Type, _typeToEmit);
            ILGenerator.Emit(OpCodes.Stelem_Ref);
        }

        private AstNode EmitNewObjectInitializer() {
            Type parameterType = null;
            bool requiresPop = false;
            MethodInfo methodInfo = null;
            AstNodeDecorator nodeDecorator = null;
            var callOpCode = _isPrimitive ? OpCodes.Call : OpCodes.Callvirt;
            var firstChild = Node.Elements.First();
            var methodReference = Node.Elements.First().Annotation<MethodReference>();

            if (methodReference == null) {
                Cil.Instruction instruction;
                InstructionsIndexer.TryGetCallInstruction(firstChild, out instruction);
                methodReference = instruction.Operand as MethodReference;
            }

            methodInfo = methodReference.GetActualMethod<MethodInfo>();
            parameterType = methodInfo.GetParameters()[0].ParameterType;
            requiresPop = !methodInfo.ReturnType.Equals(TypeSystem.Void);

            Node.Elements.ForEach((node, i) => {
                ILGenerator.EmitLoadLocal(_localBuilder);
                nodeDecorator = node.AcceptVisitor(ILGenerator, Visitor);
                ILGenerator.EmitCastIfNeeded(nodeDecorator.Type, parameterType);
                ILGenerator.Emit(callOpCode, methodInfo);

                if (requiresPop) {
                    ILGenerator.Emit(OpCodes.Pop);
                }
            });

            ILGenerator.EmitLoadLocal(_localBuilder, true);

            return new AstNodeDecorator(Node, Type);
        }
    }
}