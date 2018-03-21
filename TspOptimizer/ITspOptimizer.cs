using System;
using System.Reactive.Subjects;
using System.Threading;

namespace TspOptimizer
{
    public interface ITspOptimizer
    {
        IObservable<int[]> OptimalSequence { get; }
        Subject<string> OptimizerInfo { get; }
        void Start(CancellationToken token, Action<double> action);
    }
}
