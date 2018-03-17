using Combinatorics;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TspOptimizer
{
    public class LocalTwoOptOptimizer : TspOptimizerBase
    {
        public LocalTwoOptOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
            : base(startPermutation, euclideanPath)
        {
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            Random rand = new Random();
            double minPathLength = double.MaxValue;
            int[] currentSequence = _startPermutation.ToArray();

            ParallelOptions parallelOptions = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Environment.ProcessorCount - 2
            };

            while (!token.IsCancellationRequested)
            {
                Parallel.For(0, _startPermutation.Length - 1, parallelOptions, i =>
                {
                    for (int k = i + 1; k < _startPermutation.Length; k++)
                    {
                        // Forcing delay for visualization
                        // Thread.Sleep(1);

                        var nextSequence = TwoOptSwap(currentSequence, i, k);
                        double curMin = _euclideanPath.GetCurrentPathLength(nextSequence, true);

                        if (curMin < minPathLength)
                        {
                            currentSequence = nextSequence.ToArray();
                            minPathLength = curMin;
                            action?.Invoke(minPathLength);
                            _optimalSequence.OnNext(currentSequence);
                        }
                    }
                });
            }

            _optimalSequence.OnCompleted();
        }

        private int[] TwoOptSwap(int[] sequence, int posA, int posB)
        {
            int[] nextSequence = new int[sequence.Length];

            // 1. take sequence[0] to sequence[posA-1] and add them in order to nextSequence
            // 2. take sequence[posA] to sequence[posB] and add them in reverse order to nextSequence
            // 3. take sequence[posB+1] to end and add them in order to nextSequence
            Array.Copy(sequence, nextSequence, sequence.Length);
            Array.Reverse(nextSequence, posA, posB - posA + 1);

            return nextSequence;
        }
    }
}
