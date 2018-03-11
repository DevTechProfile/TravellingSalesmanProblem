using System;
using System.Linq;
using System.Reactive.Linq;

namespace ObservableExtension
{
    public class Sequence
    {
        public static IObservable<int> LimitedTimeSequence(int length, TimeSpan timespan)
        {
            return Observable.Interval(timespan).Zip(Enumerable.Range(0, length), (n, p) => p + 1);
        }
    }
}
