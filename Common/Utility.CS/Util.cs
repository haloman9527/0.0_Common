#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System.Net;

namespace CZToolKit.Common
{
    public static partial class Util
    {
        /// <summary> 获取IP地址 </summary>
        public static string GetLocalIP()
        {
            foreach (IPAddress address in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (address.AddressFamily.ToString() == "InterNetwork")
                    return address.ToString();
            }
            return string.Empty;
        }
    }
}