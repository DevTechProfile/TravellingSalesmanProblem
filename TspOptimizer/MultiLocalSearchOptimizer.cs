using Combinatorics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TspOptimizer
{
    public class MultiLocalSearchOptimizer : TspOptimizerBase
    {
        ITspOptimizer[] _optimizerSet;
        double _globalMinimum;

        public MultiLocalSearchOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
            : base(startPermutation, euclideanPath)
        {
            _globalMinimum = double.MaxValue;
            SetMultiSearcher(Environment.ProcessorCount - 2);
        }

        public void SetMultiSearcher(int count)
        {
            _optimizerSet = new ITspOptimizer[count];

            for (int i = 0; i < count; i++)
            {
                var shuffledSequence = _startPermutation.ToArray();
                Helper.Shuffle(shuffledSequence);

                _optimizerSet[i] = new LocalSearchOptimizer(shuffledSequence, _euclideanPath);
                _optimizerSet[i].OptimalSequence.Subscribe(ObserveGlobalOptimum);
            }
        }

        public override void Start(CancellationToken token)
        {
            if (_optimizerSet == null)
            {
                throw new InvalidOperationException("No optimizer set defined.");
            }

            List<Task> tasks = new List<Task>(_optimizerSet.Length);

            foreach (var optimizer in _optimizerSet)
            {
                tasks.Add(Task.Factory.StartNew(() => optimizer.Start(token)));
            }

            tasks.ForEach(task => task.Wait());
        }

        private void ObserveGlobalOptimum(int[] sequence)
        {
            var currentPathLengh = _euclideanPath.GetCurrentPathLength(sequence, true);

            if (currentPathLengh < _globalMinimum)
            {
                // Should be locked? 
                _globalMinimum = currentPathLengh;
                _optimalSequence.OnNext(sequence);
            }
        }
    }
}
