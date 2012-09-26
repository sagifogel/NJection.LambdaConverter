using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJection.Core;
using NJection.LambdaConverter;

namespace NJection.Scope
{
    public interface IRootScope : IMethodScope, IInstructionsIndexer
    {
    }
}
