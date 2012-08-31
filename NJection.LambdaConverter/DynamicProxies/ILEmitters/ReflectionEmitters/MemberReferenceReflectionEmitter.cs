using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;
using ICSharpCode.Decompiler.Ast;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class MemberReferenceReflectionEmitter : AbstractDepthFirstVisitorEmitter<MemberReferenceExpression>
    {
        private bool _isSetter;
        protected Type Target = null;
        protected MemberInfo Member = null;

        internal MemberReferenceReflectionEmitter(MemberReferenceExpression memberReferenceExpression,
                                                  ILGenerator ilGenerator,
                                                  IOpCodeIndexer instructionsIndexer,
                                                  IAstVisitor<ILGenerator, AstNode> visitor,
                                                  List<LocalBuilder> locals,
                                                  bool isSetter = false)
            : base(memberReferenceExpression, ilGenerator, instructionsIndexer, visitor, locals) {
            
            var targetTypeReference = Node.Target as TypeReferenceExpression;

            if (targetTypeReference != null) {
                Type = targetTypeReference.Type.GetActualType();
            }
            else if (Node.Target is TypeOfExpression) {
                Type = typeof(Type);
            }
            else {
                Type = Node.Target.Annotation<TypeInformation>()
                                  .InferredType
                                  .GetActualType();
            }

            Target = Type;
            _isSetter = isSetter;
            Member = Target.GetMember(Node.MemberName, ReflectionUtils.AllFlags)[0];
        }

        public override AstNode Emit() {
            AstNode astNode = null;

            switch (Member.MemberType) {
                case MemberTypes.Event:

                    Member = Target.GetField(Node.MemberName, ReflectionUtils.AllFlags);
                    astNode = new EventReferenceReflectionEmitter(Node, Target, Member, ILGenerator, InstructionsIndexer, Visitor, Locals).Emit();
                    break;

                case MemberTypes.Field:

                    astNode = new FieldReferenceReflectionEmitter(Node, Target, Member, ILGenerator, InstructionsIndexer, Visitor, Locals, _isSetter).Emit();
                    break;

                case MemberTypes.Property:

                    if (Type.Equals(typeof(Type))) {
                        astNode = Node.Target.AcceptVisitor(ILGenerator, Visitor);
                    }
                    else {
                        astNode = new PropertyReferenceReflectionEmitter(Node, Target, Member, ILGenerator, InstructionsIndexer, Visitor, Locals, _isSetter).Emit();
                    }

                    break;
            }

            return astNode;
        }
    }
}