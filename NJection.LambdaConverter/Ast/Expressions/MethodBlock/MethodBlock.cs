using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class MethodBlock : Block
    {
        public MethodBlock(NRefactory.BlockStatement blockStatement, IScope scope, INRefcatoryExpressionVisitor visitor, IEnumerable<ParameterExpression> parameters = null)
            : base(blockStatement, parameters: parameters, scope: scope, visitor: visitor) {
            var registry = RootScope.BranchingRegistry;

            if (registry.HasReturnLabel) {
                AddReturnLabelExpression();
            }
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.MethodBlock; }
        }

        public override Expression Reduce() {
            return base.Reduce();
        }

        public override Type ResolveType(NRefactory.BlockStatement blockStatement) {
            var methodDeclaration = blockStatement.Parent as NRefactory.MethodDeclaration;

            if (methodDeclaration != null) {
                return methodDeclaration.GetReturnType();
            }

            return null;
        }
    }
}