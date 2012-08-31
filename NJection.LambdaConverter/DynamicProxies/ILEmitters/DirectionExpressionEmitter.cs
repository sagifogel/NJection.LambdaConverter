using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using System.Reflection.Emit;
using ICSharpCode.Decompiler.Ast;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class DirectionExpressionEmitter : AbstractDepthFirstVisitorEmitter<DirectionExpression>
    {
        internal DirectionExpressionEmitter(DirectionExpression directionExpression,
                                            ILGenerator ilGenerator,
                                            IOpCodeIndexer instructionIndexer,
                                            IAstVisitor<ILGenerator, AstNode> visitor,
                                            List<LocalBuilder> locals)
            : base(directionExpression, ilGenerator, instructionIndexer, visitor, locals) { }

        public override AstNode Emit() {
            var astNodeDecorator = Node.Expression.AcceptVisitor(ILGenerator, Visitor);

            if (!astNodeDecorator.Type.IsByRef) {
                Type = astNodeDecorator.Type.MakeByRefType();
            }

            return base.Emit();
        }
    }
}
