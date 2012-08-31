using System.Collections.Generic;
using System.Reflection.Emit;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class LocalReferenceEmitter : AbstractILEmitter
    {
        private List<LocalBuilder> _locals = null;
        private IdentifierExpression _identifierExpression = null;

        internal LocalReferenceEmitter(IdentifierExpression identifierExpression, ILGenerator ilGenerator, IOpCodeIndexer instructionsIndexer, List<LocalBuilder> locals)
            : base(ilGenerator, instructionsIndexer) {
            _identifierExpression = identifierExpression;
            _locals = locals;
        }

        public override AstNode Emit() {
            var variable = _identifierExpression.Annotation<ILVariable>();
            var variableReference = variable.OriginalVariable;

            if (variableReference != null) {
                var localBuilder = _locals[variableReference.Index];
                
                Type = variable.Type.GetActualType();
                ILGenerator.Emit(OpCodes.Ldloc, localBuilder);
            }

            return new AstNodeDecorator(_identifierExpression, Type);
        }
    }
}