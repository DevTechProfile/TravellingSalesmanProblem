using System;
using System.Collections.Generic;
using System.Linq;

namespace TspOptimizer
{
    internal class PartiallyMappedCrossover
    {
        private int _number;
        private Random _random;

        /// <summary>
        /// Partially mapped crossover algorithm
        /// </summary>
        /// <param name="number">Number of elements in chromosome array</param>
        public PartiallyMappedCrossover(int number)
        {
            _number = number;
            _random = new Random();
        }

        /// <summary>
        /// Get cross combined off-spring pair
        /// </summary>
        /// <returns></returns>
        public OffSpringPair<int> GetCrossCombinedOffSpringPair(ChromosomeSegment segment, ParentPair<int> parentPair)
        {
            var parentA = GetChildCrossParent(parentPair.ParentB, parentPair.ParentA, segment);
            var parentB = GetChildCrossParent(parentPair.ParentA, parentPair.ParentB, segment);

            return new OffSpringPair<int>(parentA, parentB);
        }

        protected virtual int[] GetChildCrossParent(int[] parentA, int[] parentB, ChromosomeSegment segment)
        {
            int[] child = new int[_number];

            int segmentLengh = segment.SegmentIndexB - segment.SegmentIndexA + 1;
            Array.Copy(parentA, segment.SegmentIndexA, child, segment.SegmentIndexA, segmentLengh);
            var segmentChildATabuList = new HashSet<int>(child.Skip(segment.SegmentIndexA).Take(segmentLengh));
            var outerSegmentParentATabuList = new HashSet<int>(parentB.OuterSegments(segment.SegmentIndexA, segment.SegmentIndexB));

            Action<int> fillChild = (i) =>
            {
                if (!segmentChildATabuList.Contains(parentB[i]))
                {
                    child[i] = parentB[i];
                }
                else
                {
                    for (int indexFirst = 0; indexFirst < segment.SegmentIndexA; indexFirst++)
                    {
                        if (!outerSegmentParentATabuList.Contains(parentA[indexFirst]))
                        {
                            child[i] = parentA[indexFirst];
                            outerSegmentParentATabuList.Add(parentA[indexFirst]);

                            return;
                        }
                    }

                    for (int indexSecond = segment.SegmentIndexB + 1; indexSecond < _number; indexSecond++)
                    {
                        if (!outerSegmentParentATabuList.Contains(parentA[indexSecond]))
                        {
                            child[i] = parentA[indexSecond];
                            outerSegmentParentATabuList.Add(parentA[indexSecond]);

                            return;
                        }
                    }
                }
            };

            for (int index = 0; index < segment.SegmentIndexA; index++)
            {
                fillChild(index);
            }

            for (int index = segment.SegmentIndexB + 1; index < _number; index++)
            {
                fillChild(index);
            }

            return child;
        }
    }
}
