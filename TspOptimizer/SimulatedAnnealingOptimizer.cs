using Combinatorics;
using System;
using System.Linq;
using System.Threading;

namespace TspOptimizer
{
    public class SimulatedAnnealingOptimizer : TspOptimizerBase
    {
        private double _coolingRate;

        public SimulatedAnnealingOptimizer(int[] startPermutation, EuclideanPath euclideanPath) : base(startPermutation, euclideanPath)
        {
            _coolingRate = 0.95;
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            Random rand = new Random();
            double minPathLength = double.MaxValue;
            double curPathLength = double.MaxValue;
            int[] currentSequence = _startPermutation.ToArray();

            double temperature = double.MaxValue;

            while (!token.IsCancellationRequested)
            {
                int cp1, cp2;

                do
                {
                    cp1 = rand.Next(0, currentSequence.Length - 1);
                    cp2 = rand.Next(1, currentSequence.Length);
                } while (cp1 == cp2 || cp1 > cp2);

                var nextSequence = Helper.TwoOptSwap(currentSequence, cp1, cp2);

                double nextPathLength = _euclideanPath.GetCurrentPathLength(nextSequence, true);
                double difference = nextPathLength - curPathLength;
                curPathLength = nextPathLength;

                if (curPathLength < minPathLength)
                {
                    currentSequence = nextSequence.ToArray();
                    minPathLength = curPathLength;
                    action?.Invoke(minPathLength);
                    _optimalSequence.OnNext(currentSequence.ToArray());
                }
                else if (difference > 0 && GetProbability(difference, temperature) > rand.NextDouble())
                {
                    currentSequence = nextSequence.ToArray();
                    minPathLength = curPathLength;
                }

                temperature = temperature * _coolingRate;
            }

            _optimalSequence.OnCompleted();
        }

        private double GetProbability(double difference, double temperature)
        {
            return Math.Exp(-difference / temperature);
        }
    }
}
