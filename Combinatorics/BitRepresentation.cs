using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combinatorics
{
    public class BitRepresentation : RepresentationBase<bool>
    {
        public BitRepresentation(int elementCount, int selectedElementCount) : base(elementCount, selectedElementCount)
        {
            Elements = new bool[elementCount];
            for (int i = 0; i < selectedElementCount; i++)
                Elements[i] = true;
        }

        public override RepresentationBase<bool> GetCopy()
        {
            return new BitRepresentation(ElementCount, SelectedElementCount) { Elements = Elements.ToArray() };
        }
    }
}
