using System;
using System.Threading;

namespace TspOptimizer
{
    public interface ITspOptimizer
    {
        IObservable<int[]> OptimalSequence { get; }
        void Start(CancellationToken token, Action<double> action);
    }
}
