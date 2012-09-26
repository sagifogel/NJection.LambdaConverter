using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Init : AstExpression
    {
        private NRefactory.VariableInitializer _variableInitializer = null;

        protected internal Init(NRefactory.VariableInitializer variableInitializer, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            NRefactory.Expression initializer = null;

            _variableInitializer = variableInitializer;
            initializer = variableInitializer.Initializer;
        }

        public Expression Value { get; private set; }

        public LabelTarget Target { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Init; }
        }

        public override Expression Reduce() {
            if (Value == null) {
                return Expression.Break(Target, Type);
            }

            return Expression.Break(Target, Value, Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitInit(this);
        }

        public Expression Update(LabelTarget target, Type type, Expression value) {
            if (Target.Equals(target) && Type.Equals(type) && Value.Equals(value)) {
                return this;
            }

            return AstExpression.Init(_variableInitializer, ParentScope, Visitor);
        }
    }
}