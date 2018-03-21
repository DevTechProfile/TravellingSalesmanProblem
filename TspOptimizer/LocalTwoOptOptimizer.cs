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

        public LocalTwoOptOptimizer(int[] startPermutation, EuclideanPath euclideanPath, OptimizerConfig config)
            : base(startPermutation, euclideanPath, config)
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
                MaxDegreeOfParallelism = _config.NumberOfCores
            };

            while (!token.IsCancellationRequested)
            {
                try
                {
                    // When using this parallel loop the calculation will not be deterministic 
                    // because of missing locking and therefore random order. 
                    // Nevertheless the results will be good 
                    Parallel.For(0, _startPermutation.Length - 1, parallelOptions, i =>
                    {
                        for (int k = i + 1; k < _startPermutation.Length; k++)
                        {
                            // Forcing delay for visualization
                            if (_config.UseDelay)
                            {
                                Thread.Sleep(_config.DelayTime);
                            }

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

        public void Start(int maxLoopCount, bool useRandomBounds)
        {
            Random rand = new Random();
            double minPathLength = double.MaxValue;
            int[] currentSequence = _startPermutation.ToArray();

            int endBoundOuterLoop = _startPermutation.Length - 1;
            int endBoundInnerLoop = _startPermutation.Length;

            if (useRandomBounds)
            {
                endBoundOuterLoop = rand.Next(_startPermutation.Length - 1);
                endBoundInnerLoop = rand.Next(endBoundOuterLoop + 1, _startPermutation.Length);
            }

            while (maxLoopCount-- > 0)
            {
                for (int i = 0; i < endBoundOuterLoop; i++)
                {
                    for (int k = i + 1; k < endBoundInnerLoop; k++)
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
                }
            }
        }
    }
}
