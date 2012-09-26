using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class IfElseCondition : Condition<NRefactory.IfElseStatement>
    {
        public IfElseCondition(NRefactory.IfElseStatement ifElseStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(ifElseStatement, scope, visitor) { }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.IfElseCondition; }
        }

        protected override NRefactory.AstNode TrueCondition {
            get { return ConditionNode.TrueStatement; }
        }

        protected override NRefactory.AstNode FalseCondition {
            get { return ConditionNode.FalseStatement; }
        }

        protected override NRefactory.AstNode TestCondition {
            get { return ConditionNode.Condition; }
        }

        public override Expression Reduce() {
            if (IfFalse != null) {
                return Expression.IfThenElse(Test, IfTrue, IfFalse);
            }

            return Expression.IfThen(Test, IfTrue);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitIfElseCondition(this);
        }

        protected override Expression CreateConditionalExpression() {
            return AstExpression.IfElseCondition(ConditionNode, ParentScope, Visitor);
        }
    }
}