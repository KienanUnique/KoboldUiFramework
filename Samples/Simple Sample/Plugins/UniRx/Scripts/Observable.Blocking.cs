using System;

namespace SampleUnirx
{
    public static partial class Observable
    {
        public static T Wait<T>(this IObservable<T> source)
        {
            return new SampleUnirx.Operators.Wait<T>(source, InfiniteTimeSpan).Run();
        }

        public static T Wait<T>(this IObservable<T> source, TimeSpan timeout)
        {
            return new SampleUnirx.Operators.Wait<T>(source, timeout).Run();
        }
    }
}
