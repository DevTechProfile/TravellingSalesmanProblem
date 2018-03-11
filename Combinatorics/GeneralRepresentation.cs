using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Combinatorics
{
    public class GeneralRepresentation<T> : RepresentationBase<T>
    {
        public GeneralRepresentation(T[] elements, 
                                     int elementCount, 
                                     int selectedElementCount) 
               : base(elementCount, selectedElementCount)
        {
            Elements = elements.ToArray();
        }

        public override RepresentationBase<T> GetCopy()
        {
            return new GeneralRepresentation<T>(Elements.ToArray(), 
                                                ElementCount, 
                                                SelectedElementCount);
        }
    }
}
