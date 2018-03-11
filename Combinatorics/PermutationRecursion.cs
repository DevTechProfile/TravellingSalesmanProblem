using System;
using System.Linq;
using System.Reactive.Subjects;

namespace Combinatorics
{
    public class PermutationRecursion<T> : IRecursivePermutation<T>
    {
        private int _n;
        private Func<T[], bool> _isValidSelector;
        private T[] _startPermutation;

        public int PermCount
        {
            get;
            private set;
        }

        public Subject<T[]> PermutationStream
        {
            get;
        }

        public PermutationRecursion(T[] startPerm, Func<T[], bool> isValidSelector = null)
        {
            if (isValidSelector == null)
            {
                isValidSelector = (x) => { return true; };
            }

            _startPermutation = startPerm.ToArray();
            _n = startPerm.Length;
            _isValidSelector = isValidSelector;

            PermCount = 0;
            PermutationStream = new Subject<T[]>();
        }

        private void Permute(T[] currentPermutation, int i, int endIndex)
        {
            if (i == endIndex)
            {
                PermCount++;

                // Push current permutation into stream
                PermutationStream.OnNext(currentPermutation);
            }
            else
            {
                for (int j = i; j <= endIndex; j++)
                {
                    T tmp = currentPermutation[i];
                    currentPermutation[i] = currentPermutation[j];
                    currentPermutation[j] = tmp;

                    // Cut this branch (Branch&Bound)
                    if (!_isValidSelector(currentPermutation))
                    {
                        return;
                    }

                    Permute(currentPermutation, i + 1, endIndex);

                    tmp = currentPermutation[i];
                    currentPermutation[i] = currentPermutation[j];
                    currentPermutation[j] = tmp;
                }
            }
        }

        public void StartRecursion()
        {
            Permute(_startPermutation, 0, _n - 1);
        }
    }
}
