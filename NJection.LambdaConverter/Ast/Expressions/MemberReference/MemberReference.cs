using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class MemberReference : AstExpression
    {
        private bool _isStatic = false;
        private Mono.Cecil.MemberReference _memberReference = null;
        private NRefactory.MemberReferenceExpression _memberReferenceExpression = null;

        public MemberReference(NRefactory.MemberReferenceExpression memberReference, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _memberReferenceExpression = memberReference;
            ResolveMember();
            BuildExpression();
        }

        public MemberInfo Member { get; private set; }

        public AstExpression Expression { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.MemberReference; }
        }

        public override Expression Reduce() {
            var @base = Expression as Base;
            var parent = _memberReferenceExpression.Parent as NRefactory.AssignmentExpression;
            var isAssignmentExpression = parent != null;

            if (@base != null) {
                if (isAssignmentExpression) {
                    return ReduceBaseAssignment(@base);
                }

                return ReduceBaseMemberAccess(@base);
            }

            switch (Member.MemberType) {
                case MemberTypes.Event:

                    return ReduceEvent(parent);

                case MemberTypes.Method:

                    return AstExpression.Call(Expression, Member as MethodInfo);
            }

            return AstExpression.MakeMemberAccess(Expression, Member);
        }

        private Expression ReduceEvent(NRefactory.AssignmentExpression assignmentExpression) {
            ExpressionType expressionType;

            if (!Enum.TryParse<ExpressionType>(assignmentExpression.Operator.ToString(), out expressionType)) {
                throw new InvalidOperationException("Event registration must have an add/subtract operator");
            }

            return AstExpression.Event(Expression, Member as EventInfo, expressionType, ParentScope, Visitor);
        }

        private Expression ReduceBaseMemberAccess(Base expression) {
            Delegate @delegate = null;
            MethodInfo methodInfo = null;
            ILGenerator ilGenerator = null;
            DynamicMethod dynamicMethod = null;
            Type delegateType = typeof(Func<,>);
            var expressionType = expression.Type;
            MethodDefinition methodDefinition = null;
            var propertyReference = _memberReferenceExpression.Annotation<PropertyDefinition>();

            methodDefinition = propertyReference.GetMethod;
            methodInfo = methodDefinition.GetActualMethod<MethodInfo>();
            dynamicMethod = new DynamicMethod(string.Format("Base_{0}", methodInfo.Name), methodInfo.ReturnType, new Type[] { expressionType }, expressionType);
            ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.EmitCall(OpCodes.Call, methodInfo, null);
            ilGenerator.Emit(OpCodes.Ret);
            delegateType = delegateType.MakeGenericType(expressionType, methodInfo.ReturnType);
            @delegate = dynamicMethod.CreateDelegate(delegateType);

            return AstExpression.Invoke(
                        AstExpression.Constant(@delegate),
                            Expression);
        }

        private Expression ReduceBaseAssignment(Base expression) {
            Type returnType = null;
            Delegate @delegate = null;
            MethodInfo methodInfo = null;
            ILGenerator ilGenerator = null;
            DynamicMethod dynamicMethod = null;
            Type delegateType = typeof(Func<,,>);
            MethodDefinition methodDefinition = null;
            var expressionType = expression.Type;
            ParameterExpression assignedValue = null;
            var propertyReference = _memberReferenceExpression.Annotation<PropertyDefinition>();

            methodDefinition = propertyReference.GetMethod;
            methodInfo = methodDefinition.GetActualMethod<MethodInfo>();
            returnType = InternalType = methodInfo.ReturnType;
            assignedValue = AstExpression.Parameter(returnType);
            methodDefinition = propertyReference.SetMethod;
            methodInfo = methodDefinition.GetActualMethod<MethodInfo>();

            dynamicMethod = new DynamicMethod(string.Format("Base_{0}", methodInfo.Name), returnType, new Type[] { expressionType, returnType }, expressionType);
            ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.EmitCall(OpCodes.Call, methodInfo, null);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ret);

            delegateType = delegateType.MakeGenericType(expressionType, returnType, returnType);
            @delegate = dynamicMethod.CreateDelegate(delegateType);

            return AstExpression.Lambda(
                        AstExpression.GetFuncType(returnType, returnType),
                            AstExpression.Invoke(AstExpression.Constant(@delegate), Expression, assignedValue),
                                new ParameterExpression[] { assignedValue },
                                    ParentScope, Visitor);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitMemberReference(this);
        }

        public Expression Update(MemberInfo member, Expression expression) {
            if (Member.Equals(member) && ReferenceEquals(Expression, expression)) {
                return this;
            }

            return AstExpression.MemberReference(_memberReferenceExpression, ParentScope, Visitor);
        }

        private void BuildExpression() {
            var target = _memberReferenceExpression.Target;
            string memberName = _memberReferenceExpression.MemberName;
            var astExpression = target.AcceptVisitor(Visitor, ParentScope);

            if (memberName.Equals("GetMethodFromHandle")) {
                BuildMethodHandleExpression();
                return;
            }

            if (memberName.Equals("GetTypeFromHandle")) {
                BuildTypeHandleExpression();
                return;
            }

            if (!_isStatic) {
                Expression = astExpression;
            }
        }

        private AstExpression BuildMethodHandleExpression() {
            var invocation = _memberReferenceExpression.Parent as NRefactory.InvocationExpression;
            return Expression = AstExpression.MethodOf(invocation, ParentScope, Visitor);
        }

        private AstExpression BuildTypeHandleExpression() {
            var memberExpression = _memberReferenceExpression.NextSibling as NRefactory.MemberReferenceExpression;
            return Expression = memberExpression.Target.AcceptVisitor(Visitor, ParentScope);
        }

        private void ResolveMember() {
            var @event = _memberReferenceExpression.Annotation<EventReference>();

            if (@event != null) {
                EventInfo eventInfo = null;

                _memberReference = @event;
                Member = eventInfo = @event.GetActualEvent();
                InternalType = eventInfo.EventHandlerType;
                _isStatic = false;
                return;
            }

            var field = _memberReferenceExpression.Annotation<FieldReference>();

            if (field != null) {
                FieldInfo fieldInfo = null;

                _memberReference = field;
                Member = fieldInfo = field.GetActualField();
                _isStatic = fieldInfo.IsStatic;
                InternalType = fieldInfo.FieldType;
                return;
            }

            var property = _memberReferenceExpression.Annotation<PropertyDefinition>();

            if (property != null) {
                MethodInfo methodInfo = null;
                PropertyInfo propertyInfo = null;
                var methodReference = _memberReferenceExpression.Annotation<MethodReference>();

                _memberReference = property;
                propertyInfo = property.GetActualProperty();
                methodInfo = methodReference.GetActualMethod<MethodInfo>();
                _isStatic = methodInfo.IsStatic;

                if (propertyInfo.CanWrite && propertyInfo.GetSetMethod().Equals(methodInfo)) {
                    Member = propertyInfo;
                    InternalType = propertyInfo.PropertyType;
                }
                else {
                    Member = methodInfo;
                    InternalType = methodInfo.ReturnType;
                }

                return;
            }

            var method = _memberReferenceExpression.Annotation<MethodReference>() ??
                              _memberReferenceExpression.Parent.Annotation<MethodReference>();

            if (method != null) {
                MethodInfo methodInfo = null;

                _memberReference = method;
                Member = methodInfo = method.GetActualMethod<MethodInfo>();
                _isStatic = methodInfo.IsStatic;
                InternalType = methodInfo.ReturnType;
                return;
            }
        }
    }
}