using System.Linq;

namespace Combinatorics
{
    /// <summary>
    /// Permutations without repetition
    /// </summary>
    public class PermutationEnumerator<T> : EnumeratorBase<T>
    {
        private int _n;
        private int _i;
        private int _j;
        private int[] _p;

        public GeneralRepresentation<T> CurrentPermutation
        {
            get;
            private set;
        }

        public PermutationEnumerator(GeneralRepresentation<T> startPerm) : base(startPerm)
        {
            CurrentPermutation = startPerm;
            _n = startPerm.ElementCount;
            _i = 1;
            _p = Enumerable.Range(0, _n + 1).ToArray();
        }

        public override bool SetNext()
        {
            if (_i < _n)
            {
                _p[_i]--;

                // if i is odd then j = p[i] otherwise j = 0
                _j = _i % 2 * _p[_i];
                var tmp = CurrentPermutation.Elements[_j];
                CurrentPermutation.Elements[_j] = CurrentPermutation.Elements[_i];
                CurrentPermutation.Elements[_i] = tmp;

                _i = 1;
                while (_p[_i] == 0)
                {
                    _p[_i] = _i;
                    _i++;
                }

                return true;
            }
            else
                return false;
        }
    }
}
