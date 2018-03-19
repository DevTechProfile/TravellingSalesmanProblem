namespace TspOptimizer
{
    internal class ChromosomeSegment
    {
        internal int SegmentIndexA { get; }

        internal int SegmentIndexB { get; }

        internal ChromosomeSegment(int segmentIndexA, int segmentIndexB)
        {
            SegmentIndexA = segmentIndexA;
            SegmentIndexB = segmentIndexB;
        }
    }
}