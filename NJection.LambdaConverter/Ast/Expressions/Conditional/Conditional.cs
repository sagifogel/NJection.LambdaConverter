using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Conditional : Condition<NRefactory.ConditionalExpression>
    {
        public Conditional(NRefactory.ConditionalExpression conditionalExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(conditionalExpression, scope, visitor) { }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Conditional; }
        }

        protected override NRefactory.AstNode TrueCondition {
            get { return ConditionNode.TrueExpression; }
        }

        protected override NRefactory.AstNode FalseCondition {
            get { return ConditionNode.FalseExpression; }
        }

        protected override NRefactory.AstNode TestCondition {
            get { return ConditionNode.Condition; }
        }

        public override Expression Reduce() {
            return Expression.Condition(Test, IfTrue, IfFalse);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitCondition(this);
        }

        protected override Expression CreateConditionalExpression() {
            return AstExpression.Condition(ConditionNode, ParentScope, Visitor);
        }
    }
}