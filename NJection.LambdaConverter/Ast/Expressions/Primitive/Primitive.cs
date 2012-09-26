using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Primitive : AstExpression
    {
        private NRefactory.PrimitiveExpression _primitiveExpression = null;

        protected internal Primitive(NRefactory.PrimitiveExpression primitiveExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _primitiveExpression = primitiveExpression;
            Value = primitiveExpression.Value;
            InternalType = Value.GetType();
        }

        public object Value { get; set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Primitive; }
        }

        public override Expression Reduce() {
            return Expression.Constant(Value, Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitPrimitive(this);
        }

        public Expression Update(object value, Type type) {
            if (Value.Equals(value) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.Primitive(_primitiveExpression, ParentScope, Visitor);
        }
    }
}