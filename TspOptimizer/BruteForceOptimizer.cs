using Combinatorics;
using System;
using System.Linq;
using System.Threading;

namespace TspOptimizer
{
    public class BruteForceOptimizer : TspOptimizerBase
    {
        private long _permutationCount;

        public BruteForceOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
            : base(startPermutation, euclideanPath)
        {
            _permutationCount = Enumerable.Range(1, _startPermutation.Length).Aggregate(1, (acc, val) => acc * val);
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            double minPathLength = double.MaxValue;
            var representation = new GeneralRepresentation<int>(_startPermutation, _startPermutation.Length, _startPermutation.Length);
            var permutationEnumerator = new PermutationEnumerator<int>(representation);

            do
            {
                var curPermuation = permutationEnumerator.CurrentPermutation.Elements;
                double curMin = _euclideanPath.GetPathLength(curPermuation, true);

                if (curMin < minPathLength)
                {
                    minPathLength = curMin;
                    action?.Invoke(minPathLength);
                    _optimalSequence.OnNext(curPermuation);
                }
            } while (permutationEnumerator.SetNext() && !token.IsCancellationRequested);

            _optimalSequence.OnCompleted();
        }
    }
}
