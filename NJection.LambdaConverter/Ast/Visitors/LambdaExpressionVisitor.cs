using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using NJection.LambdaConverter.Extensions;
using LinqExpressions = System.Linq.Expressions;

namespace NJection.LambdaConverter.Visitors
{
    internal class LambdaExpressionVisitor<TNode> : DepthFirstAstVisitor<object, TNode>
        where TNode : AstNode
    {
        private BlockStatement _block = null;
        private AssignmentVisitor _assignmentVisitor = null;
        private InvocationVisitor _invocationVisitor = null;
        private MethodDeclaration _methodDeclaration = null;
        private MemberReferenceVisitor _memberVisitor = null;
        private Dictionary<string, Tuple<AstNode, Type>> _variables = null;

        internal LambdaExpressionVisitor() {
            _block = new BlockStatement();
            _assignmentVisitor = new AssignmentVisitor();
            _invocationVisitor = new InvocationVisitor();
            _memberVisitor = new MemberReferenceVisitor();
            _variables = new Dictionary<string, Tuple<AstNode, Type>>();
        }

        public override TNode VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data) {
            BlockStatement body = null;

            _methodDeclaration = methodDeclaration;
            _methodDeclaration.Parameters.ForEach(p => AddVariable(p, p.Name, p.Type));
            base.VisitMethodDeclaration(methodDeclaration, data);
            body = methodDeclaration.Body;
            _block.AddRange(body.Statements.Select(s => s.Clone()));
            methodDeclaration.Body = _block;

            return methodDeclaration as TNode;
        }

        public override TNode VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, object data) {
            var variableInitializer = variableDeclarationStatement.Variables.Single();
            string name = variableInitializer.Name;
            var astType = variableDeclarationStatement.Type;

            variableDeclarationStatement.Remove();
            AddVariable(variableDeclarationStatement, name, astType);
            _block.Add(variableDeclarationStatement);

            return base.VisitVariableDeclarationStatement(variableDeclarationStatement, data);
        }

        public override TNode VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data) {
            var invocation = assignmentExpression.Right as InvocationExpression;

            if (IsLambdaInvocation(invocation)) {
                MapCapturedParametersToVariableDeclarations(invocation);
            }

            return base.VisitAssignmentExpression(assignmentExpression, data);
        }

        private void AddVariable(AstNode node, string name, AstType astType) {
            if (!_variables.ContainsKey(name)) {
                Type type = astType.GetActualType();
                _variables.Add(name, Tuple.Create(node, type));
            }
        }

        private bool IsLambdaInvocation(InvocationExpression invocation) {
            if (invocation != null) {
                var methodReference = invocation.Annotation<MethodReference>();

                if (methodReference.Name.Equals("Lambda") && methodReference.DeclaringType.FullName.Equals("System.Linq.Expressions.Expression")) {
                    return true;
                }
            }

            return false;
        }

        private void MapCapturedParametersToVariableDeclarations(InvocationExpression invocation) {
            var lambdaParameters = invocation.Arguments.Last() as ArrayCreateExpression;
            var initializer = lambdaParameters.Initializer as ArrayInitializerExpression;
            var identifier = invocation.PrevSibling as IdentifierExpression;
            Func<MemberReferenceExpression, bool> predicate = (mref) => MemberPredicate(mref, identifier);
            var memberReference = _memberVisitor.VisitAndCompare(_methodDeclaration, predicate);
            var types = GetMatchingTypesFromInvocation(memberReference.Parent as InvocationExpression);

            initializer.Elements.ForEach((e, i) => {
                CreateParameterAssignmentIfNeeded(types[i], e as IdentifierExpression);
            });
        }

        private bool MemberPredicate(MemberReferenceExpression memberReferenceExpression, IdentifierExpression identifier) {
            var target = memberReferenceExpression.Target as IdentifierExpression;

            return target != null &&
                   target.Identifier.Equals(identifier.Identifier) &&
                   memberReferenceExpression.MemberName.Equals("Compile");
        }

        private bool AssignmentPredicate(AssignmentExpression assignmentExpression, IdentifierExpression identifier) {
            var expressionIdentifier = assignmentExpression.Left as IdentifierExpression;

            return expressionIdentifier != null && expressionIdentifier.Identifier.Equals(identifier.Identifier);
        }

        private bool AssignmentPredicate(InvocationExpression invocationExpression, IdentifierExpression identifier) {
            var expressionIdentifier = invocationExpression.Target as IdentifierExpression;

            return expressionIdentifier != null &&
                   expressionIdentifier.Identifier.Equals(identifier.Identifier);
        }

        private IList<Type> GetMatchingTypesFromInvocation(InvocationExpression invocationExpression) {
            Type invocationType = null;
            List<Type> returnTypes = null;
            IdentifierExpression identifier = null;
            var parent = invocationExpression.Parent;
            var invocation = parent as InvocationExpression;
            IEnumerable<string> types = Enumerable.Empty<string>();

            if (invocation == null) {
                var assignment = parent as AssignmentExpression;

                if (assignment != null) {
                    identifier = assignment.Left as IdentifierExpression;
                    Func<InvocationExpression, bool> predicate = (assign) => AssignmentPredicate(assign, identifier);

                    invocation = _invocationVisitor.VisitAndCompare(_methodDeclaration, predicate);
                }
            }

            if (invocation.Target is InvocationExpression) {
                returnTypes = invocation.Target.Annotation<MethodReference>()
                                        .ReturnType
                                        .GenericParameters
                                        .Select(p => p.GetActualType())
                                        .ToList();

                returnTypes.RemoveAt(returnTypes.Count - 1);
            }
            else {
                identifier = invocation.Target as IdentifierExpression;
                invocationType = _variables[identifier.Identifier].Item2;

                if (invocationType.IsDelegate()) {
                    returnTypes = invocationType.GetGenericArguments().ToList();
                    returnTypes.RemoveAt(returnTypes.Count - 1);
                }
                else {
                    returnTypes = new List<Type> { invocationType };
                }
            }

            return returnTypes;
        }

        private void CreateParameterAssignmentIfNeeded(Type type, IdentifierExpression identifier) {
            Func<AssignmentExpression, bool> predicate = (assign) => AssignmentPredicate(assign, identifier);
            var assignment = _assignmentVisitor.VisitAndCompare(_methodDeclaration, predicate);

            if (assignment == null) {
                var mainModule = _methodDeclaration.Annotation<MethodReference>().Module.Assembly.MainModule;
                var getTypeFromHandleInvocation = GetTypeFromHandleInvocation(type, mainModule);
                var parameterCreationInvocation = GetParameterCreationInvocation(mainModule, getTypeFromHandleInvocation);
                var assignmentExpression = new AssignmentExpression(identifier.Clone(), AssignmentOperatorType.Assign, parameterCreationInvocation);
                var expressionStatement = new ExpressionStatement(assignmentExpression);

                _block.Statements.Add(expressionStatement);
            }
        }

        private InvocationExpression GetTypeFromHandleInvocation(Type type, ModuleDefinition mainModule) {
            var typeofSystemType = typeof(Type);
            var astTypeReference = BuildAstTypeFromSystemType(type);
            var typofExpression = new TypeReferenceExpression(new SimpleType(typeofSystemType.Name));
            var getTypeFromHandle = typofExpression.Member("GetTypeFromHandle");
            var typeOfExpression = new TypeOfExpression(astTypeReference).Member("TypeHandle");
            var invocation = getTypeFromHandle.Invoke(typeOfExpression);
            var getTypeFromHandleMethodInfo = typeofSystemType.GetMethod("GetTypeFromHandle");
            var getTypeFromHandleReference = mainModule.Import(getTypeFromHandleMethodInfo);

            invocation.AddAnnotation(getTypeFromHandleReference);

            return invocation;
        }

        private InvocationExpression GetParameterCreationInvocation(ModuleDefinition mainModule, InvocationExpression argument) {
            var typeofExpression = typeof(LinqExpressions.Expression);
            var parameterInocationMethodInfo = typeofExpression.GetMethod("Parameter", new Type[] { typeof(Type) });
            var paramterInvocationReference = mainModule.Import(parameterInocationMethodInfo);
            var typofExpression = new TypeReferenceExpression(new SimpleType(typeofExpression.Name));
            var parameterInvocation = typofExpression.Member("Parameter")
                                                     .Invoke(argument);

            parameterInvocation.AddAnnotation(paramterInvocationReference);

            return parameterInvocation;
        }

        private AstType BuildAstTypeFromSystemType(Type type) {
            if (type.IsNested) {
                var target = BuildAstTypeFromSystemType(type.DeclaringType);
                return new MemberType(target, type.Name);
            }
            else if (type.IsArray) {
                var elementType = type.GetElementType();
                var target = BuildAstTypeFromSystemType(elementType);

                return target.MakeArrayType(type.GetArrayRank());
            }

            return AstType.Create(type);
        }

        private class MemberReferenceVisitor : DepthFirstAstVisitor<Func<MemberReferenceExpression, bool>, AstNode>
        {
            private MemberReferenceExpression _memberReferenceExpression = null;

            public MemberReferenceExpression VisitAndCompare(AstNode expression, Func<MemberReferenceExpression, bool> predicate) {
                _memberReferenceExpression = null;
                expression.AcceptVisitor(this, predicate);

                return _memberReferenceExpression;
            }

            public override AstNode VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, Func<MemberReferenceExpression, bool> predicate) {
                if (predicate(memberReferenceExpression)) {
                    _memberReferenceExpression = memberReferenceExpression;
                }

                return base.VisitMemberReferenceExpression(memberReferenceExpression, predicate);
            }
        }

        private class AssignmentVisitor : DepthFirstAstVisitor<Func<AssignmentExpression, bool>, AstNode>
        {
            private AssignmentExpression _assignmentExpression = null;

            public AssignmentExpression VisitAndCompare(AstNode expression, Func<AssignmentExpression, bool> predicate) {
                _assignmentExpression = null;
                expression.AcceptVisitor(this, predicate);

                return _assignmentExpression;
            }

            public override AstNode VisitAssignmentExpression(AssignmentExpression assignmentExpression, Func<AssignmentExpression, bool> predicate) {
                if (predicate(assignmentExpression)) {
                    _assignmentExpression = assignmentExpression;
                }

                return base.VisitAssignmentExpression(assignmentExpression, predicate);
            }
        }

        private class InvocationVisitor : DepthFirstAstVisitor<Func<InvocationExpression, bool>, AstNode>
        {
            private InvocationExpression _assignmentExpression = null;

            public InvocationExpression VisitAndCompare(AstNode expression, Func<InvocationExpression, bool> predicate) {
                _assignmentExpression = null;
                expression.AcceptVisitor(this, predicate);

                return _assignmentExpression;
            }

            public override AstNode VisitInvocationExpression(InvocationExpression invocationExpression, Func<InvocationExpression, bool> predicate) {
                if (predicate(invocationExpression)) {
                    _assignmentExpression = invocationExpression;
                }

                return base.VisitInvocationExpression(invocationExpression, predicate);
            }
        }
    }
}