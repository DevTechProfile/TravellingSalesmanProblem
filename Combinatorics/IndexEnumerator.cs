using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combinatorics
{
    public class IndexEnumerator : EnumeratorBase<int>
    {
        private int _j;
        private int _k;
        private int _n;

        public RepresentationBase<int> CurrentCombination
        {
            get;
            private set;
        }

        public IndexEnumerator(RepresentationBase<int> startCombination) : base(startCombination)
        {
            _k = startCombination.SelectedElementCount - 1;
            _j = _k;
            _n = StartCombination.ElementCount - 1;

            CurrentCombination = startCombination.GetCopy();
        }

        //Dieser Konstruktur ist dazu gedacht, um eine beliebige Stelle
        //im lokalen Ring zu springen, ist aber noch fehlerhaft
        //public IndexEnumerator(RepresentationBase<int> startCombination, int j) : base(startCombination)
        //{
        //    _j = j;

        //    _k = startCombination.SelectedElementCount - 1;
        //    _n = StartCombination.ElementCount - 1;

        //    CurrentCombination = startCombination.GetCopy();
        //}

        public override bool SetNext()
        {
            CurrentCombination.Elements[_j]++;

            for (int i = _j + 1; i <= _k; i++)
                CurrentCombination.Elements[i] = CurrentCombination.Elements[i - 1] + 1;

            if (CurrentCombination.Elements[_k] < _n)
                _j = _k;
            else
            {
                _j--;
                if (_j < 0)
                    return false;
            }

            return true;
        }
    }
}
