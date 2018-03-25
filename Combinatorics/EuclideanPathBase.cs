using System.Windows;

namespace Combinatorics
{
    public abstract class EuclideanPathBase : IEuclideanPath
    {
        private Point[] _points;
        protected int _numberOfPoints;

        public EuclideanPathBase(Point[] points)
        {
            _points = points;
            _numberOfPoints = points.Length;
        }

        public Point[] Points => _points;

        public abstract double GetAveragedeDistance();

        public abstract double GetDistance(int i, int j);

        public abstract double GetPathLength(int[] sequence, bool closedPath);

        public abstract double GetSubPathLength(int[] sequence, int maxIndex);
    }
}
