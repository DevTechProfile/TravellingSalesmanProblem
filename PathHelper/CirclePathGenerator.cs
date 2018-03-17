using System;
using System.Windows;

namespace PathHelper
{
    public class CirclePathGenerator : IPathGenerator
    {
        public Coordinate[] GetPath(Size size, int numberOfPoints)
        {
            double radius = size.Width / 2;
            var path = new Coordinate[numberOfPoints];

            int count = 0;
            for (double i = 0; i < 2 * Math.PI; i += 2d * Math.PI / numberOfPoints)
            {
                path[count] = radius * new Coordinate(Math.Cos(i), Math.Sin(i));
                count++;

                if (count >= numberOfPoints)
                    break;
            }

            return path;
        }
    }
}
