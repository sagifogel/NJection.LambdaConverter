using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class TypeOf : AstExpression
    {
        private Expression _typeReference = null;
        private NRefactory.TypeOfExpression _typeOfExpression = null;
        private static readonly Type _type = typeof(RuntimeTypeHandle);

        protected internal TypeOf(NRefactory.TypeOfExpression typeOfExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _typeOfExpression = typeOfExpression;
            _typeReference = _typeOfExpression.Type.AcceptVisitor(visitor, ParentScope);
            InternalType = _type;
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.TypeOf; }
        }

        public override Expression Reduce() {
            return Expression.Constant(_typeReference.Type.TypeHandle);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitTypeOf(this);
        }

        public Expression Update(Type type) {
            if (Type.Equals(type)) {
                return this;
            }

            return AstExpression.TypeOf(_typeOfExpression, ParentScope, Visitor);
        }
    }
}