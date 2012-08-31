using System;
using System.Linq;
using System.Linq.Expressions;
using ICSharpCode.Decompiler.Ast.Transforms;
using NJection.LambdaConverter.Visitors;
using Ast_Expression = NJection.LambdaConverter.Expressions.AstExpression;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class Cast : AstExpression
    {
        private NRefactory.CastExpression _castExpression = null;

        protected internal Cast(NRefactory.CastExpression castExpression, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _castExpression = castExpression;
            Operand = castExpression.Expression.AcceptVisitor(Visitor, ParentScope);
            InternalType = castExpression.Type.AcceptVisitor(Visitor, ParentScope).Type;
        }

        public Ast_Expression Operand { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Cast; }
        }

        public override Expression Reduce() {
            Func<Expression, Type, Expression> convert = null;
            var checkedUncheckedAnnotation = _castExpression.Annotations.FirstOrDefault(a => a.Equals(AddCheckedBlocks.CheckedAnnotation));

            if (checkedUncheckedAnnotation == AddCheckedBlocks.CheckedAnnotation) {
                if (Operand.AstNodeType == AstExpressionType.Unary) {
                    return new ConvertCheckedVisitor().Visit(Operand);
                }

                convert = Expression.ConvertChecked;
            }
            else {
                convert = Expression.Convert;
            }

            return convert(Operand, Type);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitCast(this);
        }

        public Expression Update(Expression operand, Type type) {
            if (Operand.Equals(operand) && Type.Equals(type)) {
                return this;
            }

            return AstExpression.Cast(_castExpression, ParentScope, Visitor);
        }
    }
}