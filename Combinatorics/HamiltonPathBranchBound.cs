using System;
using System.Linq;

namespace Combinatorics
{
    public class HamiltonPathBranchBound
    {
        private int _n;
        private int[] _startPermutation;
        private double _min;
        private bool _closedPath;
        private EuclideanPath _euclideanPath;

        public int[] OptPermutation
        {
            get; private set;
        }

        public HamiltonPathBranchBound(int[] startPerm, EuclideanPath euclideanPath, double initialMin, bool closedPath)
        {
            _n = startPerm.Length;
            _euclideanPath = euclideanPath;
            _min = initialMin;
            _startPermutation = startPerm.ToArray();
            _closedPath = closedPath;

            OptPermutation = _startPermutation;
        }

        public void DoRecursion(bool preOpt)
        {
            if (preOpt)
            {
                Random rand = new Random();
                var curPermutation = _startPermutation.ToArray();

                for (int i = 0; i < 100000; i++)
                {
                    int cp1, cp2;

                    do
                    {
                        cp1 = rand.Next(curPermutation.Length);
                        cp2 = rand.Next(curPermutation.Length);
                    } while (cp1 == cp2);

                    Helper.SwapPositions(curPermutation, new int[] { cp1, cp2 });

                    double curMin = _euclideanPath.GetCurrentPathLength(curPermutation, _closedPath);

                    if (curMin < _min)
                    {
                        _min = curMin;
                        _startPermutation = curPermutation.ToArray();
                    }
                    else
                    {
                        Helper.SwapPositions(curPermutation, new int[] { cp1, cp2 });
                    }
                }
            }

            Permute(_startPermutation, 0, _n - 1);
        }

        public void DoRecursion(int startIndex, int endIndex)
        {
            Permute(_startPermutation, startIndex, endIndex);
        }

        private void Permute(int[] currentPermutation, int i, int endIndex)
        {
            if (i == endIndex)
            {
                var currentMinValue = _euclideanPath.GetCurrentPathLength(currentPermutation, _closedPath);

                if (_min.CompareTo(currentMinValue) > 0)
                {
                    OptPermutation = currentPermutation.ToArray();
                    _min = currentMinValue;
                }
            }
            else
            {
                for (int j = i; j <= endIndex; j++)
                {
                    int tmp = currentPermutation[i];
                    currentPermutation[i] = currentPermutation[j];
                    currentPermutation[j] = tmp;

                    if (i > 2)
                    {
                        //Cut this branch(Branch & Bound)
                        if (_min.CompareTo(_euclideanPath.GetCurrentSubPathLength(currentPermutation, i + 1, _min, _closedPath)) < 0)
                        {
                            return;
                        }
                    }

                    Permute(currentPermutation, i + 1, endIndex);

                    tmp = currentPermutation[i];
                    currentPermutation[i] = currentPermutation[j];
                    currentPermutation[j] = tmp;
                }
            }
        }
    }
}
