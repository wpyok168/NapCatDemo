using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqliteHelp_Until;

namespace WinDownLoad
{
    public class WinISODownLoadAPI
    {
        public event Action<long, long, object, bool> SendMsgEvent; 
        /// <summary>
        /// win10系统镜像下载地址
        /// </summary>
        /// <param name="szGruopId"></param>
        /// <param name="szQQId"></param>
        /// <param name="szVerName"></param>
        /// <param name="productEditionId"></param>
        /// <param name="isprivatemsg"></param>
        public async void GetWin10ISOaddr(long szGruopId, long szQQId, string szVerName, string productEditionId, bool isprivatemsg)
        {
            
            string str = "正从www.microsoft.com/en-us/software-download/windows10ISO获取Win10镜像(" + szVerName + "),请稍等...";
            SendMsgEvent?.Invoke(szGruopId, szQQId, str, isprivatemsg);
            

            WinISODownLoad winIsoDownLoad = new WinISODownLoad();
            string[] ret = await winIsoDownLoad.GetWin10ISO1(productEditionId);
            SendMsgEvent?.Invoke(szGruopId, szQQId, ret, isprivatemsg);
            //CreateDB dB = new CreateDB();
            //List<string> list = new List<string>() { productEditionId, szVerName, $"{ret[0]}|{ret[1]}", DateTime.Now.AddDays(1).Ticks.ToString() };
            //dB.SaveData("isoaddr", list, "密钥", productEditionId, true);
            SaveISO(ret, productEditionId, szVerName, "密钥");
        }
        /// <summary>
        /// win11系统镜像下载地址
        /// </summary>
        /// <param name="szGruopId"></param>
        /// <param name="szQQId"></param>
        /// <param name="szVerName"></param>
        /// <param name="productEditionId"></param>
        /// <param name="isprivatemsg"></param>
        public async void GetWin11ISOaddr(long szGruopId, long szQQId, string szVerName, string productEditionId, bool isprivatemsg)
        {
            string str = "正从https://www.microsoft.com/zh-cn/software-download/windows11获取Win11镜像(" + szVerName + "),请稍等...";
            SendMsgEvent?.Invoke(szGruopId, szQQId, str, isprivatemsg);
            System.Threading.Thread.Sleep(5000);
            SendMsgEvent?.Invoke(szGruopId, szQQId, "str", isprivatemsg);
            WinISODownLoad winIsoDownLoad = new WinISODownLoad();
            string[] ret = await winIsoDownLoad.GetWin11ISO2(productEditionId);
            if (ret == null)
            {
                //string[] ret1 = new string[] { "老版本微软不再提供 或者 程序错误，请联系作者修复", "老版本微软不再提供 或者 程序错误，请联系作者修复" };
                SendMsgEvent?.Invoke(szGruopId, szQQId, ret, isprivatemsg);
                return;
            }
            if (string.IsNullOrEmpty(ret[0]))
            {
                ret[0] = "微软未提供32位的win11系统下载地址";
            }
            SendMsgEvent?.Invoke(szGruopId, szQQId, ret, isprivatemsg);

            SaveISO(ret, productEditionId, szVerName, "密钥");
        }

        private void SaveISO(string[] ret, string productEditionId, string szVerName, string sqlkey)
        {
            if (ret == null)
            {
                return;
            }
            CreateDB dB = new CreateDB();
            List<string> list = new List<string>() { productEditionId, szVerName, $"{ret[0]}|{ret[1]}", DateTime.Now.AddDays(1).Ticks.ToString() };
            //dB.DelTbRow("isoaddr","密钥", productEditionId);
            dB.SaveData("isoaddr", list, sqlkey, productEditionId, true);
        }
    }
}
