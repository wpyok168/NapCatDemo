using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinDownLoad
{
    public class WinVerInfo
    {
        public static Dictionary<string, string> win10ver = new Dictionary<string, string>() {
                    { "1.Win10_22H2 V1", "2618" },
                    { "2.Win10_22H2_Family V1", "2378" },
        };

        public static string sendst_10 = string.Concat(new string[] {
                   "1.Win10_22H2 中文版 V1","\r\n",
                   "2.Win10_22H2_家庭中文版 V1","\r\n",
        });

        public static string sendstr_11 = string.Concat(new string[] {
                    "1.Windows 11 22H2 V2 中文版","\r\n",
                    "2.Windows 11 22H2 V2 家庭中文版","\r\n",
                    "3.Windows 11 23H2    中文版","\r\n",
                    "4.Windows 11 23H2    家庭中文版","\r\n",
                    "5.Windows 11 23H2 V2 中文版","\r\n",
                    "6.Windows 11 23H2 V2 家庭中文版","\r\n",
                    "7.Windows 11 24H2    多版本","\r\n",
                    "8.Windows 11 24H2    家庭中文版","\r\n",
                    "9.Windows 11 24H2    专业中文版","\r\n",
        });
        public static Dictionary<string, string> win11ver = new Dictionary<string, string>() {
                    { "1.Windows 11 22H2  V2 中文版", "2616" },
                    { "2.Windows 11 22H2  V2 家庭中文版", "2617" },
                    { "3.Windows 11 23H2     中文版", "2860" },
                    { "4.Windows 11 23H2     家庭中文版", "2861" },
                    { "5.Windows 11 23H2  V2 中文版", "2935" },
                    { "6.Windows 11 23H2  V2 家庭中文版", "2936" },
                    { "7.Windows 11 24H2     多版本", "3113" },
                    { "8.Windows 11 24H2     家庭中文版", "3114" },
                    { "9.Windows 11 24H2     专业中文版", "3115" },

        };
    }
}
