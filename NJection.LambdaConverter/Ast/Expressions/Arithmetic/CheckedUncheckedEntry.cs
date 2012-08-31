using System;
using System.Linq.Expressions;

namespace NJection.LambdaConverter.Expressions.Arithmetic
{
    internal class CheckedUncheckedEntry
    {
        private Func<Expression, Expression, BinaryExpression> _checked = null;
        private Func<Expression, Expression, BinaryExpression> _unchecked = null;

        internal CheckedUncheckedEntry(Func<Expression, Expression, BinaryExpression> @unchecked, Func<Expression, Expression, BinaryExpression> @checked) {
            _checked = @checked;
            _unchecked = @unchecked;
        }

        internal Func<Expression, Expression, BinaryExpression> this[bool isChecked] {
            get {
                return isChecked ? _checked : _unchecked;
            }
        }
    }
}