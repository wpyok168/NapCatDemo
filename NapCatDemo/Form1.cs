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
                    if (recmsgdic.ContainsKey("message"))
                    {
                        JsonElement msgdic = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(recmsgdic["message"].ToString());
                        if (msgdic.ValueKind==JsonValueKind.Array)
                        {
                            foreach (var item in msgdic.EnumerateArray())
                            {
                                if (item.TryGetProperty("data", out JsonElement data) && data.TryGetProperty("text", out JsonElement text))
                                {
                                    string msgtext = text.GetString();
                                }
                            }
                        }
                        
                        
                    }
                    
                    if (recmsgdic["sub_type"].ToString().Equals("friend"))
                    {

                    }
                    else if (recmsgdic["sub_type"].ToString().Equals("group"))
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
    }
}
