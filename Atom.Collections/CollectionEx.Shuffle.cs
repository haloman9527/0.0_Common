using System;
using System.Collections.Generic;

namespace Atom
{
    public static partial class CollectionEx
    {
        /// <summary>
        /// 洗牌
        /// </summary>
        /// <param name="original"></param>
        /// <param name="random"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> original, Random random = null)
        {
            random = random == null ? Consts.DefaultRandom : random;
            Shuffle(original, 0, original.Count - 1, random);
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        /// <param name="original"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="random"></param>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> original, int startIndex, int endIndex, Random random = null)
        {
            random = random == null ? Consts.DefaultRandom : random;
            while (endIndex-- - startIndex > 0)
            {
                var index = random.Next(startIndex, endIndex + 1);
                if (index != endIndex)
                    (original[endIndex], original[index]) = (original[index], original[endIndex]);
            }
        }

        public static unsafe void Shuffle<T>(T* original, int startIndex, int endIndex, Random random = null) where T : unmanaged
        {
            random = random == null ? Consts.DefaultRandom : random;
            while (endIndex-- - startIndex > 0)
            {
                var index = random.Next(startIndex, endIndex + 1);
                if (index != endIndex)
                    (original[endIndex], original[index]) = (original[index], original[endIndex]);
            }
        }
    }
}