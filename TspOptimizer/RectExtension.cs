using System.Windows;

namespace TspOptimizer
{
    public static class RectExtension
    {
        public static bool IsInnerPoint(this Rect rect, Point point)
        {
            bool isInnerPoint = true;

            // top-left corner
            var rectLocation = rect.Location;

            if (point.X < rectLocation.X)
                return false;

            if (point.Y > rectLocation.Y)
                return false;

            if (point.X >= rectLocation.X + rect.Width)
                return false;

            if (point.Y <= rectLocation.Y - rect.Height)
                return false;

            return isInnerPoint;
        }
    }
}
