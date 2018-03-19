using System;
using System.Windows;

namespace PathHelper
{
    public class BigValleyPathGenerator : IPathGenerator
    {
        private readonly Random _random;
        private readonly double _insideOutRatio;

        public BigValleyPathGenerator()
        {
            _random = new Random();
            _insideOutRatio = 1d / 8d;
        }

        public Coordinate[] GetPath(Size size, int numberOfPoints)
        {
            var path = new Coordinate[numberOfPoints];

            for (int i = 0; i < numberOfPoints; i++)
            {
                double x = _random.NextDouble() * size.Width;
                double y = _random.NextDouble() * size.Height;

                if (x > _insideOutRatio * size.Width && x < (1 - _insideOutRatio) * size.Width &&
                    y > _insideOutRatio * size.Height && y < (1 - _insideOutRatio) * size.Height)
                {
                    y = size.Height / 2;
                }

                path[i] = new Coordinate(x, y);
            }

            return path;
        }
    }
}
