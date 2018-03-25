using Combinatorics;
using System;
using System.Linq;
using System.Threading;

namespace TspOptimizer
{
    public class LocalCombinationOptimizer : TspOptimizerBase
    {
        public LocalCombinationOptimizer(int[] startPermutation, IEuclideanPath euclideanPath, OptimizerConfig config)
            : base(startPermutation, euclideanPath, config)
        {
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            double minPathLength = double.MaxValue;
            int[] minSequence = null;
            const int selected = 2;

            GeneralRepresentation representation = new GeneralRepresentation(_startPermutation.Length, selected);
            IndexEnumerator indexEnumerator = new IndexEnumerator(representation);

            int[] currentSequence = _startPermutation.ToArray();

            while (!token.IsCancellationRequested)
            {
                // One local ring
                do
                {
                    // Forcing delay for visualization
                    if (_config.UseDelay)
                    {
                        Thread.Sleep(_config.DelayTime);
                    }

                    Helper.SwapPositions(currentSequence, indexEnumerator.CurrentCombination.Elements);
                    var currentPathLength = _euclideanPath.GetPathLength(currentSequence, ClosedPath);

                    if (currentPathLength < minPathLength)
                    {
                        minPathLength = currentPathLength;
                        action?.Invoke(minPathLength);
                        minSequence = currentSequence.ToArray();
                        _optimalSequence.OnNext(minSequence);

                        // Stop on first optimum, something like greedy strategy
                        break;
                    }
                    else
                    {
                        Helper.SwapPositions(currentSequence, indexEnumerator.CurrentCombination.Elements);
                    }
                } while (indexEnumerator.SetNext());

                // Continue on new ring starting from minimum path length of ancestor ring
                representation = new GeneralRepresentation(_startPermutation.Length, selected);
                indexEnumerator = new IndexEnumerator(representation);
            }

            _optimalSequence.OnCompleted();
        }
    }
}
