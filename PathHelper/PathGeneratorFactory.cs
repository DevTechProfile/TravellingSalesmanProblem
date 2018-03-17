﻿using System;
using System.Windows;

namespace PathHelper
{
    public class PathGeneratorFactory
    {
        public static Coordinate[] Create(TspPathType tspPathType, Size size, int numberOfPoints)
        {
            Coordinate[] path = null;
            IPathGenerator generator;
            switch (tspPathType)
            {
                case TspPathType.Uniform2DRandom:
                    generator = new Uniform2DRandomPathGenerator();
                    path = generator.GetPath(size, numberOfPoints);
                    break;
                case TspPathType.Normal2DRandom:
                    break;
                case TspPathType.Uniform3DRandom:
                    break;
                case TspPathType.Normal3DRandom:
                    break;
                case TspPathType.Ciclre:
                    generator = new CirclePathGenerator();
                    path = generator.GetPath(size, numberOfPoints);
                    break;
                default:
                    throw new ArgumentException("Unknown path type");
            }

            return path;
        }
    }
}
