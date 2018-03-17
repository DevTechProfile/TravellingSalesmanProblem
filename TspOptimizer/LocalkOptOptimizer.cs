using Combinatorics;
using System;
using System.Linq;
using System.Threading;

namespace TspOptimizer
{
    public class LocalkOptOptimizer : TspOptimizerBase
    {
        public LocalkOptOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
            : base(startPermutation, euclideanPath)
        {
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            Random rand = new Random();
            double minPathLength = double.MaxValue;
            int[] currentSequence = _startPermutation.ToArray();

            while (!token.IsCancellationRequested)
            {
                for (int i = 0; i < _startPermutation.Length - 1; i++)
                {
                    for (int k = i + 1; k < _startPermutation.Length; k++)
                    {
                        // Forcing delay for visualization
                        Thread.Sleep(1);

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
                }
            }

            _optimalSequence.OnCompleted();
        }

        private int[] TwoOptSwap(int[] sequence, int posA, int posB)
        {
            int[] nextSequence = new int[sequence.Length];

            // 1. take sequence[0] to sequence[posA-1] and add them in order to nextSequence
            for (int i = 0; i <= posA - 1; i++)
            {
                nextSequence[i] = sequence[i];
            }

            // 2. take sequence[posA] to sequence[posB] and add them in reverse order to nextSequence
            int dec = 0;
            for (int i = posA; i <= posB; i++)
            {
                nextSequence[i] = sequence[posB - dec];
                dec++;
            }

            // 3. take sequence[posB+1] to end and add them in order to nextSequence
            for (int i = posB + 1; i < sequence.Length; ++i)
            {
                nextSequence[i] = sequence[i];
            }

            return nextSequence;
        }
    }
}
