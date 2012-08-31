using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public partial class Block : Scope
    {
        private NRefactory.BlockStatement _blockStatement = null;
        protected IEnumerable<ParameterExpression> Parameters = null;
        protected Collection<Expression> BlockExpressions = new Collection<Expression>();

        protected internal Block(NRefactory.BlockStatement blockStatement, IScope scope, INRefcatoryExpressionVisitor visitor, IEnumerable<ParameterExpression> parameters = null)
            : base(scope, visitor) {

            _blockStatement = blockStatement;
            InternalType = ResolveType(blockStatement);
            Parameters = parameters;
            BuildExpressions();
        }

        public virtual Type ResolveType(NRefactory.BlockStatement blockStatement) {
            return TypeSystem.Void;
        }

        public IEnumerable<Expression> Expressions {
            get { return BlockExpressions; }
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Block; }
        }

        public override Expression Reduce() {
            if (BlockExpressions.Count == 0) {
                return Expression.Empty();
            }

            int i = BlockExpressions.Count - 1;
            Type lastType = BlockExpressions[i].Type;

            if (!Type.Equals(lastType) && Type.Equals(TypeSystem.Void)) {
                BlockExpressions.Add(Expression.Empty());
            }

            return Expression.Block(Type, Variables, BlockExpressions);
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitBlock(this);
        }

        public Expression Update(IEnumerable<ParameterExpression> variables, IEnumerable<Expression> parameters) {
            if (Variables.Equals(variables) && Expressions.Equals(parameters)) {
                return this;
            }

            return AstExpression.Block(_blockStatement, this, Visitor);
        }

        protected virtual void BuildExpressions() {
            bool isVariable = true;
            IEnumerable<NRefactory.Statement> statements = null;
            var groups = _blockStatement.Statements.GroupBy(s => s is NRefactory.VariableDeclarationStatement)
                                                   .ToDictionary(group => group.Key,
                                                                 group => group.AsEnumerable());

            if (groups.TryGetValue(isVariable, out statements)) {
                VariablesStore = statements.Select(s => s.AcceptVisitor(Visitor, this))
                                           .Cast<VariableDeclaration>()
                                           .Select(e => new {
                                               Name = e.Name,
                                               Expression = e.Reduce() as ParameterExpression
                                           })
                                           .ToDictionary(p => p.Name,
                                                         p => new Variable(p.Expression));
            }

            if (groups.TryGetValue(!isVariable, out statements)) {
                statements.ForEach(s => BlockExpressions.Add(s.AcceptVisitor(Visitor, this)));
            }
        }

        protected void AddReturnLabelExpression() {
            var registry = RootScope.BranchingRegistry;
            var returnExpresion = registry.ResolveReturnStatementLabel();

            BlockExpressions.Add(
                    Expression.Label(returnExpresion,
                        Expression.Default(returnExpresion.Type)));
        }
    }
}