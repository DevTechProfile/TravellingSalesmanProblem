namespace TspOptimizer
{
    internal class ParentPair<T>
    {
        public T[] ParentA { get; }

        public T[] ParentB { get; }

        public ParentPair(T[] parentA, T[] parentB)
        {
            ParentA = parentA;
            ParentB = parentB;
        }
    }
}