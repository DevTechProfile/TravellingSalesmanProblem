using System.Windows;

namespace PathHelper
{
    public interface IPathGenerator
    {
        Coordinate[] GetPath(Size size, int numberOfPoints);
    }
}
