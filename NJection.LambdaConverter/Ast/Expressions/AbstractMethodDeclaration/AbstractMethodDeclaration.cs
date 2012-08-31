using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NJection.LambdaConverter.TypeResolvers;
using NJection.LambdaConverter.Visitors;
using NRefactory = ICSharpCode.NRefactory.CSharp;
using NJection.LambdaConverter.DynamicProxies;

namespace NJection.LambdaConverter.Expressions
{
    public abstract class AbstractMethodDeclaration<TDeclaration> : Method, IMethodScope
        where TDeclaration : NRefactory.AstNode
    {
        protected Type TypeOfThis = null;
        protected object ThisReference = null;
        protected TDeclaration Declaration = null;
        protected MethodDefinition MethodDefinition = null;
        private IInstructionsIndexer _instructionIndexer = null;
        protected IEnumerable<Parameter> BaseConstructorParameters = null;

        protected internal AbstractMethodDeclaration(TDeclaration declaration, 
                                                     IScope scope, 
                                                     INRefcatoryExpressionVisitor visitor, 
                                                     object context = null, 
                                                     Type type = null, 
                                                     IEnumerable<Parameter> baseConstructorParameters = null)
            : base(scope: scope, visitor: visitor) {
            
            Declaration = declaration;
            ThisReference = context;
            TypeOfThis = type;
            RootScope = this;
            BaseConstructorParameters = baseConstructorParameters;
            InitializeMethod();
        }

        public IContext Context { get; protected set; }

        public IBranchingRegistry BranchingRegistry { get; protected set; }

        internal ICecilArgumentsResolver ArgumentsResolver { get; set; }

        public abstract Expression Update(Expression body, IEnumerable<ParameterExpression> parameters);

        protected virtual void InitializeMethod() {
            var blockStatement = GetBody();

            MethodDefinition = Declaration.Annotation<MethodDefinition>();
            InternalType = GetReturnType();
            ExtractContext();
            Parameters = ExtractParameters();
            GetILInstructions(MethodDefinition);
            BranchingRegistry = new BranchingRegistry();
            Body = CreateBody(blockStatement, Parameters, this, Visitor);
        }

        protected virtual Expression CreateBody(NRefactory.BlockStatement blockStatement, IEnumerable<ParameterExpression> parameters, IScope scope, INRefcatoryExpressionVisitor visitor) {
            return AstExpression.MethodBlock(blockStatement, parameters: parameters, scope: this, visitor: visitor);
        }

        protected abstract Type GetReturnType();

        protected abstract NRefactory.BlockStatement GetBody();

        protected abstract IEnumerable<ParameterExpression> ExtractParameters();

        protected abstract void ExtractContext();

        protected void GetILInstructions(Mono.Cecil.MethodDefinition methodDefinition) {
            _instructionIndexer = new InstructionsIndexer(methodDefinition.Body.Instructions.ToList());
        }

        public bool TryGetInstruction(NRefactory.AstNode node, OpCode opCode, out Instruction instruction) {
            return _instructionIndexer.TryGetInstruction(node, opCode, out instruction);
        }

        public Instruction GetInstruction(NRefactory.AstNode node) {
            return _instructionIndexer.GetInstruction(node);
        }

        public Instruction GetNextInstruction(Instruction instruction) {
            return _instructionIndexer.GetNextInstruction(instruction);
        }

        public Instruction GetLastInstructionInRange(NRefactory.AstNode node) {
            return _instructionIndexer.GetLastInstructionInRange(node);
        }

        public LabelTarget RegisterLabel(Type type, string name) {
            return BranchingRegistry.RegisterLabel(type, name);
        }

        public LabelTarget ResolveLabel(string name) {
            return BranchingRegistry.ResolveLabel(name);
        }

        public LabelTarget RegisterReturnStatementLabel(Type type) {
            return BranchingRegistry.RegisterReturnStatementLabel(type);
        }

        public bool HasReturnLabel {
            get { return BranchingRegistry.HasReturnLabel; }
        }

        public LabelTarget ResolveReturnStatementLabel() {
            return BranchingRegistry.ResolveReturnStatementLabel();
        }
    }
}