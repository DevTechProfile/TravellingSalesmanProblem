using Combinatorics;
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

        public override void Start(CancellationToken token)
        {
            double minPathLength = double.MaxValue;
            int[] minSequence = null;
            const int selected = 2;

            GeneralRepresentation presentation = new GeneralRepresentation(_startPermutation.Length, selected);
            IndexEnumerator indexEnumerator = new IndexEnumerator(presentation);

            int[] currentSequence = null;
            currentSequence = _startPermutation.ToArray();

            while (!token.IsCancellationRequested)
            {
                // Forcing delay for visualization
                Thread.Sleep(100);

                // One local ring
                do
                {                    
                    Helper.SwapPositions(currentSequence, indexEnumerator.CurrentCombination.Elements);
                    var currentPathLength = _euclideanPath.GetCurrentPathLength(currentSequence, true);

                    if (currentPathLength < minPathLength)
                    {
                        minPathLength = currentPathLength;
                        minSequence = currentSequence.ToArray();
                        _optimalSequence.OnNext(minSequence);
                    }
                    else
                    {
                        Helper.SwapPositions(currentSequence, indexEnumerator.CurrentCombination.Elements);
                    }
                } while (indexEnumerator.SetNext());            

                // Continue on new ring starting from miniumn on ancestor ring
                currentSequence = minSequence.ToArray();
                presentation = new GeneralRepresentation(_startPermutation.Length, selected);
                indexEnumerator = new IndexEnumerator(presentation);
            }
        }
    }
}
