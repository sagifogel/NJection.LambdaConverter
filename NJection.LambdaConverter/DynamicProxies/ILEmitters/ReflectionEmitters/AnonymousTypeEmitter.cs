using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.Extensions;

namespace NJection.LambdaConverter.DynamicProxies.ILEmitters
{
    internal class AnonymousTypeEmitter : AbstractDepthFirstVisitorEmitter<AnonymousTypeCreateExpression>
    {
        internal AnonymousTypeEmitter(AnonymousTypeCreateExpression anonymousTypeCreateExpression,
                                      ILGenerator ilGenerator,
                                      IOpCodeIndexer instructionIndexer,
                                      IAstVisitor<ILGenerator, AstNode> visitor,
                                      List<LocalBuilder> locals)
            : base(anonymousTypeCreateExpression, ilGenerator, instructionIndexer, visitor, locals) {
            Type = anonymousTypeCreateExpression.Annotation<TypeInformation>()
                                                .InferredType
                                                .GetActualType();
        }

        public override AstNode Emit() {
            var typeOfSystemType = typeof(Type);
            var ctor = Type.GetConstructors()[0];
            var parameters = ctor.GetParameters();
            var parmetersCount = parameters.Length;
            var arrayOfSystemTypes = typeOfSystemType.MakeArrayType();
            var resultLocal = ILGenerator.DeclareLocal(TypeSystem.Object);
            var arrayOfObjectsLocal = ILGenerator.DeclareLocal(TypeSystem.Object.MakeArrayType());
            var arrayOfSystemTypesLocal = ILGenerator.DeclareLocal(arrayOfSystemTypes);
            var getTypeFromHandleMethodInfo = typeOfSystemType.GetMethod("GetTypeFromHandle");

            ILGenerator.Emit(OpCodes.Ldtoken, Type);
            ILGenerator.Emit(OpCodes.Call, getTypeFromHandleMethodInfo);
            ILGenerator.EmitPushInteger(parmetersCount);
            ILGenerator.Emit(OpCodes.Newarr, typeOfSystemType);
            ILGenerator.EmitStoreLocal(arrayOfSystemTypesLocal);

            parameters.ForEach((p, i) => {
                ILGenerator.EmitLoadLocal(arrayOfSystemTypesLocal);
                ILGenerator.EmitPushInteger(i);
                ILGenerator.Emit(OpCodes.Ldtoken, p.ParameterType);
                ILGenerator.Emit(OpCodes.Call, getTypeFromHandleMethodInfo);
                ILGenerator.Emit(OpCodes.Stelem_Ref);
            });

            ILGenerator.EmitLoadLocal(arrayOfSystemTypesLocal);
            ILGenerator.Emit(OpCodes.Call, typeOfSystemType.GetMethod("GetConstructor", new Type[] { arrayOfSystemTypes }));
            ILGenerator.EmitPushInteger(parmetersCount);
            ILGenerator.Emit(OpCodes.Newarr, TypeSystem.Object);
            ILGenerator.EmitStoreLocal(arrayOfObjectsLocal);

            Node.Initializers.ForEach((initializer, i) => {
                ILGenerator.EmitLoadLocal(arrayOfObjectsLocal);
                ILGenerator.EmitPushInteger(i);
                var node = initializer.AcceptVisitor(ILGenerator, Visitor);

                ILGenerator.EmitCastIfNeeded(node.Type, TypeSystem.Object);
                ILGenerator.Emit(OpCodes.Stelem_Ref);
            });

            ILGenerator.EmitLoadLocal(arrayOfObjectsLocal);
            ILGenerator.Emit(OpCodes.Callvirt, typeof(ConstructorInfo).GetMethod("Invoke", new Type[] { arrayOfObjectsLocal.LocalType }));
            ILGenerator.EmitStoreLocal(resultLocal);
            ILGenerator.EmitLoadLocal(resultLocal);

            return new AstNodeDecorator(Node, Type);
        }
    }
}