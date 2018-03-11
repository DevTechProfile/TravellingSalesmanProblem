using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Combinatorics
{
    public class EuclideanPath
    {
        Matrix<double> _distanceMatrix;
        Vector<double>[] _points;

        public List<Point> Path
        {
            get { return _points.Select(vec => new Point(vec[0], vec[1])).ToList(); }
        }

        public EuclideanPath(IEnumerable<Point> points)
        {
            _points = points.Select(point => Vector<double>.Build.Dense(new double[] { point.X, point.Y })).ToArray();
            _distanceMatrix = GetDistanceMatrix(_points);
        }

        public EuclideanPath(Vector<double>[] points)
        {
            _points = points;
            _distanceMatrix = GetDistanceMatrix(points);
        }

        public double GetAveragedeDistance()
        {
            return _distanceMatrix.Enumerate().Where(el => el != 0d).Average();
        }

        public double GetCurrentPathLength(int[] currentSequence, bool closedPath)
        {
            double currentPathLength = 0d;

            for (int i = 0; i < currentSequence.Length - 1; i++)
            {
                currentPathLength += GetDistance(currentSequence[i], currentSequence[i + 1]);
            }

            // closed hamiltonian path
            if (closedPath)
            {
                currentPathLength += GetDistance(currentSequence.First(), currentSequence.Last());
            }

            return currentPathLength;
        }

        public double GetCurrentSubPathLength(int[] currentSequence, int maxIndex, double minPath, bool closedPath)
        {
            double currentPathLength = 0d;

            for (int i = 0; i < maxIndex; i++)
            {
                currentPathLength += GetDistance(currentSequence[i], currentSequence[i + 1]);
            }

            return currentPathLength;
        }

        public double GetDistance(int indexA, int indexB)
        {
            int orderedIndexA = Math.Min(indexA, indexB);
            int orderedIndexB = Math.Max(indexA, indexB);

            return _distanceMatrix[orderedIndexA, orderedIndexB];
        }

        private Matrix<double> GetDistanceMatrix(Vector<double>[] points)
        {
            Matrix<double> distanceMatrix = Matrix<double>.Build.Sparse(points.Length, points.Length);

            for (int row = 0; row < points.Length; row++)
            {
                for (int column = row + 1; column < points.Length; column++)
                {
                    // euclidean distance
                    distanceMatrix[row, column] = Distance.Euclidean(points[row], points[column]);
                }
            }

            return distanceMatrix;
        }

        private Matrix<double> GetSquaredDistanceMatrix(Vector<double>[] points)
        {
            Matrix<double> distanceMatrix = Matrix<double>.Build.Sparse(points.Length, points.Length);

            for (int row = 0; row < points.Length; row++)
            {
                for (int column = row + 1; column < points.Length; column++)
                {
                    // euclidean distance
                    var differenceA = points[row][0] - points[column][0];
                    var differenceB = points[row][1] - points[column][1];
                    distanceMatrix[row, column] = differenceA * differenceA + differenceB * differenceB; //Distance.SSD(points[row], points[column]);
                }
            }

            return distanceMatrix;
        }
    }
}
