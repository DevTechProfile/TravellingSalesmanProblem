using Combinatorics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TspOptimizer
{
    public class MultiLocalCombinationOptimizer : TspOptimizerBase
    {
        ITspOptimizer[] _optimizerSet;
        double _globalMinimum;

        public MultiLocalCombinationOptimizer(int[] startPermutation, EuclideanPath euclideanPath)
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

                _optimizerSet[i] = new LocalCombinationOptimizer(shuffledSequence, _euclideanPath);
            }
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            Array.ForEach(_optimizerSet, optimizer =>
            {
                optimizer.OptimalSequence.Subscribe(sequence => ObserveGlobalOptimum(sequence, action));
            });
            List<Task> tasks = new List<Task>(_optimizerSet.Length);

            foreach (var optimizer in _optimizerSet)
            {
                tasks.Add(Task.Factory.StartNew(() => optimizer.Start(token, null)));
            }

            tasks.ForEach(task => task.Wait());

            _optimalSequence.OnCompleted();
        }

        private void ObserveGlobalOptimum(int[] sequence, Action<double> action)
        {
            var currentPathLengh = _euclideanPath.GetPathLength(sequence, true);

            if (currentPathLengh < _globalMinimum)
            {
                // Should be locked? 
                _globalMinimum = currentPathLengh;
                action?.Invoke(_globalMinimum);
                _optimalSequence.OnNext(sequence);
            }
        }
    }
}
