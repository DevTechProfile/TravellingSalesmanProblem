using Combinatorics;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace TspOptimizer
{
    public class RandomOptimizer : ITspOptimizer
    {
        private int[] _startPermutation;
        private Subject<int[]> _optimalSequence;
        private EuclideanPath _euclideanPath;

        public RandomOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
        {
            _startPermutation = startPermutation;
            _euclideanPath = euclideanPath;
            _optimalSequence = new Subject<int[]>();
        }

        IObservable<int[]> ITspOptimizer.OptimalSequence => _optimalSequence.AsObservable();

        public void Start(CancellationToken token)
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
        }
    }
}
