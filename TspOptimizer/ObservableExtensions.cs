using System;
using System.Linq;
using System.Reactive.Linq;

namespace TspOptimizer
{
    public static class ObservableExtensions
    {
        public static IObservable<TRes> CombineWithLatest<TSource1, TSource2, TRes>(this IObservable<TSource1> source1, 
            IObservable<TSource2> source2, Func<TSource1, TSource2, TRes> resultSelector)
        {
            var latestCache = default(TSource2);
            source2.Subscribe(s2 => latestCache = s2);

            return source1.Select(s1 => resultSelector(s1, latestCache));
        }
    }
}
