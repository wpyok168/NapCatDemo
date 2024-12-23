using System;
using System.Text;
using System.Management;
using System.Security.Cryptography;

namespace SqliteHelp_Until
{
    /// <summary>
    /// 注册码
    /// </summary>
    public class RegisterClass
    {
        //步骤一: 获得CUP序列号和硬盘序列号的实现代码如下:
        //获得CPU的序列号

        bool Stupids = true;
        bool Cat = false;
        private string getCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");
            ManagementObjectCollection myCpuConnection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuConnection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
                break;
            }
            return strCpu;
        }

        //取得设备硬盘的卷标号
        private string GetDiskVolumeSerialNumber()
        {
            ManagementClass mc =
                 new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObject disk =
                 new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        ///   <summary> 
        ///   获取网卡硬件地址 
        ///   </summary> 
        ///   <returns> string </returns> 
        private string GetMoAddress()
        {
            string MoAddress = "";
            try
            {
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                {
                    ManagementObjectCollection moc2 = mc.GetInstances();
                    foreach (ManagementObject mo in moc2)
                    {
                        if ((bool)mo["IPEnabled"] == true)
                            MoAddress = mo["MacAddress"].ToString();
                        mo.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return MoAddress.ToString();
        }


        //步骤二: 收集硬件信息生成机器码, 代码如下: 
        //生成机器码
        /// <summary>
        /// 生成机器码1
        /// </summary>
        /// <returns></returns>
        public string CreateCode()
        {
            string temp = getCpu() + GetDiskVolumeSerialNumber();//获得24位Cpu和硬盘序列号
            string[] strid = new string[24];//
            for (int i = 0; i < 24; i++)//把字符赋给数组
            {
                strid[i] = temp.Substring(i, 1);
            }
            temp = "";
            //Random rdid = new Random();
            for (int i = 0; i < 24; i++)//从数组随机抽取24个字符组成新的字符生成机器三
            {
                //temp += strid[rdid.Next(0, 24)];
                temp += strid[i + 3 >= 24 ? 0 : i + 3];
            }
            return GetMd5(temp);
        }

        /// <summary>
        /// 生成机器码2
        /// </summary>
        /// <returns></returns>
        public string CreateCode1()
        {
            string temp = GetMoAddress() + GetDiskVolumeSerialNumber();//获得24位网卡和硬盘序列号
            string[] strid = new string[24];//
            for (int i = 0; i < 24; i++)//把字符赋给数组
            {
                strid[i] = temp.Substring(i, 1);
            }
            temp = "";
            //Random rdid = new Random();
            for (int i = 0; i < 24; i++)//从数组随机抽取24个字符组成新的字符生成机器三
            {
                //temp += strid[rdid.Next(0, 24)];
                temp += strid[i + 3 >= 24 ? 0 : i + 3];
            }
            return GetMd5(temp);
        }

        //步骤三: 使用机器码生成软件注册码, 代码如下:
        //使用机器码生成注册码
        private int[] intCode = new int[127];//用于存密钥

        private void setIntCode()//给数组赋值个小于10的随机数
        {
            //Random ra = new Random();
            //for (int i = 1; i < intCode.Length;i++ )
            //{
            //    intCode[i] = ra.Next(0, 9);
            //}
            for (int i = 1; i < intCode.Length; i++)
            {
                intCode[i] = i + 3 > 9 ? 0 : i + 3;
            }
        }
        private int[] intNumber = new int[25];//用于存机器码的Ascii值
        private char[] Charcode = new char[25];//存储机器码字

        //生成注册码
        /// <summary>
        /// 生成注册码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetCode(string code)
        {
            if (code != "")
            {
                //把机器码存入数组中
                setIntCode();//初始化127位数组
                for (int i = 1; i < Charcode.Length; i++)//把机器码存入数组中
                {
                    Charcode[i] = Convert.ToChar(code.Substring(i - 1, 1));
                }//
                for (int j = 1; j < intNumber.Length; j++)//把字符的ASCII值存入一个整数组中。
                {
                    intNumber[j] =
                       intCode[Convert.ToInt32(Charcode[j])] +
                       Convert.ToInt32(Charcode[j]);
                }
                string strAsciiName = null;//用于存储机器码
                for (int j = 1; j < intNumber.Length; j++)
                {
                    //MessageBox.Show((Convert.ToChar(intNumber[j])).ToString());
                    //判断字符ASCII值是否0－9之间

                    if (intNumber[j] >= 48 && intNumber[j] <= 57)
                    {
                        strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                    }
                    //判断字符ASCII值是否A－Z之间

                    else if (intNumber[j] >= 65 && intNumber[j] <= 90)
                    {
                        strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                    }
                    //判断字符ASCII值是否a－z之间


                    else if (intNumber[j] >= 97 && intNumber[j] <= 122)
                    {
                        strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                    }
                    else//判断字符ASCII值不在以上范围内
                    {
                        if (intNumber[j] > 122)//判断字符ASCII值是否大于z
                        {
                            strAsciiName += Convert.ToChar(intNumber[j] - 10).ToString();
                        }
                        else
                        {
                            strAsciiName += Convert.ToChar(intNumber[j] - 9).ToString();
                        }

                    }
                    //label3.Text = strAsciiName;//得到注册码
                }
                return strAsciiName;
            }
            else
            {
                return "";
            }
        }


        //步骤四: 用户输入注册码注册软件, 演示代码如下:

        //注册
        /// <summary>
        /// 注册软件
        /// </summary>
        /// <param name="jqm">机器码</param>
        /// <param name="currentCode">注册码--由内部程序生成的注册码</param>
        /// <param name="realCode">用户输入的注册码和currentCode注册码比较</param>
        /// <param name="appname"></param>
        /// <returns></returns>
        public bool RegistIt(string jqm, string currentCode, string realCode, string appname)
        {
            if (realCode != "")
            {
                if (currentCode.TrimEnd().Equals(realCode.TrimEnd()))
                {
                    Microsoft.Win32.RegistryKey retkey =
                         Microsoft.Win32.Registry.CurrentUser.
                         OpenSubKey("software", true).CreateSubKey($"{appname}").
                         CreateSubKey($"{appname}.ini").
                         CreateSubKey(jqm.TrimEnd());
                    //CreateSubKey(currentCode.TrimEnd());
                    //retkey.SetValue("StupidsCat", "BBC6D58D0953F027760A046D58D52786");
                    retkey.SetValue($"{appname}", realCode);

                    retkey = Microsoft.Win32.Registry.LocalMachine.
                        OpenSubKey("software", true).CreateSubKey($"{appname}").
                         CreateSubKey($"{appname}.ini").
                         CreateSubKey(jqm.TrimEnd());
                    //CreateSubKey(currentCode.TrimEnd());
                    //retkey.SetValue("StupidsCat", "BBC6D58D0953F027760A046D58D52786");
                    retkey.SetValue($"{appname}", realCode);

                    return Stupids;
                }
                else
                {
                    return Cat;
                }
            }
            else { return Cat; }
        }

        /// <summary>
        /// 验证软件是否注册
        /// </summary>
        /// <param name="jqm">机器码</param>
        /// <param name="sn">注册码</param>
        /// <param name="appname"></param>
        /// <returns></returns>
        //public bool BoolRegist(string sn)
        public bool BoolRegist(string jqm, string sn, string appname)
        {
            string[] keynames; bool flag = false;
            Microsoft.Win32.RegistryKey localRegKey = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey userRegKey = Microsoft.Win32.Registry.CurrentUser;
            try
            {
                //keynames = localRegKey.OpenSubKey("software\\StupidsCat\\StupidsCat.ini\\" + GetMd5(sn)).GetValueNames();
                keynames = localRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\" + jqm).GetValueNames();
                foreach (string name in keynames)
                {
                    if (name == appname)
                    {
                        if (localRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\" + jqm).GetValue(appname).ToString() == sn)
                            //if (localRegKey.OpenSubKey("software\\StupidsCat\\StupidsCat.ini\\" + GetMd5(sn)).GetValue("StupidsCat").ToString() == "BBC6D58D0953F027760A046D58D52786")
                            flag = true;
                    }
                }
                //keynames = userRegKey.OpenSubKey("software\\StupidsCat\\StupidsCat.ini\\" + GetMd5(sn)).GetValueNames();
                keynames = userRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\" + jqm).GetValueNames();
                foreach (string name in keynames)
                {
                    if (name == appname)
                    {
                        //if (flag && userRegKey.OpenSubKey("software\\StupidsCat\\StupidsCat.ini\\" + GetMd5(sn)).GetValue("StupidsCat").ToString() == "BBC6D58D0953F027760A046D58D52786")
                        if (flag && userRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\" + jqm).GetValue(appname).ToString() == sn)
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                localRegKey.Close();
                userRegKey.Close();
            }
        }

        private string GetMd5(object text)
        {
            string path = text.ToString();

            MD5CryptoServiceProvider MD5Pro = new MD5CryptoServiceProvider();
            Byte[] buffer = Encoding.GetEncoding("utf-8").GetBytes(text.ToString());
            Byte[] byteResult = MD5Pro.ComputeHash(buffer);

            string md5result = BitConverter.ToString(byteResult).Replace("-", "");
            return md5result;
        }

        /// <summary>
        /// 记录软件首次使用日期
        /// </summary>
        /// <param name="appname"></param>
        /// <returns>返回false即禁用软件</returns>
        public bool TryOut(string appname)
        {
            string[] keynames; bool flag = false; bool flag1 = false;
            Microsoft.Win32.RegistryKey localRegKey = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey userRegKey = Microsoft.Win32.Registry.CurrentUser;
            try
            {
                keynames = localRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValueNames();
                foreach (string name in keynames)
                {
                    if (name == "UserTime")
                    {
                        DateTime appdt = DateTime.Parse(localRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValue("UserTime").ToString());
                        if (appdt != null)
                        {
                            flag = true;
                            //return true;
                        }

                    }
                }
            }
            catch
            {
                flag = false;
            }
            finally
            {
                localRegKey.Close();
            }
            try
            {
                keynames = userRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValueNames();
                foreach (string name in keynames)
                {
                    if (name == "UserTime")
                    {
                        DateTime appdt = DateTime.Parse(userRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValue("UserTime").ToString());
                        if (appdt != null)
                        {
                            flag1 = true;
                            //return true;
                        }
                    }
                }
                //return false;
            }
            catch
            {
                flag1 = false;
            }

            if (flag == false && flag1 == false)
            {
                return SetUserTime(appname);
            }
            else
            {
                if (flag && flag1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 记录软件首次使用时间
        /// </summary>
        /// <param name="appname"></param>
        /// <returns></returns>
        private bool SetUserTime(string appname)
        {
            try
            {
                Microsoft.Win32.RegistryKey retkey =
                                     Microsoft.Win32.Registry.CurrentUser.
                                     OpenSubKey("software", true).CreateSubKey($"{appname}").
                                     CreateSubKey($"{appname}.ini").
                                     CreateSubKey("UserTime");
                retkey.SetValue("UserTime", DateTime.Now.ToUniversalTime());

                retkey = Microsoft.Win32.Registry.LocalMachine.
                    OpenSubKey("software", true).CreateSubKey($"{appname}").
                     CreateSubKey($"{appname}.ini").
                     CreateSubKey("UserTime");
                retkey.SetValue("UserTime", DateTime.Now.ToUniversalTime());

                //记录软件启动日期
                retkey =
                                     Microsoft.Win32.Registry.CurrentUser.
                                     OpenSubKey("software", true).CreateSubKey($"{appname}").
                                     CreateSubKey($"{appname}.ini").
                                     CreateSubKey("UserTime");
                retkey.SetValue("NowDay", DateTime.Now.ToUniversalTime());

                retkey = Microsoft.Win32.Registry.LocalMachine.
                    OpenSubKey("software", true).CreateSubKey($"{appname}").
                     CreateSubKey($"{appname}.ini").
                     CreateSubKey("UserTime");
                retkey.SetValue("NowDay", DateTime.Now.ToUniversalTime());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 记录软件启动使用日期
        /// </summary>
        /// <param name="appname"></param>
        /// <returns></returns>
        private bool UPNowDay(string appname)
        {
            try
            {
                Microsoft.Win32.RegistryKey retkey =
                                         Microsoft.Win32.Registry.CurrentUser.
                                         OpenSubKey("software", true).CreateSubKey($"{appname}").
                                         CreateSubKey($"{appname}.ini").
                                         CreateSubKey("UserTime");
                retkey.SetValue("NowDay", DateTime.Now.ToUniversalTime());

                retkey = Microsoft.Win32.Registry.LocalMachine.
                    OpenSubKey("software", true).CreateSubKey($"{appname}").
                     CreateSubKey($"{appname}.ini").
                     CreateSubKey("UserTime");
                retkey.SetValue("NowDay", DateTime.Now.ToUniversalTime());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        int days = 5;
        /// <summary>
        /// 验证软件只用10天
        /// </summary>
        /// <param name="appname"></param>
        /// <param name="syday">返回剩余天数</param>
        /// <returns>返回false禁用软件</returns>
        public bool GetUserTimes(string appname, ref int syday,ref DateTime NowDay)
        {
            string[] keynames; bool flag = false; bool flag1 = false;
            Microsoft.Win32.RegistryKey localRegKey = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey userRegKey = Microsoft.Win32.Registry.CurrentUser;
            try
            {
                keynames = localRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValueNames();
                foreach (string name in keynames)
                {
                    if (name == "UserTime")
                    {
                        DateTime appdt = DateTime.Parse(localRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValue("UserTime").ToString());
                        TimeSpan timeSpan = DateTime.Now - appdt;
                        int days = timeSpan.Days;
                        if (days > this.days)
                        {
                            return false;
                        }
                        int temp = Convert.ToDateTime(DateTime.Now.ToString("yy/MM/dd")).CompareTo(Convert.ToDateTime(appdt.ToString("yy/MM/dd")));
                        if (temp >= 0 && days < this.days && timeSpan.TotalMilliseconds > 0)
                        {
                            flag = true;
                            syday = this.days - days;
                        }
                    }
                    if (name == "NowDay")
                    {
                        flag1 = true;
                        NowDay = DateTime.Parse(localRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValue("NowDay").ToString());
                    }
                }
                keynames = userRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValueNames();
                foreach (string name in keynames)
                {
                    if (name == "UserTime")
                    {
                        DateTime appdt = DateTime.Parse(userRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValue("UserTime").ToString());
                        TimeSpan timeSpan = DateTime.Now - appdt;
                        int days = timeSpan.Days;
                        if (days > this.days)
                        {
                            return false;
                        }
                        int temp = Convert.ToDateTime(DateTime.Now.ToString("yy/MM/dd")).CompareTo(Convert.ToDateTime(appdt.ToString("yy/MM/dd")));
                        if (flag && temp >= 0 && days < this.days && timeSpan.TotalMilliseconds > 0)
                        {
                            int syday1 = this.days - days;
                            syday = Math.Min(syday1, syday);
                            return true;
                        }
                        
                    }
                    if (name == "NowDay")
                    {
                        if (flag1 == true)
                        {
                            DateTime NowDay1 = DateTime.Parse(localRegKey.OpenSubKey($"software\\{appname}\\{appname}.ini\\UserTime").GetValue("NowDay").ToString());
                            if (NowDay.ToLongDateString()!= NowDay1.ToLongDateString())
                            {
                                return false;
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                localRegKey.Close();
                userRegKey.Close();
            }

        }
    }
}
