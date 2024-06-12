using System;
using System.Collections.Generic;

namespace CZToolKit
{
    public static partial class Util_Collections
    {
        /// <summary>
        /// 洗牌
        /// </summary>
        /// <param name="original"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> original)
        {
            Shuffle(original, 0, original.Count - 1);
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> original, int startIndex, int endIndex)
        {
            while (endIndex - startIndex > 0)
            {
                var index = Util.DefaultRandom.Next(startIndex, endIndex + 1);
                if (index != endIndex)
                {
                    var last = original[endIndex];
                    original[endIndex] = original[index];
                    original[index] = last;
                }

                endIndex--;
            }
        }

        public static unsafe void Shuffle<T>(T* original, int startIndex, int endIndex) where T : unmanaged
        {
            while (endIndex - startIndex > 0)
            {
                var index = Util.DefaultRandom.Next(startIndex, endIndex + 1);
                if (index != endIndex)
                {
                    var last = original[endIndex];
                    original[endIndex] = original[index];
                    original[index] = last;
                }

                endIndex--;
            }
        }
    }
}