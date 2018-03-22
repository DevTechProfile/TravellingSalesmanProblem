using Combinatorics;
using System;
using System.Linq;
using System.Threading;

namespace TspOptimizer
{
    public class BranchAndBoundOptimizer : TspOptimizerBase
    {
        private double _minPathLength;
        private CancellationToken _token;
        private Action<double> _action;

        public BranchAndBoundOptimizer(int[] startPermutation, EuclideanPath euclideanPath, OptimizerConfig config)
            : base(startPermutation, euclideanPath, config)
        {
            _minPathLength = double.MaxValue;
        }

        public override void Start(CancellationToken token, Action<double> action)
        {
            _token = token;
            _action = action;

            // Do pre optimization -> early cuts in branches
            var preOptimizer = new LocalTwoOptOptimizer(_startPermutation, _euclideanPath, _config);
            preOptimizer.Start(1, false, 1);

            var preOptimalSequence = preOptimizer.OptimalSequence;
            _optimalSequence.OnNext(preOptimalSequence.ToArray());
            _minPathLength = _euclideanPath.GetPathLength(preOptimalSequence, true);
            action?.Invoke(_minPathLength);
            Permute(_startPermutation, 0, _startPermutation.Length - 1);

            _optimalSequence.OnCompleted();
        }

        private void Permute(int[] currentPermutation, int i, int endIndex)
        {
            if (_token.IsCancellationRequested)
            {
                return;
            }

            // Forcing delay for visualization
            if (_config.UseDelay)
            {
                Thread.Sleep(_config.DelayTime);
            }

            if (i == endIndex)
            {
                var currentMinValue = _euclideanPath.GetPathLength(currentPermutation, true);

                if (_minPathLength.CompareTo(currentMinValue) > 0)
                {
                    _optimalSequence.OnNext(currentPermutation.ToArray());
                    _minPathLength = currentMinValue;
                    _action?.Invoke(_minPathLength);
                }
            }
            else
            {
                for (int j = i; j <= endIndex; j++)
                {
                    int tmp = currentPermutation[i];
                    currentPermutation[i] = currentPermutation[j];
                    currentPermutation[j] = tmp;

                    if (i > 1)
                    {
                        //Cut this branch(Branch & Bound)
                        if (_minPathLength.CompareTo(_euclideanPath.GetSubPathLength(currentPermutation, i + 1)) <= 0)
                        {
                            return;
                        }
                    }

                    Permute(currentPermutation, i + 1, endIndex);

                    tmp = currentPermutation[i];
                    currentPermutation[i] = currentPermutation[j];
                    currentPermutation[j] = tmp;
                }
            }
        }
    }
}
