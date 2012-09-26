using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class TypeExpression : AstExpression
    {
        private NRefactory.AstType _astType = null;
        private TypeDescription _typeDescription;

        protected internal TypeExpression(NRefactory.MemberType memberType, IScope scope, INRefcatoryExpressionVisitor visitor)
            : this(memberType, TypeDescription.Memeber, scope, visitor) {
            InternalType = memberType.GetActualType();
        }

        protected internal TypeExpression(NRefactory.PrimitiveType primitiveType, IScope scope, INRefcatoryExpressionVisitor visitor)
            : this(primitiveType, TypeDescription.Primitive, scope, visitor) {
            InternalType = primitiveType.GetActualType();
        }

        protected internal TypeExpression(NRefactory.SimpleType simpleType, IScope scope, INRefcatoryExpressionVisitor visitor)
            : this(simpleType, TypeDescription.Simple, scope, visitor) {
            InternalType = simpleType.GetActualType();
        }

        public TypeExpression(NRefactory.ComposedType composedType, IScope scope, INRefcatoryExpressionVisitor visitor)
            : this(composedType, TypeDescription.Composed, scope, visitor) {
            InternalType = composedType.GetActualType();
        }

        private TypeExpression(NRefactory.AstType astType, TypeDescription typeDescription, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _astType = astType;
            _typeDescription = typeDescription;
        }

        public string Name { get; set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Type; }
        }

        public override Expression Reduce() {
            return Expression.Parameter(Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitType(this);
        }

        public Expression Update(Type type) {
            TypeExpression typeExpression = null;

            if (Type.Equals(type)) {
                return this;
            }

            switch (_typeDescription) {
                case TypeDescription.Memeber:

                    typeExpression = AstExpression.TypeExpression(_astType as NRefactory.MemberType, ParentScope, Visitor);
                    break;

                case TypeDescription.Primitive:

                    typeExpression = AstExpression.TypeExpression(_astType as NRefactory.PrimitiveType, ParentScope, Visitor);
                    break;

                case TypeDescription.Simple:

                    typeExpression = AstExpression.TypeExpression(_astType as NRefactory.SimpleType, ParentScope, Visitor);
                    break;

                case TypeDescription.Composed:

                    typeExpression = AstExpression.TypeExpression(_astType as NRefactory.ComposedType, ParentScope, Visitor);
                    break;
            }

            return typeExpression;
        }

        private enum TypeDescription
        {
            Memeber,
            Primitive,
            Simple,
            Composed
        }
    }
}