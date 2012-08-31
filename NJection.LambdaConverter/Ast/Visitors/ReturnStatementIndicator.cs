using System.Linq.Expressions;

namespace NJection.LambdaConverter.Visitors
{
    public class ReturnStatementIndicator
    {       
        public ReturnStatementIndicator(LabelTarget labelTarget)
        {
            LabelTarget = labelTarget;
        }
            
        public LabelTarget LabelTarget { get; private set; }

        public bool ReturnStatementFound { get; private set; }
    }
}
