using System.Collections.Generic;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class NamedExpressionEmitter : AbstractDepthFirstVisitorEmitter<NamedExpression>
    {
        internal NamedExpressionEmitter(NamedExpression namedExpression,
                                        ILGenerator ilGenerator,
                                        IOpCodeIndexer instructionIndexer,
                                        IAstVisitor<ILGenerator, AstNode> visitor,
                                        List<LocalBuilder> locals)
            : base(namedExpression, ilGenerator, instructionIndexer, visitor, locals) { }

        public override AstNode Emit() {
            return Node.Expression.AcceptVisitor(ILGenerator, Visitor);
        }
    }
}