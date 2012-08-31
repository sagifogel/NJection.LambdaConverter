using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract class Declaration<T> : AstExpression where T : NRefactory.AstNode
    {
        private bool _isByRef = false;
        protected T DeclarationValue = null;

        protected internal Declaration(T declaration, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            DeclarationValue = declaration;
        }

        public string Name { get; set; }

        public bool IsByRef {
            get { return _isByRef; }
        }

        public override Expression Reduce() {
            return Expression.Parameter(Type, Name);
        }

        public Expression Update(Type type, string name) {
            if (Type.Equals(type) && Name == name) {
                return this;
            }

            return CreateDeclaration();
        }

        protected internal void MarkAsByRef() {
            _isByRef = true;
        }

        protected bool TypeIsVar(NRefactory.AstNode type) {
            var simpleType = type as NRefactory.SimpleType;

            if (simpleType != null) {
                return simpleType.Identifier.Equals("var");
            }

            return false;
        }

        protected abstract Expression CreateDeclaration();
    }
}