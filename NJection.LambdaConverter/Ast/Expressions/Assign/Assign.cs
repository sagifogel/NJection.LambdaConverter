using System.Linq.Expressions;
using Mono.Cecil.Cil;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Assign : AstExpression
    {
        private bool _isInvocable = false;
        private AstExpressionType _invokedType;
        private AstExpression _rightExpression = null;
        private NRefactory.AssignmentExpression _assignment = null;

        protected internal Assign(NRefactory.AssignmentExpression assignmentExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            
            AstExpression left = null;

            _assignment = assignmentExpression;
            Right = _rightExpression = assignmentExpression.Right.AcceptVisitor(Visitor, ParentScope);
            left = assignmentExpression.Left.AcceptVisitor(Visitor, ParentScope);
            Left = left.Reduce();
            _isInvocable = IsInvocable(Left, out _invokedType);
            InternalType = left.Type;
        }

        public Expression Left { get; private set; }

        public Expression Right { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Assign; }
        }

        public override Expression Reduce() {
            if (!Left.Type.Equals(Right.Type)) {
                if (RightOperandShouldBeBoxed()) {
                    Right = Expression.Convert(Right, Left.Type);
                }
            }

            if (_isInvocable) {
                if (_invokedType == AstExpressionType.Event) {
                    InternalType = TypeSystem.Void;
                }

                return Expression.Invoke(Left, Right);
            }

            return Expression.Assign(Left, Right);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitAssign(this);
        }

        public Expression Update(Expression left, Expression right) {
            if (Left.Equals(left) && Right.Equals(right)) {
                return this;
            }

            return AstExpression.Assign(_assignment, ParentScope, Visitor);
        }

        private bool IsInvocable(Expression expression, out AstExpressionType invokedType) {
            invokedType = default(AstExpressionType);
            var astExpression = expression as AstExpression;

            if (astExpression != null) {
                if (astExpression.AstNodeType == AstExpressionType.Event || astExpression.AstNodeType == AstExpressionType.Lambda) {
                    invokedType = astExpression.AstNodeType;
                    return true;
                }
            }

            return false;
        }

        private bool RightOperandShouldBeBoxed() {
            Instruction instruction;

            return RootScope.TryGetInstruction(_assignment, OpCodes.Box, out instruction) ||
                    _rightExpression.AstNodeType == AstExpressionType.NullReference;
        }
    }
}