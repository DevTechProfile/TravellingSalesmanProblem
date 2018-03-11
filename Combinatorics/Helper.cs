using System;

namespace Combinatorics
{
    public class Helper
    {
        static Random _random = new Random();

        public static long GetBinCoeff(long N, long K)
        {
            long r = 1;
            long d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++)
            {
                r *= N--;
                r /= d;
            }
            return r;
        }

        public static int GetBinCoeff(int N, int K)
        {
            int r = 1;
            int d;
            if (K > N) return 0;
            for (d = 1; d <= K; d++)
            {
                r *= N--;
                r /= d;
            }
            return r;
        }

        static public void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                // Use Next on random instance with an argument.
                // ... The argument is an exclusive bound.
                //     So we will not go past the end of the array.
                int r = i + _random.Next(n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        public static void SwapPositions<T>(T[] array, int[] swaps)
        {
            for (int i = 0; i < swaps.Length - 1; i++)
            {
                T posA = array[swaps[i]];
                array[swaps[i]] = array[swaps[i + 1]];
                array[swaps[i + 1]] = posA;
            }
        }
    }
}
