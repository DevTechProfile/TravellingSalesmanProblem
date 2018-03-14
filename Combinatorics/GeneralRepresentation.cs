using System.Linq;

namespace Combinatorics
{
    /// <summary>
    /// General set representation for combinations, variations and permutations 
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
