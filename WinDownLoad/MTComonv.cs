using MsTool.IniFolder;
using Native.Tool.IniConfig.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinDownLoad
{
    public static class MTComon
    {
        private static IObject inode = LoadIni.Load();
        private static string path = System.Environment.CurrentDirectory + "\\下载配置.ini";
        private static CookieContainer cookieContainer = new CookieContainer();

        public static IObject Inode { get => inode; set => inode = value; }
        public static string Path { get => path; set => path = value; }
        public static CookieContainer CookieContainer { get => cookieContainer; set => cookieContainer = value; }
    }
}
