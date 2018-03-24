using MathNet.Numerics.LinearAlgebra;
using System;
using System.Linq;
using System.Windows;

namespace Combinatorics
{
    public class EuclideanPath : EuclideanPathBase
    {
        Matrix<double> _distanceMatrix;

        public EuclideanPath(Point[] points) : base(points)
        {
            _numberOfPoints = points.Length;
            _distanceMatrix = GetDistanceMatrix(points);
        }

        public override double GetAveragedeDistance()
        {
            return _distanceMatrix.Enumerate().Where(el => el != 0d).Average();
        }

        public override double GetPathLength(int[] sequence, bool closedPath)
        {
            double currentPathLength = 0d;

            for (int i = 0; i < sequence.Length - 1; i++)
            {
                currentPathLength += GetDistance(sequence[i], sequence[i + 1]);
            }

            // closed hamiltonian path
            if (closedPath)
            {
                currentPathLength += GetDistance(sequence.First(), sequence.Last());
            }

            return currentPathLength;
        }

        public override double GetSubPathLength(int[] sequence, int maxIndex)
        {
            double currentPathLength = 0d;

            for (int i = 0; i < maxIndex; i++)
            {
                currentPathLength += GetDistance(sequence[i], sequence[i + 1]);
            }

            return currentPathLength;
        }

        public override double GetDistance(int indexA, int indexB)
        {
            int orderedIndexA = Math.Min(indexA, indexB);
            int orderedIndexB = Math.Max(indexA, indexB);

            return _distanceMatrix[orderedIndexA, orderedIndexB];
        }

        private Matrix<double> GetDistanceMatrix(Point[] points)
        {
            Matrix<double> distanceMatrix = Matrix<double>.Build.Sparse(_numberOfPoints, _numberOfPoints);

            for (int row = 0; row < _numberOfPoints; row++)
            {
                for (int column = row + 1; column < _numberOfPoints; column++)
                {
                    // euclidean distance
                    distanceMatrix[row, column] = Point.Subtract(points[row], points[column]).Length;
                }
            }

            return distanceMatrix;
        }
    }
}
