using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SqliteHelp_Until
{
    public sealed class DESHelper
    {
        //密钥
        public static byte[] _KEY = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
        //向量
        public static byte[] _IV = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };

        /// <summary>
        /// DES加密操作1
        /// </summary>
        /// <param name="normalTxt"></param>
        /// <returns></returns>
        public static string DesEncrypt(string normalTxt)
        {
            //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(_KEY, _IV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(normalTxt);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();

            string strRet = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            return strRet;
        }

        /// <summary>
        /// DES解密操作1
        /// </summary>
        /// <param name="securityTxt">加密字符串</param>
        /// <returns></returns>
        public static string DesDecrypt(string securityTxt)//解密  
        {
            //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);
            byte[] byEnc;
            try
            {
                securityTxt.Replace("_%_", "/");
                securityTxt.Replace("-%-", "#");
                byEnc = Convert.FromBase64String(securityTxt);
            }
            catch
            {
                return null;
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(_KEY, _IV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        //==========以上为一组===============================

        /// <summary>
        /// DES加密操作2
        /// </summary>
        /// <param name="mes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string desEncryptString(string mes, string key)
        {
            byte[] inputBytes = ASCIIEncoding.UTF8.GetBytes(mes);

            //   往下 ===此段代码解决 des加密算法指定键的大小对于此算法无效（ASCIIEncoding.ASCII.GetBytes(encryptKey)）
            string encryptKeyall = Convert.ToString(key);    //定义密钥  
            if (encryptKeyall.Length < 9)
            {
                for (; ; )
                {
                    if (encryptKeyall.Length < 9)
                        encryptKeyall += encryptKeyall;
                    else
                        break;
                }
            }
            string encryptKey = encryptKeyall.Substring(0, 8);
            //   往上 ===此段代码解决 des加密算法指定键的大小对于此算法无效（ASCIIEncoding.ASCII.GetBytes(encryptKey)）

            MemoryStream outputStream = new MemoryStream();
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(encryptKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(encryptKey);
            ICryptoTransform desencrypt = des.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(outputStream, desencrypt, CryptoStreamMode.Write);
            cryptostream.Write(inputBytes, 0, inputBytes.Length);
            cryptostream.FlushFinalBlock();
            cryptostream.Close();

            string outputString = Convert.ToBase64String(outputStream.ToArray());
            return outputString;
        }

        /// <summary>
        /// DES解密操作2
        /// </summary>
        /// <param name="mes"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string desDecryptString(string mes, string key)
        {
            byte[] inputBytes = Convert.FromBase64String(mes);
            string encryptKeyall = Convert.ToString(key);    //定义密钥  
            if (encryptKeyall.Length < 9)
            {
                for (; ; )
                {
                    if (encryptKeyall.Length < 9)
                        encryptKeyall += encryptKeyall;
                    else
                        break;
                }
            }
            string encryptKey = encryptKeyall.Substring(0, 8);

            MemoryStream outputStream = new MemoryStream();
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Key = ASCIIEncoding.ASCII.GetBytes(encryptKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(encryptKey);
            ICryptoTransform desencrypt = des.CreateDecryptor();
            CryptoStream cryptostream = new CryptoStream(outputStream, desencrypt, CryptoStreamMode.Write);
            cryptostream.Write(inputBytes, 0, inputBytes.Length);
            cryptostream.FlushFinalBlock();
            cryptostream.Close();

            string outputString = ASCIIEncoding.UTF8.GetString(outputStream.ToArray());
            return outputString;
        }


        //==========以上为一组=================================

        /// <summary> 
        /// DES加密操作3   
        /// </summary>  
        /// <param name="str">要加密的字符串</param>  
        /// <param name="key">秘钥</param>  
        /// <returns>加密后的字符串</returns>  
        public static string desEncrypt(string str, string myKey)
        {
            string encryptKeyall = Convert.ToString(myKey);    //定义密钥  
            if (encryptKeyall.Length < 9)
            {
                for (; ; )
                {
                    if (encryptKeyall.Length < 9)
                        encryptKeyall += encryptKeyall;
                    else
                        break;
                }
            }
            string encryptKey = encryptKeyall.Substring(0, 8);
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象   
            byte[] key = Encoding.UTF8.GetBytes(encryptKey); //定义字节数组，用来存储密钥    
            byte[] data = Encoding.UTF8.GetBytes(str);//定义字节数组，用来存储要加密的字符串  
            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      
            //使用内存流实例化加密流对象   
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateEncryptor(key, key), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);  //向加密流中写入数据      
            CStream.FlushFinalBlock();              //释放加密流      
            return Convert.ToBase64String(MStream.ToArray());//返回加密后的字符串  
        }

        /// <summary>  
        /// DES解密操作3   
        /// </summary>  
        /// <param name="str">要解密的字符串</param>  
        ///  <param name="myKey">秘钥</param>  
        /// <returns>解密后的字符串</returns>  
        public static string desDecrypt(string str, string myKey)
        {
            string encryptKeyall = Convert.ToString(myKey);    //定义密钥  
            if (encryptKeyall.Length < 9)
            {
                for (; ; )
                {
                    if (encryptKeyall.Length < 9)
                        encryptKeyall += encryptKeyall;
                    else
                        break;
                }
            }
            string encryptKey = encryptKeyall.Substring(0, 8);
            DESCryptoServiceProvider descsp = new DESCryptoServiceProvider();   //实例化加/解密类对象    
            byte[] key = Encoding.UTF8.GetBytes(encryptKey); //定义字节数组，用来存储密钥    
            byte[] data = Convert.FromBase64String(str);//定义字节数组，用来存储要解密的字符串  
            MemoryStream MStream = new MemoryStream(); //实例化内存流对象      
            //使用内存流实例化解密流对象       
            CryptoStream CStream = new CryptoStream(MStream, descsp.CreateDecryptor(key, key), CryptoStreamMode.Write);
            CStream.Write(data, 0, data.Length);      //向解密流中写入数据     
            CStream.FlushFinalBlock();               //释放解密流      
            return Encoding.UTF8.GetString(MStream.ToArray());       //返回解密后的字符串  
        }

    }
}
