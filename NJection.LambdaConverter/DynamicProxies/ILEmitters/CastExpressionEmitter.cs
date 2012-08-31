using System.Collections.Generic;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Mappers;
using Cil = Mono.Cecil.Cil;
using System.Diagnostics;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class CastExpressionEmitter : AbstractDepthFirstVisitorEmitter<CastExpression>
    {
        internal CastExpressionEmitter(CastExpression castExpression,
                                       ILGenerator ilGenerator,
                                       IOpCodeIndexer instructionsIndexer,
                                       IAstVisitor<ILGenerator, AstNode> visitor,
                                       List<LocalBuilder> locals)
            : base(castExpression, ilGenerator, instructionsIndexer, visitor, locals) { }

        public override AstNode Emit() {
            var astNodeDecorator = Node.Expression.AcceptVisitor(ILGenerator, Visitor);
            
            Type = Node.Type.GetActualType();
            ILGenerator.EmitConversion(Type, astNodeDecorator.Type);
            
            return base.Emit();
        }
    }
}