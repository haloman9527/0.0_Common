using System;

namespace Moyo
{
    public static class Util_Time
    {
        public static long ToFileTimeUtcMs(this DateTime dateTime)
        {
            return dateTime.ToFileTimeUtc() / 10000;
        }
        
        public static long UTCToGMT(long utc)
        {
            return utc - 116444736000000000;
        }
    }
}