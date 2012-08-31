using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.ILAst;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.TypeResolvers;

namespace NJection.LambdaConverter.Visitors
{
    internal class GenericTypesTransformerVisitor : DepthFirstAstVisitor<ICecilArgumentsResolver, AstNode>
    {
        public override AstNode VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, ICecilArgumentsResolver argumentsResolver) {
            arrayCreateExpression.Arguments.ForEach(a => a.AcceptVisitor(this, argumentsResolver));
            arrayCreateExpression.Initializer.AcceptVisitor(this, argumentsResolver);

            return arrayCreateExpression;
        }

        public override AstNode VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression, ICecilArgumentsResolver argumentsResolver) {
            arrayInitializerExpression.Elements.ForEach(e => e.AcceptVisitor(this, argumentsResolver));

            return arrayInitializerExpression;
        }

        public override AstNode VisitAsExpression(AsExpression asExpression, ICecilArgumentsResolver argumentsResolver) {
            asExpression.Expression.AcceptVisitor(this, argumentsResolver);
            asExpression.Type.AcceptVisitor(this, argumentsResolver);

            return asExpression;
        }

        public override AstNode VisitAssignmentExpression(AssignmentExpression assignmentExpression, ICecilArgumentsResolver argumentsResolver) {
            assignmentExpression.Left.AcceptVisitor(this, argumentsResolver);
            assignmentExpression.Right.AcceptVisitor(this, argumentsResolver);

            return assignmentExpression;
        }

        public override AstNode VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, ICecilArgumentsResolver argumentsResolver) {
            return baseReferenceExpression;
        }

        public override AstNode VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, ICecilArgumentsResolver argumentsResolver) {
            binaryOperatorExpression.Right.AcceptVisitor(this, argumentsResolver);
            binaryOperatorExpression.Left.AcceptVisitor(this, argumentsResolver);

            return binaryOperatorExpression;
        }

        public override AstNode VisitBlockStatement(BlockStatement blockStatement, ICecilArgumentsResolver argumentsResolver) {
            blockStatement.Statements.ForEach(s => s.AcceptVisitor(this, argumentsResolver));

            return blockStatement;
        }

        public override AstNode VisitBreakStatement(BreakStatement breakStatement, ICecilArgumentsResolver argumentsResolver) {
            return breakStatement;
        }

        public override AstNode VisitCaseLabel(CaseLabel caseLabel, ICecilArgumentsResolver argumentsResolver) {
            return caseLabel;
        }

        public override AstNode VisitCastExpression(CastExpression castExpression, ICecilArgumentsResolver argumentsResolver) {
            castExpression.Expression.AcceptVisitor(this, argumentsResolver);
            castExpression.Type.AcceptVisitor(this, argumentsResolver);

            return castExpression;
        }

        public override AstNode VisitCatchClause(CatchClause catchClause, ICecilArgumentsResolver argumentsResolver) {
            catchClause.Body.AcceptVisitor(this, argumentsResolver);

            return catchClause;
        }

        public override AstNode VisitComposedType(ComposedType composedType, ICecilArgumentsResolver argumentsResolver) {
            composedType.BaseType.AcceptVisitor(this, argumentsResolver);

            return composedType;
        }

        public override AstNode VisitConditionalExpression(ConditionalExpression conditionalExpression, ICecilArgumentsResolver argumentsResolver) {
            conditionalExpression.Condition.AcceptVisitor(this, argumentsResolver);
            conditionalExpression.TrueExpression.AcceptVisitor(this, argumentsResolver);
            conditionalExpression.FalseExpression.AcceptVisitor(this, argumentsResolver);

            return conditionalExpression;
        }

        public override AstNode VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, ICecilArgumentsResolver argumentsResolver) {
            constructorDeclaration.Parameters.ForEach(p => p.AcceptVisitor(this, argumentsResolver));
            constructorDeclaration.Body.AcceptVisitor(this, argumentsResolver);

            return constructorDeclaration;
        }

        public override AstNode VisitContinueStatement(ContinueStatement continueStatement, ICecilArgumentsResolver argumentsResolver) {
            return continueStatement;
        }

        public override AstNode VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, ICecilArgumentsResolver argumentsResolver) {
            defaultValueExpression.Type.AcceptVisitor(this, argumentsResolver);

            return defaultValueExpression;
        }

        public override AstNode VisitDirectionExpression(DirectionExpression directionExpression, ICecilArgumentsResolver argumentsResolver) {
            directionExpression.Expression.AcceptVisitor(this, argumentsResolver);

            return directionExpression;
        }

        public override AstNode VisitEmptyExpression(EmptyExpression emptyExpression, ICecilArgumentsResolver argumentsResolver) {
            return emptyExpression;
        }

        public override AstNode VisitExpressionStatement(ExpressionStatement expressionStatement, ICecilArgumentsResolver argumentsResolver) {
            return expressionStatement.Expression.AcceptVisitor(this, argumentsResolver);
        }

        public override AstNode VisitGotoStatement(GotoStatement gotoStatement, ICecilArgumentsResolver argumentsResolver) {
            return gotoStatement;
        }

        public override AstNode VisitIdentifierExpression(IdentifierExpression identifierExpression, ICecilArgumentsResolver argumentsResolver) {
            var variable = identifierExpression.Annotation<ILVariable>();

            if (variable != null) {
                var variableType = variable.Type.GetGenericTypeOrSelfReference();

                if (variableType.IsGenericParameter) {
                    variable.Type = argumentsResolver.ResolveTypeReference(variableType.ToString());
                }
            }

            return identifierExpression;
        }

        public override AstNode VisitIfElseStatement(IfElseStatement ifElseStatement, ICecilArgumentsResolver argumentsResolver) {
            ifElseStatement.Condition.AcceptVisitor(this, argumentsResolver);
            ifElseStatement.TrueStatement.AcceptVisitor(this, argumentsResolver);
            ifElseStatement.FalseStatement.AcceptVisitor(this, argumentsResolver);

            return ifElseStatement;
        }

        public override AstNode VisitIndexerExpression(IndexerExpression indexerExpression, ICecilArgumentsResolver argumentsResolver) {
            indexerExpression.Target.AcceptVisitor(this, argumentsResolver);
            indexerExpression.Arguments.ForEach(a => a.AcceptVisitor(this, argumentsResolver));

            return indexerExpression;
        }

        public override AstNode VisitInvocationExpression(InvocationExpression invocationExpression, ICecilArgumentsResolver argumentsResolver) {
            MethodReference reference = null;

            invocationExpression.Arguments.ForEach(a => a.AcceptVisitor(this, argumentsResolver));
            invocationExpression.Target.AcceptVisitor(this, argumentsResolver);
            var specification = invocationExpression.Annotation<MethodSpecification>();

            if (specification != null && specification.IsGenericInstance) {
                var genericMethod = specification as GenericInstanceMethod;
                var genericArguments = genericMethod.GenericArguments.ToArray();
                var parameters = genericMethod.Parameters.Select(p => p.ParameterType);
                reference = specification.GetElementMethod();

                reference = ResolveGenericMethod(parameters, reference, genericArguments, argumentsResolver);
                invocationExpression.RemoveAnnotations<MethodSpecification>();
                invocationExpression.AddAnnotation(reference);
            }
            else {
                reference = invocationExpression.Annotation<MethodReference>();

                if (reference != null) {
                    TypeReference returnType = reference.ReturnType;
                    TypeReference declaringType = reference.DeclaringType;

                    if (declaringType.IsGenericInstance || returnType.IsGenericParameter) {
                        if (declaringType.IsGenericInstance) {
                            var genericMethod = declaringType as GenericInstanceType;
                            var parameters = reference.Parameters.Select(p => p.ParameterType);

                            reference = ResolveGenericMethod(parameters, reference, genericMethod.GenericArguments, argumentsResolver);
                            genericMethod.GenericArguments.Clear();
                            reference.GenericParameters.ForEach(p => genericMethod.GenericArguments.Add(p));
                        }

                        if (reference.ReturnType.IsGenericParameter) {
                            var genericInstanceType = declaringType as GenericInstanceType;
                            var genericParameter = reference.ReturnType as GenericParameter;
                            var genericReturnType = genericParameter.GetGenericActualType(genericInstanceType.GenericArguments.ToArray());

                            reference = reference.MakeGenericMethod(new TypeReference[] { }, genericReturnType);
                        }

                        invocationExpression.RemoveAnnotations<MethodReference>();
                        invocationExpression.AddAnnotation(reference);
                    }
                }
            }

            return invocationExpression;
        }

        private MethodReference ResolveGenericMethod(IEnumerable<TypeReference> parameters, MethodReference methodReference, IEnumerable<TypeReference> genericArguments, ICecilArgumentsResolver argumentsResolver) {
            if (methodReference.HasGenericParameters) {
                ResolveGenericArguments(parameters, methodReference.ReturnType, genericArguments, argumentsResolver);
            }

            var arguments = genericArguments.Select(argument => {
                TypeReference typeReference = argument;

                if (argument.NeedsTypeResolving()) {
                    typeReference = argumentsResolver.ResolveTypeReference(argument.ToString());
                }

                return typeReference;
            });

            methodReference = methodReference.MakeGenericMethod(arguments, methodReference.ReturnType);

            return methodReference;
        }

        private void ResolveGenericArguments(IEnumerable<TypeReference> parameters, TypeReference returnType, IEnumerable<TypeReference> genericArguments, ICecilArgumentsResolver argumentsResolver) {
            bool genericParameters = true;
            var empty = Enumerable.Empty<TypeReference>();
            var dictionary = new Dictionary<bool, IEnumerable<TypeReference>> { { true, empty }, { false, empty } };

            parameters.Where(p => p.IsGeneric())
                      .GroupBy(p => p.IsGenericParameter)
                      .ForEach(g => dictionary[g.Key] = g.AsEnumerable());

            ResolveGenericParameters(dictionary[genericParameters], genericArguments, argumentsResolver);
            ResolveGenericParameters(dictionary[!genericParameters], genericArguments, argumentsResolver);

            if (returnType.IsGeneric()) {
                ResolveGenericParameter(returnType, genericArguments, argumentsResolver);
            }
        }

        private void ResolveGenericParameters(IEnumerable<TypeReference> parameters, IEnumerable<TypeReference> genericArguments, ICecilArgumentsResolver argumentsResolver) {
            parameters.ForEach(p => ResolveGenericParameter(p, genericArguments, argumentsResolver));
        }

        private void ResolveGenericParameter(TypeReference typeReference, IEnumerable<TypeReference> genericArguments, ICecilArgumentsResolver argumentsResolver) {
            string name = string.Empty;
            TypeReferencePair pair = null;

            if (typeReference.IsGenericInstance) {
                typeReference.GetGenericActualType(genericArguments);
                return;
            }

            name = typeReference.Name;

            if (!argumentsResolver.Contains(name)) {
                typeReference = typeReference.GetGenericActualType(genericArguments);

                if (typeReference.IsGenericParameter) {
                    name = typeReference.FullName;
                    pair = argumentsResolver.ResolvePair(name);
                    typeReference = pair.TypeReference;

                    if (typeReference.IsByReference) {
                        var type = pair.Type.MakeByRefType();

                        typeReference = pair.TypeReference.Resolve();
                        typeReference = typeReference.Module.Import(typeReference);
                    }
                }
            }
        }

        public override AstNode VisitLabelStatement(LabelStatement labelStatement, ICecilArgumentsResolver argumentsResolver) {
            return labelStatement;
        }

        public override AstNode VisitLockStatement(LockStatement lockStatement, ICecilArgumentsResolver argumentsResolver) {
            lockStatement.Expression.AcceptVisitor(this, argumentsResolver);

            return lockStatement;
        }

        public override AstNode VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, ICecilArgumentsResolver argumentsResolver) {
            memberReferenceExpression.TypeArguments.ForEach(t => t.AcceptVisitor(this, argumentsResolver));
            memberReferenceExpression.Target.AcceptVisitor(this, argumentsResolver);

            return memberReferenceExpression;
        }

        public override AstNode VisitMemberType(MemberType memberType, ICecilArgumentsResolver argumentsResolver) {
            memberType.Target.AcceptVisitor(this, argumentsResolver);

            return memberType;
        }

        public override AstNode VisitMethodDeclaration(MethodDeclaration methodDeclaration, ICecilArgumentsResolver argumentsResolver) {
            TypeReference returnTypeReference = null;
            var methodReference = methodDeclaration.Annotation<MethodReference>();

            methodDeclaration.TypeParameters.ForEach(p => p.AcceptVisitor(this, argumentsResolver));
            methodDeclaration.Parameters.ForEach(p => p.AcceptVisitor(this, argumentsResolver));
            methodDeclaration.Body.AcceptVisitor(this, argumentsResolver);
            methodDeclaration.ReturnType.AcceptVisitor(this, argumentsResolver);

            if (methodReference.ReturnType.IsGeneric()) {
                returnTypeReference = methodDeclaration.ReturnType.Annotation<TypeReference>();
                methodReference.ReturnType = returnTypeReference;
            }

            return methodDeclaration;
        }

        public override AstNode VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, ICecilArgumentsResolver argumentsResolver) {
            namedArgumentExpression.Expression.AcceptVisitor(this, argumentsResolver);

            return namedArgumentExpression;
        }

        public override AstNode VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, ICecilArgumentsResolver argumentsResolver) {
            return nullReferenceExpression;
        }

        public override AstNode VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, ICecilArgumentsResolver argumentsResolver) {
            objectCreateExpression.Arguments.ForEach(a => a.AcceptVisitor(this, argumentsResolver));
            objectCreateExpression.Initializer.AcceptVisitor(this, argumentsResolver);
            objectCreateExpression.Type.AcceptVisitor(this, argumentsResolver);

            var memeberReference = objectCreateExpression.Annotation<MemberReference>();

            if (memeberReference != null) {
                var typeReference = memeberReference.DeclaringType;

                if (typeReference.IsGenericInstance) {
                    ResolveGenericInstance(typeReference, argumentsResolver);
                }
            }

            return objectCreateExpression;
        }

        private void ResolveGenericInstance(TypeReference typeReference, ICecilArgumentsResolver argumentsResolver) {
            var genericInstance = typeReference as GenericInstanceType;
            var arguments = genericInstance.GenericArguments.ToList();
            var genericArguments = genericInstance.GenericArguments;

            genericArguments.Clear();

            arguments.ForEach(a => {
                TypeReference argument = a;

                if (argument.NeedsTypeResolving()) {
                    argument = argumentsResolver.ResolveTypeReference(argument.FullName);
                }

                genericArguments.Add(argument);
            });
        }

        public override AstNode VisitParameterDeclaration(ParameterDeclaration parameterDeclaration, ICecilArgumentsResolver argumentsResolver) {
            parameterDeclaration.Type.AcceptVisitor(this, argumentsResolver);
            var parameterReference = parameterDeclaration.Annotation<ParameterReference>();

            if (parameterReference != null) {
                var parameterType = parameterReference.ParameterType;
                var genericTypeOrSelf = parameterType.GetGenericTypeOrSelfReference();

                if (genericTypeOrSelf.IsGeneric()) {
                    if (genericTypeOrSelf.IsGenericInstance) {
                        ResolveGenericInstance(genericTypeOrSelf, argumentsResolver);
                        return parameterDeclaration;
                    }

                    var pair = argumentsResolver.ResolvePair(genericTypeOrSelf.ToString());
                    TypeReference typeReference = pair.TypeReference;

                    if (parameterType.IsByReference) {
                        Type type = pair.Type.MakeByRefType();
                        typeReference = new TypeReference(string.Empty, type.FullName, typeReference.Module, typeReference.Scope);
                    }

                    parameterReference.ParameterType = typeReference;
                }
            }

            return parameterDeclaration;
        }

        public override AstNode VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, ICecilArgumentsResolver argumentsResolver) {
            return primitiveExpression;
        }

        public override AstNode VisitPrimitiveType(PrimitiveType primitiveType, ICecilArgumentsResolver argumentsResolver) {
            return primitiveType;
        }

        public override AstNode VisitReturnStatement(ReturnStatement returnStatement, ICecilArgumentsResolver argumentsResolver) {
            returnStatement.Expression.AcceptVisitor(this, argumentsResolver);

            return returnStatement;
        }

        public override AstNode VisitSimpleType(SimpleType simpleType, ICecilArgumentsResolver argumentsResolver) {
            Type type = null;
            var typeReference = simpleType.Annotation<TypeReference>();

            if (typeReference != null) {
                bool hasTypes = false;
                Type[] types = new Type[simpleType.TypeArguments.Count];

                if (typeReference.HasGenericParameters) {
                    simpleType.TypeArguments
                              .ForEach((a, i) => {
                                  type = a.GetActualType() ?? argumentsResolver.ResolveType(a.ToString());

                                  if (type != null) {
                                      types[i] = type;
                                      hasTypes = true;
                                  }
                              });

                    if (hasTypes) {
                        type = typeReference.GetActualType()
                                            .MakeGenericType(types);

                        simpleType.RemoveAnnotations<TypeReference>();
                        typeReference = new TypeReference(string.Empty, type.FullName, typeReference.Module, typeReference.Scope);
                        simpleType.AddAnnotation(typeReference);
                    }
                }
            }
            else {
                var pair = argumentsResolver.ResolvePair(simpleType.Identifier);

                type = pair.Type;
                typeReference = pair.TypeReference;
                simpleType.AddAnnotation(typeReference);
                simpleType.Identifier = pair.Type.FullName;
            }

            return simpleType;
        }

        public override AstNode VisitSwitchSection(SwitchSection switchSection, ICecilArgumentsResolver argumentsResolver) {
            switchSection.Statements.ForEach(s => s.AcceptVisitor(this, argumentsResolver));
            switchSection.CaseLabels.ForEach(c => c.AcceptVisitor(this, argumentsResolver));

            return switchSection;
        }

        public override AstNode VisitSwitchStatement(SwitchStatement switchStatement, ICecilArgumentsResolver argumentsResolver) {
            switchStatement.Expression.AcceptVisitor(this, argumentsResolver);
            switchStatement.SwitchSections.ForEach(s => s.AcceptVisitor(this, argumentsResolver));

            return switchStatement;
        }

        public override AstNode VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, ICecilArgumentsResolver argumentsResolver) {
            return thisReferenceExpression;
        }

        public override AstNode VisitThrowStatement(ThrowStatement throwStatement, ICecilArgumentsResolver argumentsResolver) {
            throwStatement.Expression.AcceptVisitor(this, argumentsResolver);

            return throwStatement;
        }

        public override AstNode VisitTryCatchStatement(TryCatchStatement tryCatchStatement, ICecilArgumentsResolver argumentsResolver) {
            tryCatchStatement.TryBlock.AcceptVisitor(this, argumentsResolver);
            tryCatchStatement.CatchClauses.ForEach(c => c.AcceptVisitor(this, argumentsResolver));
            tryCatchStatement.FinallyBlock.AcceptVisitor(this, argumentsResolver);

            return tryCatchStatement;
        }

        public override AstNode VisitTypeOfExpression(TypeOfExpression typeOfExpression, ICecilArgumentsResolver argumentsResolver) {
            typeOfExpression.Type.AcceptVisitor(this, argumentsResolver);

            return typeOfExpression;
        }

        public override AstNode VisitTypeParameterDeclaration(TypeParameterDeclaration typeParameterDeclaration, ICecilArgumentsResolver argumentsResolver) {
            return typeParameterDeclaration;
        }

        public override AstNode VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, ICecilArgumentsResolver argumentsResolver) {
            return typeReferenceExpression;
        }

        public override AstNode VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, ICecilArgumentsResolver argumentsResolver) {
            unaryOperatorExpression.Expression.AcceptVisitor(this, argumentsResolver);

            return unaryOperatorExpression;
        }

        public override AstNode VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, ICecilArgumentsResolver argumentsResolver) {
            variableDeclarationStatement.Type.AcceptVisitor(this, argumentsResolver);

            return variableDeclarationStatement;
        }

        public override AstNode VisitWhileStatement(WhileStatement whileStatement, ICecilArgumentsResolver argumentsResolver) {
            whileStatement.Condition.AcceptVisitor(this, argumentsResolver);
            whileStatement.EmbeddedStatement.AcceptVisitor(this, argumentsResolver);

            return whileStatement;
        }

        public override AstNode VisitYieldBreakStatement(YieldBreakStatement yieldBreakStatement, ICecilArgumentsResolver argumentsResolver) {
            return yieldBreakStatement;
        }

        public override AstNode VisitYieldReturnStatement(YieldReturnStatement yieldReturnStatement, ICecilArgumentsResolver argumentsResolver) {
            return yieldReturnStatement;
        }
    }
}