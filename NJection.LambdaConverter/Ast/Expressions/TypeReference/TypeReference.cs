using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class TypeReference : AstExpression
    {
        private NRefactory.TypeReferenceExpression _typeReferenceExpression = null;

        protected internal TypeReference(NRefactory.TypeReferenceExpression typeReferenceExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _typeReferenceExpression = typeReferenceExpression;
            InternalType = _typeReferenceExpression.Type.AcceptVisitor(Visitor, ParentScope).Type;
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.TypeReference; }
        }

        public override Expression Reduce() {
            return Expression.Constant(InternalType);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitTypeReference(this);
        }

        public Expression Update(Type type) {
            if (Type.Equals(type)) {
                return this;
            }

            return AstExpression.TypeReference(_typeReferenceExpression, ParentScope, Visitor);
        }
    }
}