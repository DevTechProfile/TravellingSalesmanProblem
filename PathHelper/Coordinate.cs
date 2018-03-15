using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PathHelper
{
    /// <summary>
    /// Struct for cart. coordinates
    /// </summary>
    public struct Coordinate
    {
        public double X { get; }

        public double Y { get; }

        public double Z { get; }

        public int Dimension { get; }

        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
            Z = double.NaN;
            Dimension = 2;
        }

        public Coordinate(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            Dimension = 3;
        }

        public Point To2DPoint()
        {
            if (Dimension != 2)
            {
                throw new InvalidCastException("Cannot convert 3D to 2D");
            }

            return new Point(X, Y);
        }

        public Point3D To3DPoint()
        {
            if (Dimension != 3)
            {
                throw new InvalidCastException("Cannot convert 2D to 3D");
            }

            return new Point3D(X, Y, Z);
        }

        public static Coordinate operator *(double scalar, Coordinate coordinate)
        {
            if (coordinate.Dimension == 2)
            {
                return new Coordinate(scalar * coordinate.X, scalar * coordinate.Y);
            }
            else
            {
                return new Coordinate(scalar * coordinate.X, scalar * coordinate.Y, scalar * coordinate.Z);
            }
        }
    }
}
