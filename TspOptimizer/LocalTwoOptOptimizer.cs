using Combinatorics;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TspOptimizer
{
    public class LocalTwoOptOptimizer : TspOptimizerBase
    {
        public int[] OptimalSequence { get; private set; }

        public LocalTwoOptOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
            : base(startPermutation, euclideanPath)
        {
            OptimalSequence = _startPermutation.ToArray();
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
                try
                {
                    Parallel.For(0, _startPermutation.Length - 1, parallelOptions, i =>
                    {
                        for (int k = i + 1; k < _startPermutation.Length; k++)
                        {
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
                catch (OperationCanceledException e)
                {
                    _optimalSequence.OnCompleted();
                }
            }
        }

        public void Start(int maxLoopCount)
        {
            Random rand = new Random();
            double minPathLength = double.MaxValue;
            int[] currentSequence = _startPermutation.ToArray();

            ParallelOptions parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 2
            };

            while (maxLoopCount-- > 0)
            {
                Parallel.For(0, _startPermutation.Length - 1, parallelOptions, i =>
                {
                    for (int k = i + 1; k < _startPermutation.Length; k++)
                    {
                        var nextSequence = TwoOptSwap(currentSequence, i, k);
                        double curMin = _euclideanPath.GetCurrentPathLength(nextSequence, true);

                        if (curMin < minPathLength)
                        {
                            currentSequence = nextSequence.ToArray();
                            minPathLength = curMin;
                            OptimalSequence = currentSequence.ToArray();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/2-opt
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="i"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private int[] TwoOptSwap(int[] sequence, int i, int k)
        {
            int[] nextSequence = new int[sequence.Length];

            // 1. take sequence[0] to sequence[posA-1] and add them in order to nextSequence
            // 2. take sequence[posA] to sequence[posB] and add them in reverse order to nextSequence
            // 3. take sequence[posB+1] to end and add them in order to nextSequence
            Array.Copy(sequence, nextSequence, sequence.Length);
            Array.Reverse(nextSequence, i, k - i + 1);

            return nextSequence;
        }
    }
}
