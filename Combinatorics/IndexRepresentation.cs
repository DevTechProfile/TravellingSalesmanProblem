using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combinatorics
{
    public class GeneralRepresentation : RepresentationBase<int>
    {
        public GeneralRepresentation(int elementCount, int selectedElementCount) : base(elementCount, selectedElementCount)
        {
            Elements = new int[selectedElementCount];
            for (int i = 0; i < selectedElementCount; i++)
                Elements[i] = i;
        }
        public override RepresentationBase<int> GetCopy()
        {
            return new GeneralRepresentation(ElementCount, SelectedElementCount) { Elements = Elements.ToArray() };
        }
    }
}
