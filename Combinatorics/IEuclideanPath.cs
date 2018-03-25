using System.Windows;

namespace Combinatorics
{
    public interface IEuclideanPath
    {
        Point[] Points { get; }
        double GetAveragedeDistance();
        double GetPathLength(int[] sequence, bool closedPath);
        double GetSubPathLength(int[] sequence, int maxIndex);
        double GetDistance(int i, int j);
    }
}
