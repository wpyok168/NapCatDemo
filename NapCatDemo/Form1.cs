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
using System.IO;
using MsTool.IniFolder;
using WinDownLoad.MyDB;

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
            //WinISODownLoadAPI winapi = new WinISODownLoadAPI();
            //winapi.SendMsgEvent += (szGruopId, szQQId, obj, isprivatemsg) =>
            //{
            //    Type type1 = obj.GetType();
            //    string name = type1.Name;
            //    string tn = type1.ToString();
            //};
            //winapi.GetWin11ISOaddr(414725048, 414725048, "", "3113", false);
            CreateWinConfig();
            TouchSocket();
        }
        WebSocketClient client = new WebSocketClient();
        private void CreateWinConfig()
        {
            CreateCQDb db = new CreateCQDb();
            db.createDB();
            if (!File.Exists(MTComon.Path))
            {
                IObject iobj = new IObject(MTComon.Path)
                {
                    new ISection("office下载")
                    {
                        {"1.Office 2013 专业增强零售版","http://officecdn.microsoft.com.edgesuite.net/db/39168D7E-077B-48E7-872C-B232C3E72675/media/zh-cn/ProPlusRetail.img;https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=ProfessionalRetail&platform=X86&language=zh-CN"},
                        {"2.Office 2016 专业增强零售版","https://officecdn.microsoft.com/db/492350F6-3A01-4F97-B9C0-C7C6DDF67D60/media/zh-CN/ProPlusRetail.img;https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=ProPlusRetail&platform=X86&language=zh-CN"},
                        {"3.Office 2019 专业增强零售版","https://officecdn.microsoft.com/pr/492350f6-3a01-4f97-b9c0-c7c6ddf67d60/media/zh-cn/ProPlus2019Retail.img;https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=ProPlus2019&platform=X86&language=zh-CN"},
                        {"4.Office 2021 专业增强零售版","https://officecdn.microsoft.com/pr/492350f6-3a01-4f97-b9c0-c7c6ddf67d60/media/zh-cn/ProPlus2021Retail.img;https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=ProPlus2021&platform=X86&language=zh-CN"},
                        {"5.Office 365 专业增强零售版","http://officecdn.microsoft.com.edgesuite.net/db/492350F6-3A01-4F97-B9C0-C7C6DDF67D60/media/zh-cn/O365ProPlusRetail.img;https://c2rsetup.officeapps.live.com/c2r/download.aspx?ProductreleaseID=O365ProPlusRetail&platform=X86&language=zh-CN"},
                    }
                };
                iobj.Save();
                MTComon.Inode = LoadIni.Load();
            }
            else
            {
                MTComon.Inode = LoadIni.Load();
            }
        }
        private void TouchSocket()
        {
            //var client = new WebSocketClient();
            client.Setup(new TouchSocketConfig().SetRemoteIPHost("ws://127.0.0.1:3001")
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
                        //RobotQQ(recmsg);
                        RobotWork(recmsg);
                        DownLoadofficeISO(recmsg);
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
            string user_id = "2403875843";
            string sendmsg = "你好！ NapCat！！";
            string msg1 = $"{{\"action\":\"send_private_msg\",\"params\":{{\"user_id\":{user_id},\"message\":{sendmsg},\"auto_escape\":false}}, \"echo\":\"\"}}";
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
                    GetMsgText(recmsg);
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

        private async void SendMsg(string recmsg,string sendmsg)
        {
            try
            {
                JsonDocument recmsgdic = System.Text.Json.JsonDocument.Parse(recmsg);
                if (recmsgdic != null)
                {
                    if (recmsgdic.RootElement.TryGetProperty("message", out JsonElement message))
                    {
                        if (message.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in message.EnumerateArray())
                            {
                                if (item.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("text", out JsonElement text))
                                {
                                    string msgtext = text.GetString();
                                    if (recmsgdic.RootElement.TryGetProperty("sub_type", out JsonElement subtype))
                                    {
                                        if (subtype.GetString().Equals("friend") && !string.IsNullOrEmpty(msgtext)) //好友消息
                                        {
                                            if (recmsgdic.RootElement.TryGetProperty("sender", out JsonElement sender) && sender.TryGetProperty("user_id", out JsonElement user_id))
                                            {
                                                long _user_id = user_id.GetInt64();
                                                sendmsg = sendmsg.Replace("\r", "\\r").Replace("\n", "\\n");
                                                string msg1 = $@"{{""action"":""send_private_msg"",""params"":{{""user_id"":{_user_id},""message"":""{sendmsg.ToString()}"",""auto_escape"":false}}, ""echo"":""""}}";
                                                if (this.richTextBox1.InvokeRequired)
                                                {
                                                    this.richTextBox1.Invoke(new Action(() => { this.richTextBox1.AppendText(sendmsg+"\r\n"); }));
                                                }

                                                await client.SendAsync(msg1);
                                            }
                                        }
                                        else //if (subtype.GetString().Equals("group") && !string.IsNullOrEmpty(msgtext))
                                        {
                                            //message_type":"group"，"sub_type":"normal" 群聊  "message_type":"private"，"sub_type":"group" 群私聊  "message_type":"private"，"sub_type":"friend" 好友
                                            if (recmsgdic.RootElement.TryGetProperty("message_type", out JsonElement messagetype))
                                            {
                                                RecMsgMode msg = GetRecMsgMode(recmsg);
                                                if (messagetype.GetString().Equals("private"))//群私聊消息
                                                {
                                                    sendmsg = sendmsg.Replace("\r", "\\r").Replace("\n", "\\n");
                                                    string msg1 = $"{{\"action\":\"send_msg\",\"params\":{{\"message_type\":\"private\", \"user_id\":{msg.UserID},\"group_id\":{msg.GroupID}, \"message\":\"{sendmsg}\",\"auto_escape\":false}}, \"echo\":\"\"}}";
                                                    if (this.richTextBox1.InvokeRequired)
                                                    {
                                                        this.richTextBox1.Invoke(new Action(() => { this.richTextBox1.AppendText(sendmsg + "\r\n"); }));
                                                    }
                                                    await client.SendAsync(msg1);
                                                }
                                                else if (messagetype.GetString().Equals("group"))//群消息
                                                {
                                                    sendmsg = sendmsg.Replace("\r", "\\r").Replace("\n", "\\n");
                                                    string msg1 = $"{{\"action\":\"send_group_msg\",\"params\":{{\"group_id\":{msg.GroupID}, \"message\":\"{sendmsg}\",\"auto_escape\":false}}, \"echo\":\"\"}}";
                                                    if (this.richTextBox1.InvokeRequired)
                                                    {
                                                        this.richTextBox1.Invoke(new Action(() => { this.richTextBox1.AppendText(sendmsg + "\r\n"); }));
                                                    }
                                                    await client.SendAsync(msg1);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        private string GetMsgText(string recmsg)
        {
            System.Text.Json.JsonDocument recmsgdic = System.Text.Json.JsonDocument.Parse(recmsg);
            string msgtext = string.Empty;
            if (recmsgdic.RootElement.TryGetProperty("message", out JsonElement message))
            {
                if(message.ValueKind==JsonValueKind.Array)
                {
                    foreach (var item in message.EnumerateArray())
                    {
                        if (item.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("text", out JsonElement text))
                        {
                            msgtext = text.GetString();
                        }
                    }
                }
            }
            return msgtext;
        }

        private RecMsgMode GetRecMsgMode(string recmsg)
        {
            RecMsgMode recmsgMode = new RecMsgMode();
            System.Text.Json.JsonDocument recmsgdic = System.Text.Json.JsonDocument.Parse(recmsg);
            if (recmsgdic.RootElement.TryGetProperty("group_id", out JsonElement group_id))
            {
                recmsgMode.GroupID = group_id.GetInt64();
            }
            if (recmsgdic.RootElement.TryGetProperty("user_id", out JsonElement user_id))
            {
                recmsgMode.UserID = user_id.GetInt64();
            }
            if (recmsgdic.RootElement.TryGetProperty("sub_type", out JsonElement sub_type))
            {
                recmsgMode.IsFriend = sub_type.GetString().Equals("friend") ? true : false;
                recmsgMode.IsGroupPrivate = sub_type.GetString().Equals("group") ? true : false;
            }
            if (recmsgdic.RootElement.TryGetProperty("message", out JsonElement message))
            {
                if (message.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in message.EnumerateArray())
                    {
                        if (item.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("text", out JsonElement text))
                        {
                            recmsgMode.RecMsgContent = text.GetString();
                        }
                    }
                }
            }
            return recmsgMode;
        }

        private static ISection isection = null;
        private static List<KeyValuePair<string, IValue>> isection1 = null;

        private void RobotWork(string recmsg)
        {
            if (!string.IsNullOrEmpty(GetRecMsgMode(recmsg).RecMsgContent) && GetRecMsgMode(recmsg).RecMsgContent.Replace(" ", "").Equals("下载菜单"))
            {
                CreateWinConfig();
                string sendstr = "win11下载\r\nwin10下载\r\n";
                //string sendstr = "win10下载\r\nwin8.1下载\r\nwin7下载\r\n";
                for (int i = 0; i < MTComon.Inode.Count(); i++)
                {
                    sendstr += (MTComon.Inode.ElementAt(i).SectionName + "\r\n");
                }
                SendMsg(recmsg, sendstr);
            }
        }

        

        private void DownLoadofficeISO(string recmsg)
        {
            RecMsgMode msg = GetRecMsgMode(recmsg);
            if (msg.RecMsgContent == null) { return; }
            int isname = -1;
            for (int i = 0; i < MTComon.Inode.Count; i++)
            {
                string SectionName = MTComon.Inode.ElementAt(i).SectionName.ToLower();
                string tempstr = msg.RecMsgContent.Replace(" ", "");
                if (tempstr.Equals(SectionName))
                {
                    isname = i;
                }

            }
            //string nodename = MTComon.Inode[e.Msg.ToLower().Replace(" ", "")].SectionName;
            bool[] falg = dlof10.Select(t => t.Key == msg.UserID).ToArray();

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
                    dlof10.Add(msg.UserID, msg.GroupID);
                }
                catch (Exception)
                {

                }
                SendMsg(recmsg, sendstr);
                return;
            }
            if (dlof10.Count > 0)
            {
                KeyValuePair<long, long> windwl = dlof10.FirstOrDefault(t => t.Key == msg.UserID);
                if (windwl.Key == msg.UserID)
                {
                    try
                    {
                        if (Regex.IsMatch(msg.RecMsgContent, "\\d") && int.Parse(msg.RecMsgContent) <= isection.Count)
                        {
                            dlof10.Remove(msg.UserID);
                            SendMsg(recmsg, isection1.ElementAt(int.Parse(msg.RecMsgContent) - 1).Value.ToString().Replace(";", "\r\n")); //ini配置文件分隔符
                        }
                        else
                        {
                            dlof10.Remove(msg.UserID);
                        }
                    }
                    catch (Exception)
                    {
                        dlof10.Remove(msg.UserID);
                    }
                }
            }
            DownLoadWinISO(recmsg);
        }
        private static Dictionary<long, long> dlof10 = new Dictionary<long, long>();
        private static Dictionary<long, long> dlwin10 = new Dictionary<long, long>();
        private static Dictionary<long, long> dlwin7 = new Dictionary<long, long>();
        private static Dictionary<long, long> dlwin11 = new Dictionary<long, long>();

        public void DownLoadWinISO(string recmsg)
        {

            RecMsgMode msg = GetRecMsgMode(recmsg);
            if (msg.RecMsgContent.ToLower().Replace(" ", "").Equals("win10下载"))
            {

                try
                {
                    dlwin10.Add(msg.UserID, msg.UserID);
                }
                catch (Exception)
                {

                    throw;
                }
                SendMsg(recmsg, WinVerInfo.sendst_10);
                return;
            }
            if (dlwin10.Count > 0)
            {
                KeyValuePair<long, long> windwl = dlwin10.FirstOrDefault(t => t.Key == msg.UserID);
                if (windwl.Key == msg.UserID)
                {
                    try
                    {
                        if (Regex.IsMatch(msg.RecMsgContent, "\\d") && int.Parse(msg.RecMsgContent) <= WinVerInfo.win10ver.Count)
                        {
                            dlwin10.Remove(msg.UserID);
                            if (GetDbISOAddr(recmsg, WinVerInfo.win10ver.ElementAt(int.Parse(msg.RecMsgContent) - 1).Value))
                            {
                                WinISODownLoadAPI winl = new WinISODownLoadAPI();
                                winl.SendMsgEvent += (szGruopId, szQQId, obj, isprivatemsg) =>
                                {
                                    if (obj==null)
                                    {
                                        SendMsg(recmsg, "获取失败");
                                        return;
                                    }
                                    Type type1 = obj.GetType();
                                    string name = type1.Name;
                                    string tn = type1.ToString();
                                    if (type1.ToString().ToLower().Equals("system.string"))
                                    {
                                        SendMsg(recmsg, obj as string);
                                    }
                                    else if (type1.ToString().ToLower().Equals("system.string[]"))
                                    {
                                        string[] isoaddr = (string[])obj;
                                        if (isoaddr != null && isoaddr.Length > 1)
                                        {
                                            string x64 = string.Empty; string x32 = string.Empty;
                                            foreach (string s in isoaddr) 
                                            {
                                                if (s.Contains("_x64"))
                                                {
                                                    x64 = s;
                                                }
                                                else
                                                {
                                                    x32 = s; 
                                                }   
                                            }
                                            SendMsg(recmsg, "32位：" + x32 + "\r\n" + "64位：" + x64 + "\r\n" + " (此链接24小时内有效!)");
                                        }
                                        
                                    }

                                };
                                winl.GetWin10ISOaddr(windwl.Value, msg.GroupID, WinVerInfo.win10ver.ElementAt(int.Parse(msg.RecMsgContent) - 1).Key, WinVerInfo.win10ver.ElementAt(int.Parse(msg.RecMsgContent) - 1).Value, true);
                            }
                            // Common.xlzAPI.SendGroupMessage(e.ThisQQ, e.FromQQ, e.Msg);
                        }
                        else
                        {
                            dlwin10.Remove(msg.UserID);
                        }
                    }
                    catch (Exception)
                    {
                        dlwin10.Remove(msg.UserID);
                    }
                }
            }
            
            win11(recmsg);
        }
        private void win11(string recmsg)
        {
            RecMsgMode msg = GetRecMsgMode(recmsg);
            if (msg.RecMsgContent.ToLower().Replace(" ", "").Equals("win11下载"))
            {
                try
                {
                    dlwin11.Add(msg.UserID, msg.UserID);
                }
                catch (Exception)
                {

                    throw;
                }
                SendMsg(recmsg, WinVerInfo.sendstr_11);
                return;
            }
            if (dlwin11.Count > 0)
            {
                KeyValuePair<long, long> windwl = dlwin11.FirstOrDefault(t => t.Key == msg.UserID);
                if (windwl.Key == msg.UserID)
                {
                    try
                    {
                        if (Regex.IsMatch(msg.RecMsgContent, "\\d") && int.Parse(msg.RecMsgContent) <= WinVerInfo.win11ver.Count)
                        {
                            dlwin11.Remove(msg.UserID);
                            if (GetDbISOAddr(recmsg, WinVerInfo.win11ver.ElementAt(int.Parse(msg.RecMsgContent) - 1).Value))
                            {
                                WinISODownLoadAPI winl = new WinISODownLoadAPI();
                                winl.SendMsgEvent += (szGruopId, szQQId, obj, isprivatemsg) =>
                                {
                                    Type type1 = obj.GetType();
                                    string name = type1.Name;
                                    string tn = type1.ToString();
                                    if (type1.ToString().ToLower().Equals("system.string"))
                                    {
                                        SendMsg(recmsg, obj as string);
                                    }
                                    else if (type1.ToString().ToLower().Equals("system.string[]"))
                                    {
                                        string[] isoaddr = (string[])obj;
                                        if (isoaddr != null && isoaddr.Length > 1)
                                        {
                                            string x64 = string.Empty; string x32 = string.Empty;
                                            foreach (string s in isoaddr)
                                            {
                                                if (s.Contains("_x64"))
                                                {
                                                    x64 = s;
                                                }
                                                else
                                                {
                                                    x32 = s;
                                                }
                                            }
                                            SendMsg(recmsg, "32位：" + x32 + "\r\n" + "64位：" + x64 + "\r\n" + " (此链接24小时内有效!)");
                                        }
                                        //SendMsg(recmsg, "32位：" + isoaddr[0] + "\r\n" + "64位：" + isoaddr[1] + "\r\n" + " (此链接24小时内有效!)");
                                    }
                                    
                                };
                                winl.GetWin11ISOaddr(windwl.Value, msg.GroupID, WinVerInfo.win11ver.ElementAt(int.Parse(msg.RecMsgContent) - 1).Key, WinVerInfo.win11ver.ElementAt(int.Parse(msg.RecMsgContent) - 1).Value, true);
                            }
                            // Common.xlzAPI.SendGroupMessage(e.ThisQQ, e.FromQQ, e.Msg);
                        }
                        else
                        {
                            dlwin11.Remove(msg.UserID);
                        }
                    }
                    catch (Exception)
                    {
                        dlwin11.Remove(msg.UserID);
                    }
                }
            }
        }
        private bool GetDbISOAddr(string recmsg, string key)
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
                            SendMsg(recmsg, "32位：" + addr[0] + "\r\n" + "64位：" + addr[1] + "\r\n" + $" (此链接{dt.Hours}时{dt.Minutes}分内有效!)");
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
        private bool GetDb11ISOAddr(string recmsg)
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
                            SendMsg(recmsg, "32位：" + addr[0] + "\r\n" + "64位：" + addr[1] + "\r\n" + $" (此链接{dt.Hours}时{dt.Minutes}分内有效!)");
                            falg = false;
                        }
                    }
                }
            }
            dr.Close();
            return falg;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Close();
        }
    }
}
