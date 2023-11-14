namespace CZToolKit
{
    public class Util_Time
    {
        public static long UTCtoGMT(long fileTimeutc)
        {
            return fileTimeutc / 10000 - 11644473600000;
        }
    }
}