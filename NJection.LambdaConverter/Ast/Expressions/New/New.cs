using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class New : AstExpression
    {
        private List<Expression> _arguments = null;
        private NRefactory.ObjectCreateExpression _objectCreation = null;

        private delegate Type ResolveDelegateType(params Type[] typeArgs);

        protected internal New(NRefactory.ObjectCreateExpression objectCreation, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            
            _objectCreation = objectCreation;

            if (!objectCreation.Type.IsNull) {
                InternalType = objectCreation.Type.AcceptVisitor(Visitor, ParentScope).Type;

                if (objectCreation.Initializer != null) {
                    if (objectCreation.Arguments.Count == 2) {
                        Expression expression;
                        NRefactory.Expression @this = objectCreation.Arguments.First();
                        NRefactory.Expression func = objectCreation.Arguments.Last();

                        if (TryHandleAnonymousMethod(@this, func as NRefactory.InvocationExpression, out expression)) {
                            Expression = expression;
                            return;
                        }
                    }

                    if (objectCreation.Initializer != NRefactory.ArrayInitializerExpression.Null) {
                        Expression = objectCreation.Initializer.AcceptVisitor(Visitor, ParentScope);
                        return;
                    }
                }

                Expression = BuildConstructor();
            }
            else {
                Expression = HandleAnonymousType();
            }
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.New; }
        }

        public Expression Expression { get; private set; }

        public override Expression Reduce() {
            return Expression;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitNew(this);
        }

        public Expression Update(Expression expression) {
            if (Expression.Equals(expression)) {
                return this;
            }

            return AstExpression.New(_objectCreation, ParentScope, Visitor);
        }

        private NewExpression BuildConstructor() {
            ConstructorInfo constructorInfo = null;
            Type[] constructorParameters = Type.EmptyTypes;

            if (_objectCreation.Arguments.Count > 0) {
                _arguments = new List<Expression>(_objectCreation.Arguments.Count);
                constructorParameters = new Type[_objectCreation.Arguments.Count];

                _objectCreation.Arguments.ForEach((arg, i) => {
                    var expression = arg.AcceptVisitor(Visitor, ParentScope);
                    _arguments.Add(expression);
                    constructorParameters[i] = expression.Type;
                });
            }

            constructorInfo = Type.GetConstructor(constructorParameters);

            return _arguments != null ? Expression.New(constructorInfo, _arguments)
                                      : Expression.New(constructorInfo);
        }

        private bool TryHandleAnonymousMethod(NRefactory.Expression @this, NRefactory.InvocationExpression func, out Expression outExpression) {
            MethodReference methodReference = null;
            NRefactory.IdentifierExpression methodIdentifier = null;

            outExpression = null;

            if (func != null) {
                methodIdentifier = func.Arguments.Single() as NRefactory.IdentifierExpression;
                methodReference = methodIdentifier.Annotation<MethodReference>();

                if (methodReference != null) {
                    object target = null;
                    Delegate @delegate = null;
                    var methodInfo = methodReference.GetActualMethod() as MethodInfo;

                    if (!methodInfo.IsStatic) {
                        var context = RootScope.Context;

                        if (context != null) {
                            target = context.Value;
                        }
                    }

                    @delegate = methodInfo.CreateDelegate(InternalType, target);
                    outExpression = Expression.Constant(@delegate);
                    return true;
                }
            }

            return false;
        }

        private Expression HandleAnonymousType() {
            Instruction instruction;
            MethodReference methodReference = null;
            ConstructorInfo constructorInfo = null;

            RootScope.TryGetInstruction(_objectCreation, OpCodes.Newobj, out instruction);
            methodReference = instruction.Operand as MethodReference;
            constructorInfo = methodReference.GetActualMethod<ConstructorInfo>();

            _arguments = _objectCreation.Initializer
                                        .Elements
                                        .Select(e => e.AcceptVisitor(Visitor, ParentScope))
                                        .ToListOf<Expression>();

            InternalType = constructorInfo.DeclaringType;

            return _arguments != null ? Expression.New(constructorInfo, _arguments)
                                      : Expression.New(constructorInfo);
        }
    }
}