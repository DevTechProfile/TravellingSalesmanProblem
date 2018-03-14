using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Combinatorics.Test
{
    [TestClass]
    public class PermutationEnumeratorTest
    {
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(6)]
        [DataRow(8)]
        [DataRow(10)]
        [DataTestMethod]
        public void EnumeratePermuations_CorrectCount(int elementCount)
        {
            var elements = Enumerable.Range(0, elementCount).ToArray();
            var representation = new GeneralRepresentation<int>(elements, elementCount, elementCount);
            var permutationEnumerator = new PermutationEnumerator<int>(representation);

            // Start permutation must be counted
            int actualCount = 1;
            while (permutationEnumerator.SetNext())
            {
                actualCount++;
            }

            Assert.AreEqual(Enumerable.Range(1, elementCount).Aggregate(1, (acc, val) => acc * val), actualCount);
        }
    }
}
