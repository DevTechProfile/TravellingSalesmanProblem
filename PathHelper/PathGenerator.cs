using Combinatorics;
using MathNet.Numerics.LinearAlgebra;
using System;

namespace PathHelper
{
    public class PathGenerator
    {
        public static EuclideanPath GetCirclePath(double angleStep, double radius)
        {
            int numbeOfPoints = (int)Math.Round(360d / angleStep, 0);

            Vector<double>[] points = new Vector<double>[numbeOfPoints];

            int count = 0;
            for (double i = 0; i < 2 * Math.PI; i += 2d * Math.PI / numbeOfPoints)
            {
                var currentCoordinates = new double[]
                {
                    Math.Cos(i),
                    Math.Sin(i)
                };

                points[count] = radius * Vector<double>.Build.Dense(currentCoordinates);
                count++;

                if (count >= numbeOfPoints)
                    break;
            }

            return new EuclideanPath(points);
        }
    }
}
