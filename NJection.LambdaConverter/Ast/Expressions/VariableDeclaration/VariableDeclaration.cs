using System.Linq;
using System.Linq.Expressions;
using NJection.LambdaConverter.Visitors;
using NJection.Scope;
using NRefactory = ICSharpCode.NRefactory.CSharp;

namespace NJection.LambdaConverter.Expressions
{
    public class VariableDeclaration : Declaration<NRefactory.VariableDeclarationStatement>
    {
        protected internal VariableDeclaration(NRefactory.VariableDeclarationStatement VariableDeclaration, IScope scope, INRefcatoryExpressionVisitor visitor)
            : base(VariableDeclaration, scope, visitor) {
            var variable = VariableDeclaration.Variables.First();
            var variableType = VariableDeclaration.Type;

            InternalType = variableType.AcceptVisitor(Visitor, ParentScope).Type;

            if (InternalType == null) {
                if (TypeIsVar(variableType)) {
                    InternalType = TypeSystem.Object;
                    IsAnnonymousType = true;
                }
            }

            Name = variable.Name;
        }

        public bool IsAnnonymousType { get; private set; }

        public override AstExpressionType AstNodeType {
            get { return AstExpressionType.Variabale; }
        }

        public override Expression Accept(NJectionExpressionVisitor visitor) {
            return visitor.VisitVaraibale(this);
        }

        protected override Expression CreateDeclaration() {
            return AstExpression.Variable(DeclarationValue, ParentScope, Visitor);
        }
    }
}