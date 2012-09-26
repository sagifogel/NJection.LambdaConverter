using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Invocation : AstExpression
    {
        private Func<Expression> _reduceFunction = null;
        private NRefactory.InvocationExpression _invocationExpression = null;

        protected internal Invocation(NRefactory.InvocationExpression invocationExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            
            var methodReference = invocationExpression.Annotation<Mono.Cecil.MethodReference>();

            _invocationExpression = invocationExpression;

            if (methodReference != null) {
                MethodInfo methodInfo = null;
                Member = methodInfo = methodReference.GetActualMethod<MethodInfo>();

                if (IsInitializeArray(methodInfo)) {
                    var first = _invocationExpression.Arguments.First().AcceptVisitor(Visitor, ParentScope);
                    var invocation = _invocationExpression.Arguments.Last() as NRefactory.InvocationExpression;
                    var second = invocation.Arguments.First();
                    var memberReference = invocationExpression.Target as NRefactory.MemberReferenceExpression;
                    var target = memberReference.Target as NRefactory.TypeReferenceExpression;
                    var type = target.Type.GetActualType();
                    var parameters = methodReference.Parameters;
                    return;
                }
            }

            BuildInvocation();
        }

        public Expression Instance { get; private set; }

        public MemberInfo Member { get; private set; }

        public IEnumerable<Expression> MethodArguments { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Invocation; }
        }

        public override Expression Reduce() {
            return _reduceFunction();
        }

        private Expression ReduceToMethodInvocation() {
            return Expression.Call(Instance, Member as MethodInfo, MethodArguments);
        }

        private Expression ReduceToFieldInvocation() {
            return Expression.Invoke(
                        Expression.Field(Instance, Member as FieldInfo),
                            MethodArguments);
        }

        private Expression ReduceToPropertyInvocation() {
            return Expression.Invoke(
                        Expression.Property(Instance, Member as MethodInfo),
                            MethodArguments);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitInvocationExpression(this);
        }

        public Expression Update(MemberInfo memberInfo, IEnumerable<Expression> arguments, object instance) {
            if (Member.Equals(memberInfo) && ReferenceEquals(MethodArguments, arguments) && ReferenceEquals(Instance, instance)) {
                return this;
            }

            return AstExpression.Invocation(_invocationExpression, ParentScope, Visitor);
        }

        private void BuildPropertyInvocation(MemberReference memberReference) {
            var propertyInfo = memberReference.Member as PropertyInfo;

            Instance = memberReference.Expression;
            Member = propertyInfo.GetGetMethod();
            MethodArguments = GetArguments(Member);
            InternalType = propertyInfo.PropertyType.GetMethod("Invoke").ReturnType;
        }

        private void BuildFieldInvocation(MemberReference memberReference) {
            FieldInfo fieldInfo = null;

            Instance = memberReference.Expression;
            Member = fieldInfo = memberReference.Member as FieldInfo;
            MethodArguments = GetArguments(Member);
            InternalType = fieldInfo.FieldType.GetMethod("Invoke").ReturnType;
        }

        private void BuildMethodInvocation(MemberReference memberReference) {
            MethodInfo methodInfo = null;

            Instance = memberReference.Expression;
            Member = methodInfo = memberReference.Member as MethodInfo;
            InternalType = methodInfo.ReturnType;
            MethodArguments = GetArguments(Member);
        }

        private void BuildGetHandleInvocation(MemberReference memberReference) {
            MethodInfo methodInfo = null;

            MethodArguments = new List<Expression> { memberReference.Expression };
            Member = methodInfo = memberReference.Member as MethodInfo;
            InternalType = methodInfo.ReturnType;
        }

        private void BuildAnonymousMethodInvocation(Identifier identifier) {
            MethodInfo methodInfo = null;
            var arguments = _invocationExpression.Arguments;
            Type methodType = identifier.Expression.Type;

            Member = methodInfo = methodType.GetMethod("Invoke");
            Instance = identifier.Expression;
            InternalType = methodInfo.ReturnType;
            MethodArguments = GetArguments(Member);
        }

        private void BuildInvocation() {
            var target = _invocationExpression.Target;
            var expression = target.AcceptVisitor(Visitor, ParentScope);

            switch (expression.AstNodeType) {
                case AstExpressionType.MemberReference:

                    var memberReference = expression as MemberReference;
                    var memberType = memberReference.Member.MemberType;
                    Action<MemberReference> action = null;

                    switch (memberType) {
                        case MemberTypes.Event:
                        case MemberTypes.TypeInfo:

                            throw new NotSupportedException();

                        case MemberTypes.Field:

                            action = BuildFieldInvocation;
                            _reduceFunction = ReduceToFieldInvocation;
                            break;

                        case MemberTypes.Method:

                            var astExpression = memberReference.Expression as AstExpression;

                            if (astExpression != null) {
                                if (astExpression.AstNodeType == AstExpressionType.MethodOf ||
                                    astExpression.AstNodeType == AstExpressionType.TypeOf) {
                                    action = BuildGetHandleInvocation;
                                }
                                else {
                                    action = BuildMethodInvocation;
                                }
                            }
                            else {
                                action = BuildMethodInvocation;
                            }

                            _reduceFunction = ReduceToMethodInvocation;
                            break;

                        case MemberTypes.Property:

                            action = BuildPropertyInvocation;
                            _reduceFunction = ReduceToPropertyInvocation;
                            break;
                    }

                    action(memberReference);
                    break;

                case AstExpressionType.Identifer:

                    var identifier = expression as Identifier;
                    BuildAnonymousMethodInvocation(identifier);
                    _reduceFunction = ReduceToMethodInvocation;
                    break;

                case AstExpressionType.Invocation:

                    var invocation = expression as Invocation;
                    var methodReference = _invocationExpression.Annotation<MethodReference>();
                    MethodInfo methodInfo = methodReference.GetActualMethod<MethodInfo>();
                    var returnType = methodInfo.ReturnType;

                    MethodArguments = GetArguments(methodInfo);
                    Instance = invocation;
                    Member = methodInfo;
                    InternalType = methodInfo.ReturnType;
                    _reduceFunction = ReduceToMethodInvocation;
                    break;

                default:

                    throw new NotSupportedException();
            }
        }

        private bool IsInitializeArray(MethodInfo methodInfo) {
            if (methodInfo == null) {
                return false;
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type[] parameterTypes = new Type[] { typeof(Array), typeof(RuntimeFieldHandle) };

            if (!methodInfo.Name.Equals("InitializeArray") || !methodInfo.ReturnType.Equals(TypeSystem.Void) || parameters.Length != parameterTypes.Length) {
                return false;
            }

            return parameters.All((p, i) => p.ParameterType.Equals(parameterTypes[i]));
        }

        private List<Expression> GetArguments(MemberInfo memberInfo) {
            List<Expression> arguments = null;

            if (_invocationExpression.Arguments.Count > 0) {
                Type[] types = Type.EmptyTypes;
                bool checkArguments = memberInfo.MemberType == MemberTypes.Method;

                if (checkArguments) {
                    var methodInfo = memberInfo as MethodInfo;
                    types = methodInfo.GetParameters()
                                      .Select(p => p.ParameterType)
                                      .ToArray();
                }

                arguments = new List<Expression>(_invocationExpression.Arguments.Count);

                _invocationExpression.Arguments.ForEach((arg, i) => {
                    Expression argument = arg.AcceptVisitor(Visitor, ParentScope);

                    if (checkArguments) {
                        argument = TryCastArgument(types[i], argument);
                    }

                    arguments.Add(argument);
                });
            }

            return arguments;
        }

        private Expression TryCastArgument(Type parameterType, Expression argument) {
            Type argumentType = argument.Type;
            bool typesNotEqual = !TypesAreEqual(argumentType, parameterType);
            bool typeIsNotAssignable = !parameterType.IsAssignableFrom(argumentType);
            bool argumentShouldBeBoxed = ArgumentShouldBeBoxed(parameterType, argumentType);

            if (typesNotEqual && (typeIsNotAssignable || argumentShouldBeBoxed)) {
                argument = Expression.Convert(argument, parameterType);
            }

            return argument;
        }

        private bool ArgumentShouldBeBoxed(Type first, Type second) {
            return !first.IsValueType && second.IsValueType;
        }

        private bool TypesAreEqual(Type first, Type second) {
            if (!first.Equals(second)) {
                if (first.IsByRef) {
                    return first.Equals(second.MakeByRefType());
                }

                if (second.IsByRef) {
                    return second.Equals(first.MakeByRefType());
                }

                return false;
            }

            return true;
        }
    }
}