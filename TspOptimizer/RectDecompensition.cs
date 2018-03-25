using System;

namespace TspOptimizer
{
    public class RectDecompensition
    {
        public static Tuple<int, int> GetDecompensition(int maxCellCount, double width, double height)
        {
            double rowsRatio = height / width;
            int rows = (int)Math.Sqrt(maxCellCount / rowsRatio);

            if (rows == 0)
                rows = 1;

            int columns = maxCellCount / rows;

            return new Tuple<int, int>(rows, columns);
        }
    }
}
