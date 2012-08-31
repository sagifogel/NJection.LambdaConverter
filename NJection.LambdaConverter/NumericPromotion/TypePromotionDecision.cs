using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJection.LambdaConverter
{
    public class TypePromotionDecision
    {
        public Type To { get; private set; }
        public Type From { get; private set; }
        public bool IsPromoted { get; private set; }

        public TypePromotionDecision(Type from)
            : this(from, null, false) {
        }

        public TypePromotionDecision(Type from, Type to, bool isConverted = true) {
            To = to;
            From = from;
            IsPromoted = isConverted;
        }
    }
}
