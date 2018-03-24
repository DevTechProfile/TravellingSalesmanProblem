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
        protected IEuclideanPath _euclideanPath;
        protected OptimizerConfig _config;

        public TspOptimizerBase(int[] startPermutation, IEuclideanPath euclideanPath, OptimizerConfig config)
        {
            _startPermutation = startPermutation;
            _euclideanPath = euclideanPath;
            _config = config;
            _optimalSequence = new Subject<int[]>();
            OptimizerInfo = new Subject<string>();
        }

        public OptimizerConfig Config { get; set; }

        IObservable<int[]> ITspOptimizer.OptimalSequence => _optimalSequence.AsObservable();

        public Subject<string> OptimizerInfo { get; }

        public abstract void Start(CancellationToken token, Action<double> action);
    }
}
