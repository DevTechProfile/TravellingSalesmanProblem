using Combinatorics;
using System;
using System.Threading;

namespace TspOptimizer
{
    public class GridLocalOptimizer : TspOptimizerBase
    {
        public GridLocalOptimizer(int[] startPermutation, IEuclideanPath euclideanPath, OptimizerConfig config)
            : base(startPermutation, euclideanPath, config)
        {
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            throw new NotImplementedException();
        }
    }
}
