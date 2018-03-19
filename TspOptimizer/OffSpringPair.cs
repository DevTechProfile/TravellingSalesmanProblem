namespace TspOptimizer
{
    public class OffSpringPair<T>
    {
        public T[] ChildA { get; }

        public T[] ChildB { get; }

        public OffSpringPair(T[] childA, T[] childB)
        {
            ChildA = childA;
            ChildB = childB;
        }
    }
}
