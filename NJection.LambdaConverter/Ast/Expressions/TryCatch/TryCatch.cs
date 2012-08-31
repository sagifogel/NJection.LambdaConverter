using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class TryCatch : Scope
    {
        private NRefactory.TryCatchStatement _tryCatch = null;

        protected internal TryCatch(NRefactory.TryCatchStatement tryCatchStatement, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(scope, visitor) {
            _tryCatch = tryCatchStatement;
            BuildTryCatchBlock();
            InternalType = Body.Type;
        }

        public Expression Body { get; private set; }

        public Expression Finally { get; private set; }

        public IEnumerable<CatchBlock> Handlers { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.TryCatch; }
        }

        public override Expression Reduce() {
            if (Handlers != null) {
                var handlers = Handlers.ToArray();

                if (Finally != null) {
                    return Expression.TryCatchFinally(Body, Finally, handlers);
                }

                var tryCatch = Expression.TryCatch(Body, handlers);

                return tryCatch;
            }

            return Expression.TryFinally(Body, Finally);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitTryCatch(this);
        }

        public Expression Update(Expression body, IEnumerable<CatchBlock> handlers, Expression @finally) {
            if (Body.Equals(body) && Finally.Equals(@finally) && Handlers.Equals(handlers)) {
                return this;
            }

            return AstExpression.TryCatch(_tryCatch, ParentScope, Visitor);
        }

        private void BuildTryCatchBlock() {
            BuildTryBlock(_tryCatch.TryBlock);
            BuildCatchClauses(_tryCatch.CatchClauses);
            BuildFinallyBlock(_tryCatch.FinallyBlock);
        }

        private void BuildTryBlock(NRefactory.BlockStatement tryBlock) {
            Body = tryBlock.AcceptVisitor(Visitor, this);
        }

        private void BuildCatchClauses(NRefactory.AstNodeCollection<NRefactory.CatchClause> catchClauses) {
            Handlers = catchClauses.Select(@catch => {
                var catchClause = @catch.AcceptVisitor(Visitor, this) as CatchClause;
                Expression body = catchClause.Reduce();
                Type type = @catch.Type.AcceptVisitor(Visitor, this).Type;

                if (catchClause.ExceptionVariable != null) {
                    return Expression.Catch(catchClause.ExceptionVariable, body);
                }

                return Expression.Catch(type, body);
            });
        }

        private void BuildFinallyBlock(NRefactory.BlockStatement finallyBlack) {
            Finally = finallyBlack.AcceptVisitor(Visitor, this);
        }
    }
}