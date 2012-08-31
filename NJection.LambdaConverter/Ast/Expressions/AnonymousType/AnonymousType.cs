using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ICSharpCode.Decompiler.Ast;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class AnonymousType : AstExpression
    {
        private IEnumerable<Expression> _initializers = null;
        private NRefactory.AnonymousTypeCreateExpression _anonymousTypeCreateExpression;

        protected internal AnonymousType(NRefactory.AnonymousTypeCreateExpression anonymousTypeCreateExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            var typeInformation = anonymousTypeCreateExpression.Annotation<TypeInformation>();

            _anonymousTypeCreateExpression = anonymousTypeCreateExpression;
            _initializers = _anonymousTypeCreateExpression.Initializers.Select(i => i.AcceptVisitor(Visitor, ParentScope));
            InternalType = typeInformation.InferredType.GetActualType();
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.AnonymousType; }
        }

        public override Expression Reduce() {
            var ctor = InternalType.GetConstructors()[0];

            return Expression.New(ctor, _initializers);
        }

        public Expression Update(Type type) {
            if (Type.Equals(type)) {
                return this;
            }

            return AstExpression.AnonymousType(_anonymousTypeCreateExpression, ParentScope, Visitor);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitAnonymousTypeCreation(this);
        }
    }
}