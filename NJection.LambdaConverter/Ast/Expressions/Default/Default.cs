using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Default : AstExpression
    {
        private NRefactory.DefaultValueExpression _defaultValueExpression = null;

        public Default(NRefactory.DefaultValueExpression defaultValueExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _defaultValueExpression = defaultValueExpression;
            InternalType = _defaultValueExpression.Type.AcceptVisitor(Visitor, ParentScope).Type;
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Default; }
        }

        public override Expression Reduce() {
            return Expression.Default(Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitDefault(this);
        }

        public Expression Update(Type type) {
            if (Type.Equals(type)) {
                return this;
            }

            return AstExpression.Default(_defaultValueExpression, ParentScope, Visitor);
        }
    }
}