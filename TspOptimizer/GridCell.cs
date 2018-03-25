using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TspOptimizer
{
    public class GridCell
    {
        public Rect Rectangle { get; }

        public int[] ElementIndices { get; set; }

        public int[] InnerMinSequence { get; set; }

        public int InPointIndex { get; set; }

        public int OutPointIndex { get; set; }

        public GridCell(Rect rect)
        {
            Rectangle = rect;
            ElementIndices = new int[0];
            InPointIndex = -1;
            OutPointIndex = -1;
        }

        public int[] GetVariableIndices()
        {
            if (InPointIndex == -1 || OutPointIndex == -1)
            {
                throw new System.Exception("Invalid connection indices!");
            }

            if (ElementIndices.Length < 3)
            {
                return new int[0];
            }

            return ElementIndices.Where(index => index != InPointIndex && index != OutPointIndex).ToArray();
        }

        public int[] GetCompleteOptimizedIndices()
        {
            var innerIndexList = new List<int>
            {
                InPointIndex
            };

            innerIndexList.AddRange(InnerMinSequence);
            innerIndexList.Add(OutPointIndex);

            return innerIndexList.ToArray();
        }

        public int[] GetCompleteIndices()
        {
            var innerIndexList = new List<int>
            {
                InPointIndex
            };

            innerIndexList.AddRange(GetVariableIndices());
            innerIndexList.Add(OutPointIndex);

            return innerIndexList.ToArray();
        }
    }
}
