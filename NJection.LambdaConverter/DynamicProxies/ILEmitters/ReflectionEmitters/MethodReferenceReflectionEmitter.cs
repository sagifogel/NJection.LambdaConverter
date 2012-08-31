using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SysReflection = System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class MethodReferenceReflectionEmitter : AbstractDepthFirstVisitorEmitter<InvocationExpression>
    {
        internal MethodReferenceReflectionEmitter(InvocationExpression invocationExpression,
                                                  ILGenerator ilGenerator,
                                                  IOpCodeIndexer instructionsIndexer,
                                                  IAstVisitor<ILGenerator, AstNode> visitor,
                                                  List<LocalBuilder> locals)
            : base(invocationExpression, ilGenerator, instructionsIndexer, visitor, locals) { }

        public override AstNode VisitInvocationExpression(InvocationExpression invocationExpression, ILGenerator data) {
            var localBuilder = Locals[Locals.Count - 1];

            invocationExpression.Arguments.ForEach((arg, i) => {
                AstNodeDecorator decorator = null;

                ILGenerator.EmitLoadLocal(localBuilder);
                ILGenerator.EmitPushInteger(i);
                decorator = arg.AcceptVisitor(data, this);
                ILGenerator.EmitCastIfNeeded(decorator.Type, TypeSystem.Object);
                ILGenerator.Emit(OpCodes.Stelem_Ref);
            });

            return invocationExpression;
        }

        public override AstNode VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, ILGenerator data) {
            var methodReference = objectCreateExpression.Annotation<MethodReference>();
            var ctor = methodReference.GetActualMethod<ConstructorInfo>();

            base.VisitObjectCreateExpression(objectCreateExpression, data);
            ILGenerator.Emit(OpCodes.Newobj, ctor);

            return objectCreateExpression;
        }

        public override AstNode VisitIdentifierExpression(IdentifierExpression identifierExpression, ILGenerator data) {
            return identifierExpression.AcceptVisitor(data, Visitor);
        }

        public override AstNode VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ILGenerator data) {
            return base.VisitArrayInitializerExpression(arrayInitializerExpression, data);
        }

        public override AstNode VisitArrayCreateExpression(ArrayCreateExpression arrayObjectCreateExpression, ILGenerator data) {
            return new ArrayCreationEmitter(arrayObjectCreateExpression, ILGenerator, InstructionsIndexer, Visitor, Locals).Emit();
        }

        public override AstNode VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, ILGenerator data) {
            return defaultValueExpression.AcceptVisitor(data, Visitor);
        }

        public override AstNode VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, ILGenerator data) {
            return base.VisitUnaryOperatorExpression(unaryOperatorExpression, data);
        }

        public override AstNode VisitAssignmentExpression(AssignmentExpression assignmentExpression, ILGenerator data) {
            return base.VisitAssignmentExpression(assignmentExpression, data);
        }

        public override AstNode VisitAsExpression(AsExpression asExpression, ILGenerator data) {
            return base.VisitAsExpression(asExpression, data);
        }

        public override AstNode VisitCastExpression(CastExpression castExpression, ILGenerator data) {
            return base.VisitCastExpression(castExpression, data);
        }

        public override AstNode VisitDirectionExpression(DirectionExpression directionExpression, ILGenerator data) {
            return base.VisitDirectionExpression(directionExpression, data);
        }

        public override AstNode VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, ILGenerator data) {
            return base.VisitNullReferenceExpression(nullReferenceExpression, data);
        }

        public override AstNode VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, ILGenerator data) {
            var emitter = new PrimitiveEmitter(primitiveExpression, data, InstructionsIndexer).Emit() as AstNodeDecorator;

            return new AstNodeDecorator(primitiveExpression, emitter.Type);
        }

        public override AstNode Emit() {
            MethodBase method = null;
            bool isDelegate = false;
            MethodReference methodReference = null;

            if (Node.Target.ToString().Equals("ldftn", StringComparison.OrdinalIgnoreCase)) {
                methodReference = Node.Arguments.First().Annotation<MethodReference>();
                method = methodReference.GetActualMethod<MethodInfo>();
                ILGenerator.Emit(OpCodes.Ldftn, method as MethodInfo);
            }
            else {
                methodReference = Node.Annotation<MethodReference>();
                isDelegate = IsDelegate(methodReference);
                method = methodReference.GetActualMethod();

                if (method.DeclaringType.Namespace.Equals("System.Reflection")) {
                    throw new NotSupportedException("Emitting constructors through System.Reflection is not supported");
                }

                if (isDelegate) {
                    Node.Target.AcceptVisitor(Visitor, ILGenerator);
                }

                if (method.IsPrivate || isDelegate) {
                    var methodInfo = method as MethodInfo;

                    Type = methodInfo.ReturnType;

                    if (isDelegate) {
                        EmitPrivateDelegate(methodInfo);
                    }
                    else {
                        EmitPrivate(methodInfo);
                    }
                }
                else {
                    var parameters = method.GetParameters();

                    Node.Arguments.ForEach((arg, i) => {
                        var parameterType = parameters[i].ParameterType;
                        var expression = arg.AcceptVisitor(ILGenerator, Visitor);

                        ILGenerator.EmitCastIfNeeded(expression.Type, parameterType);
                    });

                    if (method.IsConstructor) {
                        var ctor = method as ConstructorInfo;

                        Type = ctor.DeclaringType;
                        ILGenerator.Emit(OpCodes.Call, ctor);
                    }
                    else {
                        var methodInfo = method as MethodInfo;

                        Type = methodInfo.ReturnType;
                        ILGenerator.Emit(OpCodes.Call, methodInfo);
                    }
                }
            }

            return base.Emit();
        }

        private void EmitPrivateDelegate(MethodInfo methodInfo) {
            var typeOfDelegate = typeof(Delegate);
            var localArray = ILGenerator.DeclareLocal(typeof(object[]));
            var parameters = methodInfo.GetParameters();

            Locals.Add(localArray);
            ILGenerator.Emit(OpCodes.Castclass, typeOfDelegate);
            ILGenerator.EmitPushInteger(parameters.Length);
            ILGenerator.Emit(OpCodes.Newarr, TypeSystem.Object);
            ILGenerator.EmitStoreLocal(localArray);

            Node.AcceptVisitor(this, ILGenerator);

            ILGenerator.EmitLoadLocal(localArray);
            ILGenerator.Emit(OpCodes.Callvirt, typeOfDelegate.GetMethod("DynamicInvoke"));
            ILGenerator.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
        }

        private void EmitPrivate(MethodInfo methodInfo) {
            Type typeArrayType = typeof(Type[]);
            Type objectArrayType = typeof(object[]);
            Type methodInfoType = typeof(MethodInfo);
            var parameters = methodInfo.GetParameters();
            var localTypeArray = ILGenerator.DeclareLocal(typeArrayType);
            var localObjectArray = ILGenerator.DeclareLocal(objectArrayType);
            var getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");
            var invocation = methodInfoType.BaseType.GetMethod("Invoke", new Type[] { TypeSystem.Object, typeof(object[]) });

            Locals.Add(localTypeArray);
            Locals.Add(localObjectArray);
            ILGenerator.Emit(OpCodes.Ldtoken, methodInfo.DeclaringType);
            ILGenerator.Emit(OpCodes.Call, getTypeFromHandleMethod);
            ILGenerator.Emit(OpCodes.Ldstr, methodInfo.Name);
            ILGenerator.EmitPushInteger(Convert.ToInt32(BindingFlags.NonPublic | BindingFlags.Static));
            ILGenerator.Emit(OpCodes.Ldnull);
            ILGenerator.EmitPushInteger(parameters.Length);
            ILGenerator.Emit(OpCodes.Newarr, typeArrayType.GetElementType());
            ILGenerator.EmitStoreLocal(localTypeArray);

            parameters.ForEach((p, i) => {
                ILGenerator.EmitLoadLocal(localTypeArray);
                ILGenerator.EmitPushInteger(i);
                ILGenerator.Emit(OpCodes.Ldtoken, p.ParameterType);
                ILGenerator.Emit(OpCodes.Call, getTypeFromHandleMethod);
                ILGenerator.Emit(OpCodes.Stelem_Ref);
            });

            ILGenerator.EmitLoadLocal(localTypeArray);
            ILGenerator.Emit(OpCodes.Ldnull);
            ILGenerator.Emit(OpCodes.Call, typeof(Type).GetMethod("GetMethod", new Type[] { TypeSystem.String, typeof(BindingFlags), typeof(Binder), typeof(Type[]), typeof(SysReflection.ParameterModifier[]) }));
            ILGenerator.Emit(OpCodes.Ldnull);
            ILGenerator.EmitPushInteger(parameters.Length);
            ILGenerator.Emit(OpCodes.Newarr, TypeSystem.Object);
            ILGenerator.EmitStoreLocal(localObjectArray);

            Node.AcceptVisitor(this, ILGenerator);

            ILGenerator.EmitLoadLocal(localObjectArray);
            ILGenerator.Emit(OpCodes.Callvirt, invocation);
            ILGenerator.Emit(OpCodes.Unbox_Any, methodInfo.ReturnType);
        }

        private bool IsDelegate(MethodReference methodReference) {
            return methodReference.DeclaringType
                                  .GetActualType()
                                  .IsDelegate();
        }
    }
}