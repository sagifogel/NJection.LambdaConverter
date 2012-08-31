using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;

namespace NJection.LambdaConverter.Expressions
{
    public class Lambda : AstExpression
    {
        internal protected Lambda(Type type, Expression body, IEnumerable<ParameterExpression> parameters, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            
            Body = body;
            Parameters = parameters;
            InternalType = type;
            ReturnType = type.GetMethod("Invoke").ReturnType;
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Lambda; }
        }

        public Type ReturnType { get; private set; }
        public Expression Body { get; private set; }
        public IEnumerable<ParameterExpression> Parameters { get; private set; }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitLambda(this);
        }

        public override Expression Reduce() {
            return Expression.Lambda(InternalType, Body, Parameters);
        }

        public Expression Update(Expression body, Type type, IEnumerable<ParameterExpression> parameters) {
            if (Body.Equals(body) && Type.Equals(type) && ReferenceEquals(Parameters, parameters)) {
                return this;
            }

            return AstExpression.Lambda(InternalType, Body, Parameters, ParentScope, Visitor);
        }
    }
}