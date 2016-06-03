using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using System;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using ICSharpCode.Decompiler.Ast;

namespace NJection.LambdaConverter.Expressions
{
    public class NamedExpression : AstExpression
    {
        private NRefactory.NamedExpression _namedExpression = null;

        protected internal NamedExpression(NRefactory.NamedExpression namedExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            Name = namedExpression.Identifier;
            _namedExpression = namedExpression;
            Expression = _namedExpression.Expression.AcceptVisitor(Visitor, ParentScope);
            InternalType = Expression.Type;
        }

        public string Name { get; private set; }

        public Expression Expression { get; set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.NamedExpression; }
        }

        public override Expression Reduce() {
            return Expression;
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitNamedExpression(this);
        }

        public Expression Update(Expression argument) {
            if (Expression.Equals(argument)) {
                return this;
            }

            return AstExpression.NamedExpression(_namedExpression, ParentScope, Visitor);
        }
    }
}