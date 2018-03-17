using Combinatorics;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace TspOptimizer
{
    public class RandomOptimizer : TspOptimizerBase
    {
        public RandomOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
            : base(startPermutation, euclideanPath)
        {
        }

        public override void Start(CancellationToken token)
        {
            Random rand = new Random();
            double min = double.MaxValue;
            var curPermutation = _startPermutation.ToArray();

            while (!token.IsCancellationRequested)
            {
                // Forcing delay for visualization
                Thread.Sleep(1);

                int cp1, cp2;

                do
                {
                    cp1 = rand.Next(curPermutation.Length);
                    cp2 = rand.Next(curPermutation.Length);
                } while (cp1 == cp2);

                Helper.SwapPositions(curPermutation, new int[] { cp1, cp2 });

                double curMin = _euclideanPath.GetCurrentPathLength(curPermutation, true);

                if (curMin < min)
                {
                    min = curMin;
                    _optimalSequence.OnNext(curPermutation.ToArray());
                }
                else
                {
                    Helper.SwapPositions(curPermutation, new int[] { cp1, cp2 });
                }
            }

            _optimalSequence.OnCompleted();
        }
    }
}
