using System;
using System.Linq;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;

namespace NJection.LambdaConverter.Ast.Visitors
{
    internal class OutParametersMatchingVisitor : DepthFirstAstVisitor<object, AstNode>
    {   
        private HashSet<string> _outParameters = new HashSet<string>();

        public override AstNode VisitDirectionExpression(DirectionExpression directionExpression, object data) {
            var identifier = directionExpression.Expression as NRefactory.IdentifierExpression;

            _outParameters.Add(identifier.Identifier);

            return base.VisitDirectionExpression(directionExpression, data);
        }

        public ICollection<string> OutParameters {
            get { return _outParameters; }
        }

        public bool Contains(string name) {
            return _outParameters.Contains(name);
        }
    }
}
