using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class While : Scope
    {
        private NRefactory.WhileStatement _whileStatement = null;

        protected internal While(NRefactory.WhileStatement whileStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _whileStatement = whileStatement;
            BuildWhileBlock();
            InternalType = Body.Type;
        }

        public Expression Body { get; private set; }

        public Expression ConditionExpression { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.While; }
        }

        public override Expression Reduce() {
            var innerBreak = Expression.Label("break");
            var innerContinue = Expression.Label("continue");
            var loopVisitor = new LoopExpressionVisitor(innerBreak, innerContinue);

            var loopExpression = Expression.Loop(
                                    Expression.Block(
                                        Expression.Condition(
                                            ConditionExpression,
                                                Expression.Block(
                                                    Body,
                                                    Expression.Goto(innerContinue)),
                                                    Expression.Goto(innerBreak))),
                                                    innerBreak, innerContinue);

            return loopVisitor.Visit(loopExpression);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitWhile(this);
        }

        public Expression Update(Expression body, Expression condition) {
            if (Body.Equals(body) && ConditionExpression.Equals(condition)) {
                return this;
            }

            return AstExpression.While(_whileStatement, this, Visitor);
        }

        private void BuildWhileBlock() {
            ConditionExpression = _whileStatement.Condition.AcceptVisitor(Visitor, this);
            Body = _whileStatement.EmbeddedStatement.AcceptVisitor(Visitor, this);
        }
    }
}