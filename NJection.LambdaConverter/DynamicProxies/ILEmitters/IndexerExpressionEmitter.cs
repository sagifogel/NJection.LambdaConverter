using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;
using System.Linq;
using Mono.Cecil;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class IndexerExpressionEmitter : AbstractDepthFirstVisitorEmitter<IndexerExpression>
    {
        private Type _typeToEmit = null;
        private Action _indexerEmitter = null;

        internal IndexerExpressionEmitter(IndexerExpression indexerExpression,
                                          ILGenerator ilGenerator,
                                          IOpCodeIndexer instructionIndexer,
                                          IAstVisitor<ILGenerator, AstNode> visitor,
                                          List<LocalBuilder> locals)
            : base(indexerExpression, ilGenerator, instructionIndexer, visitor, locals) {
            Type = indexerExpression.Target
                                    .Annotation<TypeInformation>()
                                    .InferredType
                                    .GetActualType();

            if (Type.IsArray) {
                _typeToEmit = Type.GetElementType();

                if (Type.GetArrayRank() == 1) {
                    _indexerEmitter = EmitArrayIndexer;
                }
                else {
                    _indexerEmitter = EmitMultiDimensionalArrayIndexer;
                }
            }
            else {
                _indexerEmitter = EmitIdexerByMethod;
            }
        }

        public override AstNode Emit() {
            _indexerEmitter();

            return new AstNodeDecorator(Node, Type);
        }

        private void EmitArrayIndexer() {
            Node.Target.AcceptVisitor(ILGenerator, Visitor);
            Node.Arguments.ForEach(arg => arg.AcceptVisitor(ILGenerator, Visitor));
            ILGenerator.EmitLoadElementByType(Type.GetElementType());
        }

        private void EmitIdexerByMethod() {
            var methodReference = Node.Annotation<MethodReference>();
            var methodInfo = methodReference.GetActualMethod<MethodInfo>();
            var callOpCode = methodReference.HasThis ? OpCodes.Callvirt : OpCodes.Call;

            Type = methodInfo.ReturnType;
            Node.Target.AcceptVisitor(ILGenerator, Visitor);
            Node.Arguments.ForEach(arg => arg.AcceptVisitor(ILGenerator, Visitor));
            ILGenerator.Emit(callOpCode, methodInfo); 
        }

        private void EmitMultiDimensionalArrayIndexer() {
            MethodInfo arrayGetMethod = null;

            arrayGetMethod = Type.GetMethod("Get");
            Node.Target.AcceptVisitor(ILGenerator, Visitor);
            Node.Arguments.ForEach(arg => arg.AcceptVisitor(ILGenerator, Visitor));
            ILGenerator.Emit(OpCodes.Call, arrayGetMethod);
        }
    }
}