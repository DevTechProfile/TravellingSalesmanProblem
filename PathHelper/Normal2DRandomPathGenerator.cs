using System;
using System.Windows;

namespace PathHelper
{
    public class Normal2DRandomPathGenerator : IPathGenerator
    {
        Random _random;

        public Normal2DRandomPathGenerator()
        {
            _random = new Random();
        }

        public Coordinate[] GetPath(Size size, int numberOfPoints)
        {
            var path = new Coordinate[numberOfPoints];

            for (int i = 0; i < numberOfPoints; i++)
            {
                double x = GaussianNextDouble() * Math.Sqrt(size.Width) / 4;
                double y = GaussianNextDouble() * Math.Sqrt(size.Height) / 4;

                path[i] = new Coordinate(x, y);
            }

            return path;
        }

        /// <summary>
        /// Standard normal distributed random number
        /// </summary>
        /// <returns></returns>
        private double GaussianNextDouble()
        {
            double argA = 1.0 - _random.NextDouble();
            double argB = 1.0 - _random.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(argA)) * Math.Sin(2.0 * Math.PI * argB);
        }
    }
}
