using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class ReturnStatementEmitter : AbstractILEmitter
    {
        private ReturnStatement _returnStatement = null;

        internal ReturnStatementEmitter(ReturnStatement returnStatement, ILGenerator ilGenerator)
            : base(ilGenerator, null) {
            _returnStatement = returnStatement;
            Type = DynamicTypeBuilder.Current.UnderlyingSystemType;
        }

        public override AstNode Emit() {
            ILGenerator.Emit(OpCodes.Ret);

            return new AstNodeDecorator(_returnStatement, Type);
        }
    }
}