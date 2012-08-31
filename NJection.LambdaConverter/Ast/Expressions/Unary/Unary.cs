using System;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Unary : AstExpression
    {
        private NRefactory.UnaryOperatorExpression _unaryOperatorExpression = null;

        protected internal Unary(NRefactory.UnaryOperatorExpression unaryOperatorExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _unaryOperatorExpression = unaryOperatorExpression;
            Operand = unaryOperatorExpression.Expression.AcceptVisitor(Visitor, ParentScope).Reduce();
            InternalType = Operand.Type;
        }

        public Expression Operand { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Unary; }
        }

        public override Expression Reduce() {
            NRefactory.UnaryOperatorType @operator = _unaryOperatorExpression.Operator;
            ExpressionType unaryType = GetUnaryOperator(@operator);

            return Expression.MakeUnary(unaryType, Operand, Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitUnary(this);
        }

        public Expression Update(Expression operand, Type type) {
            if (Operand.Equals(operand) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.Unary(_unaryOperatorExpression, ParentScope, Visitor);
        }

        private ExpressionType GetUnaryOperator(NRefactory.UnaryOperatorType @operator) {
            ExpressionType type;

            if (!Enum.TryParse<ExpressionType>(@operator.ToString(), true, out type)) {
                switch (@operator) {
                    case ICSharpCode.NRefactory.CSharp.UnaryOperatorType.BitNot:

                        return ExpressionType.Not;

                    case ICSharpCode.NRefactory.CSharp.UnaryOperatorType.Decrement:

                        return ExpressionType.Decrement;

                    case ICSharpCode.NRefactory.CSharp.UnaryOperatorType.Increment:

                        return ExpressionType.Increment;

                    case ICSharpCode.NRefactory.CSharp.UnaryOperatorType.Minus:

                        return ExpressionType.Negate;

                    case ICSharpCode.NRefactory.CSharp.UnaryOperatorType.Not:

                        return ExpressionType.NotEqual;

                    case ICSharpCode.NRefactory.CSharp.UnaryOperatorType.Plus:

                        return ExpressionType.Add;

                    case ICSharpCode.NRefactory.CSharp.UnaryOperatorType.PostDecrement:

                        return ExpressionType.PostDecrementAssign;

                    case ICSharpCode.NRefactory.CSharp.UnaryOperatorType.PostIncrement:

                        return ExpressionType.PostIncrementAssign;

                    default:

                        throw new NotSupportedException();
                }
            }

            return type;
        }
    }
}