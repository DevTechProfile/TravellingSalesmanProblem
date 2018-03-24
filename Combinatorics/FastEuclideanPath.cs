using System;
using System.Linq;
using System.Numerics;
using System.Windows;

namespace Combinatorics
{
    /// <summary>
    /// Path calculation with SIMD support
    /// </summary>
    public class FastEuclideanPath : EuclideanPathBase
    {
        private float[] _distanceArray;

        public FastEuclideanPath(Point[] points) : base(points)
        {
            _distanceArray = GetDistanceArray(points);
        }

        public override double GetAveragedeDistance()
        {
            throw new NotImplementedException();
        }

        public override double GetDistance(int i, int j)
        {
            return _distanceArray[j + i * _numberOfPoints];
        }

        public override double GetPathLength(int[] sequence, bool closedPath)
        {
            float[] currentDistanceArray = new float[_numberOfPoints - 1];

            for (int i = 0; i < sequence.Length - 1; i++)
            {
                currentDistanceArray[i] = GetDistanceFloat(sequence[i], sequence[i + 1]);
            }

            var sums = Vector<float>.Zero;
            var simdLength = Vector<float>.Count;
            for (int i = 0; i < currentDistanceArray.Length; i += simdLength)
                sums += new Vector<float>(currentDistanceArray, i);
            float sum = 0f;
            for (int i = 0; i < simdLength; i++)
                sum += sums[i];

            // closed hamiltonian path
            if (closedPath)
            {
                sum += GetDistanceFloat(sequence.First(), sequence.Last());
            }

            return sum;
        }

        public override double GetSubPathLength(int[] sequence, int maxIndex)
        {
            float currentPathLength = 0f;

            for (int i = 0; i < maxIndex; i++)
            {
                currentPathLength += GetDistanceFloat(sequence[i], sequence[i + 1]);
            }

            return currentPathLength;
        }

        private float GetDistanceFloat(int i, int j)
        {
            return _distanceArray[j + i * _numberOfPoints];
        }

        private float[] GetDistanceArray(Point[] points)
        {
            var distanceArray = new float[_numberOfPoints * _numberOfPoints];

            for (int row = 0; row < _numberOfPoints; row++)
            {
                for (int column = 0; column < _numberOfPoints; column++)
                {
                    // euclidean distance
                    distanceArray[column + row * _numberOfPoints] = (float)Point.Subtract(points[row], points[column]).Length;
                }
            }

            return distanceArray;
        }
    }
}
