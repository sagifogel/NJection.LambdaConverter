using System;
using System.Linq.Expressions;

namespace NJection.LambdaConverter
{
    public interface IBranchingRegistry
    {
        bool HasReturnLabel { get; }
        LabelTarget RegisterLabel(Type type, string name);
        LabelTarget ResolveLabel(string name);
        LabelTarget RegisterReturnStatementLabel(Type type);
        LabelTarget ResolveReturnStatementLabel();
    }
}