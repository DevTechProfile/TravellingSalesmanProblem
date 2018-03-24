using Microsoft.VisualStudio.TestTools.UnitTesting;
using PathHelper;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Combinatorics.Test
{
    [TestClass]
    public class EuclideanPathTest
    {        
        [DataRow(1000)]
        [DataTestMethod]
        public void EuclideanPathPerformanceTest(int numberOfPoints)
        {
            var path = PathGeneratorFactory.Create(TspPathType.Uniform2DRandom, new Size(10,10), numberOfPoints);
            var transPath = path.Select(coordinate => coordinate.To2DPoint()).ToArray();

            var euclideanPath = new EuclideanPath(transPath);
            var fastEuclideanPath = new FastEuclideanPath(transPath);

            var sequence = Enumerable.Range(0, numberOfPoints).ToArray();

            Stopwatch standardStopwatch = new Stopwatch();
            standardStopwatch.Start();
            double distanceStandard = euclideanPath.GetPathLength(sequence, true);
            standardStopwatch.Stop();

            Stopwatch fastStopwatch = new Stopwatch();
            fastStopwatch.Start();
            double distanceFast = euclideanPath.GetPathLength(sequence, true);
            fastStopwatch.Stop();

            Assert.IsTrue(Math.Abs(distanceStandard - distanceFast) < 1E-04);

            Console.WriteLine("Ticks standard: " + standardStopwatch.ElapsedTicks.ToString());
            Console.WriteLine("Ticks fast: " + fastStopwatch.ElapsedTicks.ToString());
        }
    }
}
