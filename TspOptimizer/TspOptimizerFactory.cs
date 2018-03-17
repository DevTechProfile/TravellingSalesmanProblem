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
                                           int[] startSequence, EuclideanPath euclideanPath)
        {
            ITspOptimizer tspOptimizer = null;
            switch (tspOptimizerAlgorithm)
            {
                case TspOptimizerAlgorithm.RandomOptimizer:
                    tspOptimizer = new RandomOptimizer(startSequence, euclideanPath);
                    break;
                case TspOptimizerAlgorithm.LocalSearchOptimizer:
                    tspOptimizer = new LocalSearchOptimizer(startSequence, euclideanPath);
                    break;
                case TspOptimizerAlgorithm.MultiLocalSearchOptimizer:
                    tspOptimizer = new MultiLocalSearchOptimizer(startSequence, euclideanPath);
                    break;
                case TspOptimizerAlgorithm.BruteForceOptimizer:
                    break;
                case TspOptimizerAlgorithm.BranchAndBoundOptimizer:
                    break;
                case TspOptimizerAlgorithm.SimulatedAnnealingOptimizer:
                    break;
                case TspOptimizerAlgorithm.GeneticOptimizer:
                    break;
                case TspOptimizerAlgorithm.GridLocalBruteForceOptimizer:
                    break;
                default:
                    throw new ArgumentException("Unknown optimizer");
            }

            return tspOptimizer;
        }
    }
}
