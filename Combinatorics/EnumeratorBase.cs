using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combinatorics
{
    public abstract class EnumeratorBase<T>
    {
        public RepresentationBase<T> StartCombination
        {
            get;
            private set;
        }

        public EnumeratorBase(RepresentationBase<T> startCombination)
        {
            StartCombination = startCombination;
        }

        public abstract bool SetNext();
    }
}
