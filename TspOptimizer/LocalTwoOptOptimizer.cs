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
                            var nextSequence = Helper.TwoOptSwap(currentSequence, i, k);
                            double curMin = _euclideanPath.GetPathLength(nextSequence, true);

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
                catch (OperationCanceledException)
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
                        var nextSequence = Helper.TwoOptSwap(currentSequence, i, k);
                        double curMin = _euclideanPath.GetPathLength(nextSequence, true);

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
    }
}
