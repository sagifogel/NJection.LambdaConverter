using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public abstract class Condition<T> : AstExpression where T : NRefactory.AstNode
    {
        protected T ConditionNode = null;

        public Condition(T conditionalNode, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            ConditionNode = conditionalNode;
            VisitChildren();
        }

        public Expression IfTrue { get; private set; }

        public Expression IfFalse { get; private set; }

        public Expression Test { get; private set; }

        #region Abstract

        protected abstract NRefactory.AstNode TrueCondition { get; }

        protected abstract NRefactory.AstNode FalseCondition { get; }

        protected abstract NRefactory.AstNode TestCondition { get; }

        protected abstract Expression CreateConditionalExpression();

        #endregion Abstract

        protected void VisitChildren() {
            Test = TestCondition.AcceptVisitor(Visitor, ParentScope);
            IfTrue = TrueCondition.AcceptVisitor(Visitor, ParentScope);
            IfFalse = FalseCondition.AcceptVisitor(Visitor, ParentScope);
            InternalType = IfTrue.Type;
        }

        public Expression Update(Expression test, Expression ifTrue, Expression ifFalse = null) {
            if (Test.Equals(test) && IfTrue.Equals(ifTrue) && IfFalse == ifFalse) {
                return this;
            }

            return CreateConditionalExpression();
        }
    }
}