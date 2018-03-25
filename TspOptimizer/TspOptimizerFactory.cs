using Combinatorics;
using System;

namespace TspOptimizer
{
    public class TspOptimizerFactory
    {
        /// <summary>
        ///  Factory to create specified optimizer
        /// </summary>
        /// <param name="tspOptimizerAlgorithm"></param>
        /// <param name="startSequence"></param>
        /// <param name="euclideanPath"></param>
        /// <returns></returns>
        public static ITspOptimizer Create(TspOptimizerAlgorithm tspOptimizerAlgorithm,
                                           int[] startSequence, IEuclideanPath euclideanPath,
                                           OptimizerConfig config)
        {
            ITspOptimizer tspOptimizer = null;
            switch (tspOptimizerAlgorithm)
            {
                case TspOptimizerAlgorithm.RandomOptimizer:
                    tspOptimizer = new RandomOptimizer(startSequence, euclideanPath, config);
                    break;
                case TspOptimizerAlgorithm.LocalCombinationOptimizer:
                    tspOptimizer = new LocalCombinationOptimizer(startSequence, euclideanPath, config);
                    break;
                case TspOptimizerAlgorithm.MultiLocalCombinationOptimizer:
                    tspOptimizer = new MultiLocalCombinationOptimizer(startSequence, euclideanPath, config);
                    break;
                case TspOptimizerAlgorithm.LocalTwoOptOptimizer:
                    tspOptimizer = new LocalTwoOptOptimizer(startSequence, euclideanPath, config);
                    break;
                case TspOptimizerAlgorithm.BruteForceOptimizer:
                    tspOptimizer = new BruteForceOptimizer(startSequence, euclideanPath, config);
                    break;
                case TspOptimizerAlgorithm.BranchAndBoundOptimizer:
                    tspOptimizer = new BranchAndBoundOptimizer(startSequence, euclideanPath, config);
                    break;
                case TspOptimizerAlgorithm.SimulatedAnnealingOptimizer:
                    tspOptimizer = new SimulatedAnnealingOptimizer(startSequence, euclideanPath, config);
                    break;
                case TspOptimizerAlgorithm.GeneticOptimizer:
                    tspOptimizer = new GeneticOptimizer(startSequence, euclideanPath, config);
                    break;
                default:
                    throw new ArgumentException("Unknown optimizer");
            }

            return tspOptimizer;
        }
    }
}
