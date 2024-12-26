using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TouchSocket.Core;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;
using TouchSocket.Http;
using WinDownLoad;
using System.Text.Json;
using Native.Tool.IniConfig.Linq;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace NapCatDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //WinISODownLoadAPI  winapi = new WinISODownLoadAPI();
            //winapi.SendMsgEvent += (szGruopId, szQQId, obj, isprivatemsg) =>
            //{
            //   Type type1 = obj.GetType();
            //    string name = type1.Name;
            //    string tn = type1.ToString();
            //};
            //winapi.GetWin11ISOaddr(414725048, 414725048, "", "3113", false);
            TouchSocket();
        }
        WebSocketClient client = new WebSocketClient();
        private event Action<long, long, object, bool> SendMsgEvent;
        private void TouchSocket()
        {
            //var client = new WebSocketClient();
            client.Setup(new TouchSocketConfig().SetRemoteIPHost("ws://127.0.0.1:5800")
                .ConfigureContainer(a =>
                {
                    a.AddFileLogger();
                })
                .ConfigurePlugins(a =>
                {
                    a.UseWebSocketHeartbeat()//使用心跳插件
                    .SetTick(TimeSpan.FromSeconds(1));//每5秒ping一次。
                }));
            client.Connect();
            client.Received = (c, e) =>
            {
                switch (e.DataFrame.Opcode)
                {
                    case WSDataType.Cont:
                        break;
                    case WSDataType.Text:
                        Console.WriteLine(e.DataFrame.ToText());
                        string recmsg = e.DataFrame.ToText();
                        if (e.DataFrame.ToText().ToString().Contains("414725048"))
                        {
                            //string msg = "{\"user_id\":2403875843 ,\"message\":\"480325208 \"}";
                            //TouchSendHttp("/send_private_msg", msg);

                            //socket模式暂时没搞定
                            // string msg1 = "{\"action\": \"send_private_msg\", \"params\": {\"user_id\": 414725048,\"message\": \"hello\"},\"echo\": \"123456\" }";
                            //TouchSendSocket("/send_private_msg", msg1);
                            //client.SendAsync(msg1).Wait();  
                            //{"action":"send_private_msg","user_id":2403875843,"message":"你好！ NapCat！！","auto_escape":false}
                            //string msg1 = "{\"action\":\"send_private_msg\",\"params\":{\"user_id\":2403875843,\"message\":\"你好！ NapCat ws\",\"auto_escape\":false}, \"echo\":\"\"}";
                            //client.SendAsync(msg1).Wait();
                        }
                        RobotQQ(recmsg);
                        break;
                    case WSDataType.Binary:
                        byte[] by = e.DataFrame.PayloadData.ReadBytesPackage();
                        break;
                    case WSDataType.Close:
                        break;
                    case WSDataType.Ping:
                        break;
                    case WSDataType.Pong:
                        break;
                    default:
                        break;
                }

                return null;
            };
        }
        private static async void TouchSendHttp(string api, string msg)
        {
            HttpClient client = new HttpClient();

            client.Setup(new TouchSocketConfig().SetRemoteIPHost(new IPHost("http://localhost:3000")));
            client.Connect();//先做连接
            HttpRequest request = new HttpRequest();
            request
                .InitHeaders()
                .SetUrl(api)
                .SetHost(client.RemoteIPHost.Host)
                .SetContent(msg)
                .AsPost();
            var respose = await client.RequestAsync(request, millisecondsTimeout: 1000 * 10);
            Console.WriteLine(respose.Response.GetBody());
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string msg1 = "{\"action\":\"send_private_msg\",\"params\":{\"user_id\":2403875843,\"message\":\"你好！ NapCat ws\",\"auto_escape\":false}, \"echo\":\"\"}";
            await client.SendAsync(msg1);
        }

        private void RobotQQ(string recmsg) 
        {
            try
            {
                // JsonDocument recmsgdic = System.Text.Json.JsonDocument.Parse(recmsg);
                Dictionary<string, object> recmsgdic = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,object>>(recmsg);
                if (recmsgdic != null)
                {
                    string msgtext = string.Empty;
                    if (recmsgdic.ContainsKey("message"))
                    {
                        RobotWork(recmsg); 
                        JsonElement msgdic = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(recmsgdic["message"].ToString());
                        if (msgdic.ValueKind==JsonValueKind.Array)
                        {
                            foreach (var item in msgdic.EnumerateArray())
                            {
                                if (item.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("text", out JsonElement text))
                                {
                                    msgtext = text.GetString();

                                }
                            }
                        }
                        
                        
                    }
                    
                    if (recmsgdic["sub_type"].ToString().Equals("friend") && string.IsNullOrEmpty(msgtext))
                    {

                    }
                    else if (recmsgdic["sub_type"].ToString().Equals("group") && string.IsNullOrEmpty(msgtext))
                    {
                        if (recmsgdic["message_type"].ToString().Equals("private"))
                        {

                        }
                        else if (recmsgdic["message_type"].ToString().Equals("group"))
                        {

                        }
                    }
                }
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static ISection isection = null;
        private static List<KeyValuePair<string, IValue>> isection1 = null;

        private void RobotWork(string recmsg)
        {
            if (recmsg.Replace(" ", "").Equals("下载菜单"))
            {
                string sendstr = "win11下载\r\nwin10下载\r\nwin8.1下载\r\n";
                //string sendstr = "win10下载\r\nwin8.1下载\r\nwin7下载\r\n";
                for (int i = 0; i < MTComon.Inode.Count(); i++)
                {
                    sendstr += (MTComon.Inode.ElementAt(i).SectionName + "\r\n");
                }
                MC_SDK.Common.MC_API.SendPrivateMsg_(e.FromQQ, sendstr);
                SendMsgEvent += (gid,fid,msg,flag)=> { };
            }
        }

        private void Form1_SendMsgEvent(long arg1, long arg2, object arg3, bool arg4)
        {
            throw new NotImplementedException();
        }

        private void DownLoadofficeISO(PrivateMsg e)
        {
            int isname = -1;
            for (int i = 0; i < MTComon.Inode.Count; i++)
            {
                string SectionName = MTComon.Inode.ElementAt(i).SectionName.ToLower();
                string tempstr = e.Msg.ToLower().Replace(" ", "");
                if (tempstr.Equals(SectionName))
                {
                    isname = i;
                }

            }
            //string nodename = MTComon.Inode[e.Msg.ToLower().Replace(" ", "")].SectionName;
            bool[] falg = dlof10.Select(t => t.Key == e.FromQQ).ToArray();

            if (isname >= 0 && falg.Count() == 0)
            {
                isection = MTComon.Inode.ElementAt(isname);
                isection1 = isection.OrderBy(x => {
                    try
                    {
                        return int.Parse(Regex.Match(x.Key, @"\d+").Value);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }).ToList();
                string sendstr = string.Empty;

                foreach (var item in isection1)
                {
                    sendstr += (item.Key + "\r\n");
                }
                try
                {
                    dlof10.Add(e.FromQQ, e.FromGroup);
                }
                catch (Exception)
                {

                }
                MC_SDK.Common.MC_API.SendPrivateMsg_(e.FromQQ, sendstr);
                return;
            }
            if (dlof10.Count > 0)
            {
                KeyValuePair<long, long> windwl = dlof10.FirstOrDefault(t => t.Key == e.FromQQ);
                if (windwl.Key == e.FromQQ)
                {
                    try
                    {
                        if (Regex.IsMatch(e.Msg, "\\d") && int.Parse(e.Msg) <= isection.Count)
                        {
                            dlof10.Remove(e.FromQQ);
                            MC_SDK.Common.MC_API.SendPrivateMsg_(e.FromQQ, isection1.ElementAt(int.Parse(e.Msg) - 1).Value.ToString().Replace(";", "\r\n")); //ini配置文件分隔符
                        }
                        else
                        {
                            dlof10.Remove(e.FromQQ);
                        }
                    }
                    catch (Exception)
                    {
                        dlof10.Remove(e.FromQQ);
                    }
                }
            }

        }
        private static Dictionary<long, long> dlof10 = new Dictionary<long, long>();
        private static Dictionary<long, long> dlwin10 = new Dictionary<long, long>();
        private static Dictionary<long, long> dlwin7 = new Dictionary<long, long>();
        private static Dictionary<long, long> dlwin11 = new Dictionary<long, long>();

        public void DownLoadWinISO(PrivateMsg e)
        {
            //KJ8Q3-YP78Y-JBTDM-KGRQQ-49YDC
            Debug.Print(e.Msg.ToLower().Replace(" ", ""));
            if (e.Msg.ToLower().Replace(" ", "").Equals("win7---###----下载"))
            {
                //WinISODownLoad winl = new WinISODownLoad();
                ////YFFJQ-YCWH8-2PWJ8-BX834-VQ66G //KJ8Q3-YP78Y-JBTDM-KGRQQ-49YDC
                //winl.GetWin7Download("YFFJQ-YCWH8-2PWJ8-BX834-VQ66G", e.FromQQ, e.ThisQQ, "", true);

                try
                {
                    dlwin7.Add(e.FromQQ, e.FromQQ);
                }
                catch (Exception)
                {

                    throw;
                }
                MC_SDK.Common.MC_API.SendPrivateMsg_(e.FromQQ, WinVerInfo.sendstr_7);
                return;
            }
            if (e.Msg.ToLower().Replace(" ", "").Equals("win8.1下载"))
            {
                if (GetDb81ISOAddr(e))
                {
                    WinISODownLoadAPI winl = new WinISODownLoadAPI();
                    winl.GetWin81ISOaddr(e.FromQQ, long.Parse(MC_SDK.Common.MC_API.GetRobotQQ()), "", true);
                }

            }
            if (e.Msg.ToLower().Replace(" ", "").Equals("win10下载"))
            {

                try
                {
                    dlwin10.Add(e.FromQQ, e.FromQQ);
                }
                catch (Exception)
                {

                    throw;
                }
                MC_SDK.Common.MC_API.SendPrivateMsg_(e.FromQQ, WinVerInfo.sendst_10);
                return;
            }
            if (dlwin10.Count > 0)
            {
                KeyValuePair<long, long> windwl = dlwin10.FirstOrDefault(t => t.Key == e.FromQQ);
                if (windwl.Key == e.FromQQ)
                {
                    try
                    {
                        if (Regex.IsMatch(e.Msg, "\\d") && int.Parse(e.Msg) <= WinVerInfo.win10ver.Count)
                        {
                            dlwin10.Remove(e.FromQQ);
                            if (GetDbISOAddr(e, WinVerInfo.win10ver.ElementAt(int.Parse(e.Msg) - 1).Value))
                            {
                                WinISODownLoadAPI winl = new WinISODownLoadAPI();
                                winl.GetWin10ISOaddr(windwl.Value, long.Parse(MC_SDK.Common.MC_API.GetRobotQQ()), WinVerInfo.win10ver.ElementAt(int.Parse(e.Msg) - 1).Key, WinVerInfo.win10ver.ElementAt(int.Parse(e.Msg) - 1).Value, true);
                            }
                            // Common.xlzAPI.SendGroupMessage(e.ThisQQ, e.FromQQ, e.Msg);
                        }
                        else
                        {
                            dlwin10.Remove(e.FromQQ);
                        }
                    }
                    catch (Exception)
                    {
                        dlwin10.Remove(e.FromQQ);
                    }
                }
            }
            if (dlwin7.Count > 0)
            {
                KeyValuePair<long, long> windwl = dlwin7.FirstOrDefault(t => t.Key == e.FromQQ);
                if (windwl.Key == e.FromQQ)
                {
                    try
                    {
                        if (Regex.IsMatch(e.Msg, "\\d") && int.Parse(e.Msg) <= WinVerInfo.win7ver.Count)
                        {
                            dlwin7.Remove(e.FromQQ);
                            if (GetDbISOAddr(e, WinVerInfo.win7ver.ElementAt(int.Parse(e.Msg) - 1).Value))
                            {
                                WinISODownLoadAPI winl = new WinISODownLoadAPI();

                                //YFFJQ-YCWH8-2PWJ8-BX834-VQ66G //KJ8Q3-YP78Y-JBTDM-KGRQQ-49YDC
                                winl.GetWin7ISOaddr(WinVerInfo.win7ver.ElementAt(int.Parse(e.Msg) - 1).Value, windwl.Value, long.Parse(MC_SDK.Common.MC_API.GetRobotQQ()), "", true, WinVerInfo.win7ver.ElementAt(int.Parse(e.Msg) - 1).Key);
                            }
                        }
                        else
                        {
                            dlwin7.Remove(e.FromQQ);
                        }
                    }
                    catch (Exception ex)
                    {
                        dlwin7.Remove(e.FromQQ);
                    }
                }
            }
            win11(e);
        }
        private void win11(PrivateMsg e)
        {
            if (e.Msg.ToLower().Replace(" ", "").Equals("win11下载"))
            {
                try
                {
                    dlwin11.Add(e.FromQQ, e.FromQQ);
                }
                catch (Exception)
                {

                    throw;
                }
                MC_SDK.Common.MC_API.SendPrivateMsg_(e.FromQQ, WinVerInfo.sendstr_11);
                return;
            }
            if (dlwin11.Count > 0)
            {
                KeyValuePair<long, long> windwl = dlwin11.FirstOrDefault(t => t.Key == e.FromQQ);
                if (windwl.Key == e.FromQQ)
                {
                    try
                    {
                        if (Regex.IsMatch(e.Msg, "\\d") && int.Parse(e.Msg) <= WinVerInfo.win11ver.Count)
                        {
                            dlwin11.Remove(e.FromQQ);
                            if (GetDbISOAddr(e, WinVerInfo.win11ver.ElementAt(int.Parse(e.Msg) - 1).Value))
                            {
                                WinISODownLoadAPI winl = new WinISODownLoadAPI();
                                winl.GetWin11ISOaddr(windwl.Value, long.Parse(MC_SDK.Common.MC_API.GetRobotQQ()), WinVerInfo.win11ver.ElementAt(int.Parse(e.Msg) - 1).Key, WinVerInfo.win11ver.ElementAt(int.Parse(e.Msg) - 1).Value, true);
                            }
                            // Common.xlzAPI.SendGroupMessage(e.ThisQQ, e.FromQQ, e.Msg);
                        }
                        else
                        {
                            dlwin11.Remove(e.FromQQ);
                        }
                    }
                    catch (Exception)
                    {
                        dlwin11.Remove(e.FromQQ);
                    }
                }
            }
        }
        private bool GetDbISOAddr(PrivateMsg e, string key)
        {
            bool falg = true;
            string sql = $"select * from isoaddr where 密钥='{key}'";
            //string sql = $"select * from isoaddr where 密钥='{this.versions7.ElementAt(int.Parse(e.Msg) - 1).Value}'";
            SqliteHelp_Until.SqliteHelp sqliteHelp = new SqliteHelp_Until.SqliteHelp(SqliteHelp_Until.DBConStr.sqlcon);
            System.Data.SQLite.SQLiteDataReader dr = sqliteHelp.SelcetDBDR(sql);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string dtdb = dr.GetValue(4).ToString();
                    TimeSpan dt = DateDiff(new DateTime(long.Parse(dtdb)), DateTime.Now);
                    string[] addr = dr.GetValue(3).ToString().Split('|');
                    if (dt.Hours > 00)
                    {
                        HttpClientHelp hch = new HttpClientHelp();
                        CookieContainer cc = new CookieContainer();
                        Task<string> t = null;
                        if (addr[0].Contains("win11"))
                        {
                            t = hch.HttpClientGet(addr[1], cc, new Action<CookieContainer>(x => cc = x), "");
                        }
                        else
                        {
                            t = hch.HttpClientGet(addr[0], cc, new Action<CookieContainer>(x => cc = x), "");
                        }
                        t.Wait();
                        if (t.Result == "403")
                        {
                            falg = true;
                        }
                        else
                        {
                            MC_SDK.Common.MC_API.SendPrivateMsg_(e.FromQQ, "32位：" + addr[0] + "\r\n" + "64位：" + addr[1] + "\r\n" + $" (此链接{dt.Hours}时{dt.Minutes}分内有效!)");
                            falg = false;
                        }
                    }
                }
            }
            dr.Close();
            return falg;
        }
        private TimeSpan DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts3 = ts1.Subtract(ts2).Duration();
            return ts3;
        }
        private bool GetDb11ISOAddr(PrivateMsg e)
        {
            bool falg = true;
            string sql = $"select * from isoaddr where 版本信息='win11'";
            SqliteHelp_Until.SqliteHelp sqliteHelp = new SqliteHelp_Until.SqliteHelp(SqliteHelp_Until.DBConStr.sqlcon);
            System.Data.SQLite.SQLiteDataReader dr = sqliteHelp.SelcetDBDR(sql);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string dtdb = dr.GetValue(4).ToString();
                    TimeSpan dt = DateDiff(new DateTime(long.Parse(dtdb)), DateTime.Now);
                    string[] addr = dr.GetValue(3).ToString().Split('|');
                    if (dt.Hours > 00)
                    {
                        HttpClientHelp hch = new HttpClientHelp();
                        CookieContainer cc = new CookieContainer();
                        var t = hch.HttpClientGet(addr[0], cc, new Action<CookieContainer>(x => cc = x), "");
                        t.Wait();
                        if (t.Result == "403")
                        {
                            falg = true;
                        }
                        else
                        {
                            MC_SDK.Common.MC_API.SendPrivateMsg_(e.FromQQ, "32位：" + addr[0] + "\r\n" + "64位：" + addr[1] + "\r\n" + $" (此链接{dt.Hours}时{dt.Minutes}分内有效!)");
                            falg = false;
                        }
                    }
                }
            }
            dr.Close();
            return falg;
        }
    }
}
