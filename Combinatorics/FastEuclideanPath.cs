using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Combinatorics
{
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
            //float pathLength = 0f;

            //unsafe
            //{
            //    fixed (int* seqpointer = sequence)
            //    {
            //        int* element = seqpointer;
            //        var remaining = sequence.Length - 1;
            //        while (remaining-- > 0)
            //        {
            //            pathLength += *element;
            //            element++;
            //        }
            //    }
            //}

            float pathLength = 0f;
            for (int i = 0; i < sequence.Length - 1; i++)
            {
                pathLength += GetDistanceFloat(sequence[i], sequence[i + 1]);
            }

            // closed hamiltonian path
            if (closedPath)
            {
                pathLength += GetDistanceFloat(sequence[0], sequence[_numberOfPoints - 1]);
            }

            return pathLength;
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
