using Combinatorics;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace TspOptimizer
{
    public abstract class TspOptimizerBase : ITspOptimizer
    {
        protected int[] _startPermutation;
        protected Subject<int[]> _optimalSequence;
        protected EuclideanPath _euclideanPath;

        public TspOptimizerBase(int[] startPermutation, EuclideanPath euclideanPath)
        {
            _startPermutation = startPermutation;
            _euclideanPath = euclideanPath;
            _optimalSequence = new Subject<int[]>();
        }

        IObservable<int[]> ITspOptimizer.OptimalSequence => _optimalSequence.AsObservable();

        public abstract void Start(CancellationToken token);
    }
}
