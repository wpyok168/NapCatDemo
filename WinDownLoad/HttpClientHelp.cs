using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


namespace WinDownLoad
{
    public class HttpClientHelp
    {
        public async Task<string> HttpClientGet(string url, CookieContainer cookieContainer, Action<CookieContainer> retCookieContainer, string referer)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
            ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                return true;
            };
            string ret = string.Empty;
            Dictionary<string, string> cc = new Dictionary<string, string>()
            {
                {"Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                {"Accept-Encoding","gzip, deflate, br" },
                {"Accept-Language","zh-CN,zh;q=0.9,en;q=0.8,ru;q=0.7,de;q=0.6" },
                //{"User-Agent","Mozilla/5.0 (Linux; Android 10; YAL-AL10 Build/HUAWEIYAL-AL10; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/89.0.4356.6 MQQBrowser/6.2 TBS/045434 Mobile Safari/537.36 V1_AND_SQ_8.5.0_1596_YYB_D QQ/8.5.0.5025 NetType/WIFI WebP/0.3.0 Pixel/1080 StatusBarHeight/108 SimpleUISwitch/0 QQTheme/999 InMagicWin/0" }
                {"User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0" }
            };

            using (HttpClientHandler hch = new HttpClientHandler())
            {
                hch.CookieContainer = cookieContainer;
                //hch.UseCookies = true;
                using (HttpClient hc = new HttpClient(hch))
                {
                    foreach (var item in cc)
                    {
                        hc.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                    }
                    if (!string.IsNullOrEmpty(referer))
                    {
                        hc.DefaultRequestHeaders.TryAddWithoutValidation("referer", referer);
                    }
                    try
                    {
                        using (var result = await hc.GetAsync(url))
                        {
                            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                if (result.ToString().ToLower().Contains("gzip"))
                                {
                                    using (Stream hcs = await result.Content.ReadAsStreamAsync())
                                    {
                                        using (GZipStream gs = new GZipStream(hcs, CompressionMode.Decompress))
                                        {
                                            ret = new StreamReader(gs, Encoding.UTF8).ReadToEnd();
                                        }
                                    }
                                }
                                else
                                {
                                    using (HttpContent content = result.Content)
                                    {
                                        ret = await content.ReadAsStringAsync();
                                    }
                                }

                            }
                            else if (result.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            {
                                ret = "403";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ret = ex.Message;
                    }

                    retCookieContainer(cookieContainer);
                }
            }
            return ret;
        }
        public async Task<string> HttpClientPost(string url, string data, CookieContainer cookieContainer, Action<CookieContainer> retCookieContainer, string referer)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
            ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            {
                return true;
            };
            string ret = string.Empty;
            Dictionary<string, string> cc = new Dictionary<string, string>()
            {
                {"Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9" },
                {"Accept-Encoding","gzip, deflate, br" },
                {"ContentType","application/x-www-form-urlencoded;application/json; charset=UTF-8" },
                {"Accept-Language","zh-CN,zh;q=0.9,en;q=0.8,ru;q=0.7,de;q=0.6" },
                //{"User-Agent","Mozilla/5.0 (Linux; Android 10; YAL-AL10 Build/HUAWEIYAL-AL10; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/89.0.4356.6 MQQBrowser/6.2 TBS/045434 Mobile Safari/537.36 V1_AND_SQ_8.5.0_1596_YYB_D QQ/8.5.0.5025 NetType/WIFI WebP/0.3.0 Pixel/1080 StatusBarHeight/108 SimpleUISwitch/0 QQTheme/999 InMagicWin/0" }
                {"User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0" }
            };

            using (HttpClientHandler hch = new HttpClientHandler())
            {
                hch.CookieContainer = cookieContainer;
                //hch.UseCookies = true;
                using (HttpClient hc = new HttpClient(hch))
                {
                    foreach (var item in cc)
                    {
                        hc.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                    }
                    if (!string.IsNullOrEmpty(referer))
                    {
                        hc.DefaultRequestHeaders.TryAddWithoutValidation("referer", referer);
                    }
                    try
                    {
                        StringContent sc = new StringContent(data, Encoding.UTF8);
                        using (var result = await hc.PostAsync(url, sc))
                        {
                            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                if (result.ToString().ToLower().Contains("gzip"))
                                {
                                    using (Stream hcs = await result.Content.ReadAsStreamAsync())
                                    {
                                        using (GZipStream gs = new GZipStream(hcs, CompressionMode.Decompress))
                                        {
                                            ret = new StreamReader(gs, Encoding.UTF8).ReadToEnd();
                                        }
                                    }
                                }
                                else
                                {
                                    using (HttpContent content = result.Content)
                                    {
                                        ret = await content.ReadAsStringAsync();
                                    }
                                }

                            }
                            else if (result.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            {
                                ret = "403";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ret = ex.Message;
                    }

                    retCookieContainer(cookieContainer);
                }
            }
            return ret;
        }

        
        public string[] GetWinISOaddr1(string html)
        {
            List<string> winUri = new List<string>();
            dynamic token = (new System.Web.Script.Serialization.JavaScriptSerializer()).DeserializeObject(html);
            foreach (Dictionary<string, object> item in token["ProductDownloadOptions"])
            {
                Debug.Print(item.Values.ElementAtOrDefault(3).ToString() + "\r\n" + item.Values.ElementAtOrDefault(1).ToString());
                Debug.Print(item["Uri"].ToString());
                //if (item["Language"].ToString().Contains("Chinese (Simplified)"))
                //{
                //    download64 = item["Uri"].ToString();
                //    break;
                //}
                ////English
                //if (item["Language"].ToString().Contains("Chinese (English)"))
                //{
                //    download32 = item["Uri"].ToString();
                //}
                winUri.Add(item["Uri"].ToString());
            }
            if (winUri.Count == 1) winUri.Add("");
            winUri.Reverse();
            return winUri.ToArray();
        }
        /// <summary>
        /// 获取中文语言对应的skuID
        /// </summary>
        /// <param name="outhtml"></param>
        /// <returns></returns>
        
        public string GetskuID1(string jsonstr)
        {
            string skuId = string.Empty;
            string skuId_en = string.Empty;
            dynamic token = (new System.Web.Script.Serialization.JavaScriptSerializer()).DeserializeObject(jsonstr);
            foreach (Dictionary<string, object> item in token["Skus"])
            {
                Debug.Print(item.Values.ElementAtOrDefault(3).ToString() + "\r\n" + item.Values.ElementAtOrDefault(1).ToString());
                Debug.Print(item["Language"].ToString());
                if (item["Language"].ToString().Contains("Chinese (Simplified)"))
                {
                    skuId = item["Id"].ToString();
                    break;
                }
                //English
                if (item["Language"].ToString().Contains("Chinese (English)"))
                {
                    skuId_en = item["Id"].ToString();
                }
            }
            return skuId;
        }
    }
}
