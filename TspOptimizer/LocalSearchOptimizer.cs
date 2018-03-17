using Combinatorics;
using System;
using System.Linq;
using System.Threading;

namespace TspOptimizer
{
    public class LocalSearchOptimizer : TspOptimizerBase
    {
        public LocalSearchOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
            : base(startPermutation, euclideanPath)
        {
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            double minPathLength = double.MaxValue;
            int[] minSequence = null;
            const int selected = 2;

            GeneralRepresentation representation = new GeneralRepresentation(_startPermutation.Length, selected);
            IndexEnumerator indexEnumerator = new IndexEnumerator(representation);

            int[] currentSequence = null;
            currentSequence = _startPermutation.ToArray();

            while (!token.IsCancellationRequested)
            {
                // One local ring
                do
                {
                    //Force delay
                    Thread.Sleep(1);

                    Helper.SwapPositions(currentSequence, indexEnumerator.CurrentCombination.Elements);
                    var currentPathLength = _euclideanPath.GetCurrentPathLength(currentSequence, true);

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
