using Combinatorics;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace TspOptimizer
{
    public class RandomOptimizer : TspOptimizerBase
    {
        public RandomOptimizer(int[] startPermutation, IEuclideanPath euclideanPath, OptimizerConfig config)
            : base(startPermutation, euclideanPath, config)
        {
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            Random rand = new Random();
            double minPathLength = double.MaxValue;
            var currentSequence = _startPermutation.ToArray();

            while (!token.IsCancellationRequested)
            {
                // Forcing delay for visualization
                if (_config.UseDelay)
                {
                    Thread.Sleep(_config.DelayTime);
                }

                int cp1, cp2;

                do
                {
                    cp1 = rand.Next(currentSequence.Length);
                    cp2 = rand.Next(currentSequence.Length);
                } while (cp1 == cp2);

                var swaps = new int[] { cp1, cp2 };
                Helper.SwapPositions(currentSequence, swaps);

                double curMin = _euclideanPath.GetPathLength(currentSequence, true);

                if (curMin < minPathLength)
                {
                    minPathLength = curMin;
                    action?.Invoke(minPathLength);
                    _optimalSequence.OnNext(currentSequence.ToArray());
                }
                else
                {
                    Helper.SwapPositions(currentSequence, swaps);
                }
            }

            _optimalSequence.OnCompleted();
        }
    }
}
