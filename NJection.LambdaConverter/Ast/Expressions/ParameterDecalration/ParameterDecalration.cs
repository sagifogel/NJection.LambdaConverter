using System.Linq.Expressions;
using NJection.LambdaConverter.Extensions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class ParameterDecalration : Declaration<NRefactory.ParameterDeclaration>
    {
        protected internal ParameterDecalration(NRefactory.ParameterDeclaration parameter, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(parameter, scope, visitor) {
            
            InternalType = parameter.GetActualType();
            Name = parameter.Name;
        }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Parmeter; }
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitParameter(this);
        }

        protected override Expression CreateDeclaration() {
            return AstExpression.Parameter(DeclarationValue, ParentScope, Visitor);
        }
    }
}