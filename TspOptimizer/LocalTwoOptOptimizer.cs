using Combinatorics;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace TspOptimizer
{
    public class LocalTwoOptOptimizer : TspOptimizerBase
    {
        private Subject<int[]> _downSampledOptSequence;
        private IDisposable _disposeStream;

        public int[] OptimalSequence { get; private set; }

        public LocalTwoOptOptimizer(int[] startPermutation, IEuclideanPath euclideanPath, OptimizerConfig config)
            : base(startPermutation, euclideanPath, config)
        {
            OptimalSequence = _startPermutation.ToArray();
            _downSampledOptSequence = new Subject<int[]>();
            _disposeStream = Observable.Interval(TimeSpan.FromMilliseconds(100))
                                       .ObserveOn(Scheduler.Default)
                                       .CombineWithLatest(_downSampledOptSequence, (time, seq) => seq)
                                       .Subscribe(seq => _optimalSequence.OnNext(seq));
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            OptimizerInfo.OnNext("Starting 2-opt Optimizer");

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
                            double curMin = _euclideanPath.GetPathLength(nextSequence, ClosedPath);

                            if (curMin < minPathLength)
                            {
                                currentSequence = nextSequence.ToArray();
                                minPathLength = curMin;
                                action?.Invoke(minPathLength);
                                _downSampledOptSequence.OnNext(currentSequence);
                            }
                        }
                    });
                }
                catch (OperationCanceledException)
                {
                    _optimalSequence.OnCompleted();
                    _disposeStream?.Dispose();
                    OptimizerInfo.OnNext("Terminated 2-opt Optimizer with distance: " + minPathLength.ToString() + " LU");
                }
            }
        }

        public void Start(int maxLoopCount, bool useRandomBounds, double checkRate)
        {
            Random rand = new Random();
            double minPathLength = double.MaxValue;
            int[] currentSequence = _startPermutation.ToArray();

            while (maxLoopCount-- > 0)
            {
                for (int i = 0; i < _startPermutation.Length - 1; i++)
                {
                    for (int k = i + 1; k < _startPermutation.Length; k++)
                    {
                        if (useRandomBounds && rand.NextDouble() > checkRate)
                            continue;

                        var nextSequence = Helper.TwoOptSwap(currentSequence, i, k);
                        double curMin = _euclideanPath.GetPathLength(nextSequence, ClosedPath);

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
