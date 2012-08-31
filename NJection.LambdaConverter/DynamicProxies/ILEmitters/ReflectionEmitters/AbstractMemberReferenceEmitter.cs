using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal abstract class AbstractMemberReferenceEmitter : AbstractReflectionEmitter
    {
        protected Type Target = null;
        protected MemberInfo Member = null;
        protected MemberReferenceExpression MemberReference = null;

        internal AbstractMemberReferenceEmitter(MemberReferenceExpression memberReferenceExpression,
                                                Type target,
                                                MemberInfo member,
                                                ILGenerator ilGenerator,
                                                IOpCodeIndexer instructionsIndexer,
                                                IAstVisitor<ILGenerator, AstNode> visitor,
                                                List<LocalBuilder> locals)
            : base(ilGenerator, instructionsIndexer, visitor, locals) {
            Target = target;
            Member = member;
            MemberReference = memberReferenceExpression;
        }
    }
}