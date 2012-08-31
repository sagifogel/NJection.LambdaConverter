using System;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class ThisExpressionEmitter : DepthFirstAstVisitor<ILGenerator, AstNode>, ILEmitter
    {
        private AstNode _astNode = null;
        private IAstVisitor<ILGenerator, AstNode> _visitor = null;

        internal ThisExpressionEmitter(ThisReferenceExpression thisReferenceExpression, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer, IAstVisitor<ILGenerator, AstNode> visitor)
            : this(thisReferenceExpression as AstNode, ilGenerator, instructionsIndexer, visitor) {
            Type = thisReferenceExpression.Annotation<TypeInformation>().InferredType.GetActualType();
        }

        internal ThisExpressionEmitter(ConstructorDeclaration constructorDeclaration, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer, IAstVisitor<ILGenerator, AstNode> visitor)
            : this(constructorDeclaration as AstNode, ilGenerator, instructionsIndexer, visitor) {
            Type = constructorDeclaration.Annotation<MethodReference>()
                                         .GetActualMethod<ConstructorInfo>()
                                         .DeclaringType;
        }

        private ThisExpressionEmitter(AstNode node, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer, IAstVisitor<ILGenerator, AstNode> visitor) {
            _astNode = node;
            _visitor = visitor;
            ILGenerator = ilGenerator;
            InstructionsIndexer = instructionsIndexer;
        }

        public Type Type { get; private set; }
        public ILGenerator ILGenerator { get; private set; }
        public IOpCodeIndexer InstructionsIndexer { get; private set; }

        public AstNode Emit() {
            ILGenerator.Emit(OpCodes.Ldarg_0);
            _astNode.Children.ForEach(node => node.AcceptVisitor(ILGenerator, _visitor));

            return new AstNodeDecorator(_astNode, Type);
        }
    }
}