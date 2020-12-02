using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace text.doors.Common
{
    public static class LAN
    {
        public static bool IsLanLink = false;

        [DllImport("WININET", CharSet = CharSet.Auto)]
        private static extern bool InternetGetConnectedState(ref InternetConnectionState lpdwFlags, int dwReserved);
        
        /// <summary>
        /// 检测网络状态
        /// </summary>
        /// <returns></returns>
        public static void ReadLanLink()
        {
            InternetConnectionState flag = InternetConnectionState.INTERNET_CONNECTION_LAN;
            IsLanLink = InternetGetConnectedState(ref flag, 0);
        }
        
        enum InternetConnectionState : int
        {
            INTERNET_CONNECTION_MODEM = 0x1,  //调制解调器
            INTERNET_CONNECTION_LAN = 0x2, //局域网
            INTERNET_CONNECTION_PROXY = 0x4, //代理
            INTERNET_RAS_INSTALLED = 0x10, //？
            INTERNET_CONNECTION_OFFLINE = 0x20, // 通道？
            INTERNET_CONNECTION_CONFIGURED = 0x40//？
        }

    }
}
