using System;
using System.Collections.Generic;

namespace CZToolKit.ReactiveX
{
    public class Sort<T> : Operator<IList<T>, IList<T>>
    {
        private Func<T, T, int> comparer;

        public Sort(IObservable<IList<T>> src, Func<T, T, int> comparer) : base(src)
        {
            this.comparer = comparer;
        }

        public override void OnNext(IList<T> value)
        {
            QuickSort(value, comparer);
            observer.OnNext(value);
        }
        
        /// <returns> List Changed </returns>
        public static bool QuickSort(IList<T> original, Func<T, T, int> comparer)
        {
            if (original.Count <= 1)
                return false;
            return Util_Collections.QuickSort(original, 0, original.Count - 1, comparer);
        }
    }

    public static partial class Extension
    {
        public static IObservable<IList<T>> Sort<T>(this IObservable<IList<T>> _src, Func<T, T, int> comparer)
        {
            return new Sort<T>(_src, comparer);
        }
    }
}