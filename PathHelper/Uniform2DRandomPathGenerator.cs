using System;
using System.Windows;

namespace PathHelper
{
    public class Uniform2DRandomPathGenerator : IPathGenerator
    {
        Random _random;

        public Uniform2DRandomPathGenerator()
        {
            _random = new Random();
        }

        public Coordinate[] GetPath(Size size, int numberOfPoints)
        {
            var path = new Coordinate[numberOfPoints];

            for (int i = 0; i < numberOfPoints; i++)
            {
                double x = _random.NextDouble() * size.Width;
                double y = _random.NextDouble() * size.Height;

                path[i] = new Coordinate(x, y);
            }

            return path;
        }
    }
}
