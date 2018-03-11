using System.Reactive.Subjects;

namespace Combinatorics
{
    /// <summary>
    /// Interface for stream based recursive permutation algorithm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRecursivePermutation<T>
    {
        int PermCount { get; }

        Subject<T[]> PermutationStream { get; }

        void StartRecursion();
    }
}
