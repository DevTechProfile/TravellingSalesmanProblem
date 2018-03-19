using System.Collections.Generic;
using System.Linq;

namespace TspOptimizer
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> OuterSegments<T>(this IList<T> source, int segmentIndexStart, int segmentIndexEnd)
        {
            var firstOuterSegment = source.Take(segmentIndexStart);
            var secondOuterSegment = source.Skip(segmentIndexEnd + 1).Take(source.Count - segmentIndexEnd);

            return firstOuterSegment.Concat(secondOuterSegment);
        }
    }
}
