using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinDownLoad
{
    public class WinISODownLoad
    {
        public async Task<string[]> GetWin10ISO1(string productEditionId)
        {
            string sessionId = Guid.NewGuid().ToString();
            string referer = "https://www.microsoft.com/zh-cn/software-download/windows10ISO";
            string skuId = "WN7-00588";
            string url1 = $"https://www.microsoft.com/software-download-connector/api/getskuinformationbyproductedition?profile=606624d44113&ProductEditionId={productEditionId}&SKU=undefined&friendlyFileName=undefined&Locale=zh-CN&sessionID={sessionId}";
            string url2 = $"https://www.microsoft.com/software-download-connector/api/GetProductDownloadLinksBySku?profile=606624d44113&ProductEditionId=undefined&SKU={skuId}&friendlyFileName=undefined&Locale=zh-CN&sessionID={sessionId}";
            string[] ret = await GetWinISO1(productEditionId, url1, url2, referer, sessionId);
            return ret;
        }
        public async Task<string[]> GetWin11ISO2(string productEditionId)
        {
            string sessionId = Guid.NewGuid().ToString();
            string referer = "https://www.microsoft.com/zh-cn/software-download/windows11";
            string skuId = "WN7-00588";
            string url1 = $"https://www.microsoft.com/software-download-connector/api/getskuinformationbyproductedition?profile=606624d44113&ProductEditionId={productEditionId}&SKU=undefined&friendlyFileName=undefined&Locale=zh-CN&sessionID={sessionId}";
            string url2 = $"https://www.microsoft.com/software-download-connector/api/GetProductDownloadLinksBySku?profile=606624d44113&ProductEditionId=undefined&SKU={skuId}&friendlyFileName=undefined&Locale=zh-CN&sessionID={sessionId}";
            string[] ret = await GetWinISO1(productEditionId, url1, url2, referer, sessionId);
            return ret;
        }

        public async Task<string[]> GetWinISO1(string productEditionId, string url1, string url2, string referer, string sessionId)
        {
            string skuId = string.Empty;//下载语言skuId
            string u1 = $"https://vlscppe.microsoft.com/fp/tags.js?org_id=y6jn8c31&session_id={sessionId}";
            string u2 = $"https://ov-df.microsoft.com/mdt.js?instanceId=3540d1d7-3513-4ec3-b52a-a8617733a58c&pageId=si&session_id={sessionId}";

            CookieContainer cookieContainer = new CookieContainer();
            HttpClientHelp httpClientHelp = new HttpClientHelp();
            var t = await httpClientHelp.HttpClientGet(u1, cookieContainer, new Action<CookieContainer>((x) => cookieContainer = x), referer);
            //t.Wait();
            t = await httpClientHelp.HttpClientGet(u2, cookieContainer, new Action<CookieContainer>((x) => cookieContainer = x), referer);
            //t.Wait();

            //t = httpClientHelp.HttpClientGet(url1, cookieContainer, new Action<CookieContainer>((x) => cookieContainer = x), referer);
            t = await httpClientHelp.HttpClientGet(url1, cookieContainer, new Action<CookieContainer>((x) => cookieContainer = x), referer);
            //t.Wait();
            skuId = httpClientHelp.GetskuID1(t);

            url2 = url2.Replace("WN7-00588", skuId);
            t = await httpClientHelp.HttpClientGet(url2, cookieContainer, new Action<CookieContainer>((x) => cookieContainer = x), referer);

            //t.Wait();
            if (t.Contains("errorModalTitle") || t.Contains("errorModalMessage") || t.Contains("ErrorSettings.SentinelReject"))//{"Errors":[{"Key":"ErrorSettings.SentinelReject","Value":"Sentinel marked this request as rejected.","Type":9}]}
            {
                return null;
            }
            string[] ret = httpClientHelp.GetWinISOaddr1(t);
            return ret;
        }
    }
}
