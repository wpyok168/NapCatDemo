using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteHelp_Until
{
    public static class DBConStr
    {
        //数据库名称
        public static string dbname = System.Environment.CurrentDirectory + "\\winisoaddr.db";
        public static string pwd = "CQSetWPY168";
        //数据库连接字符串
        public static string sqlcon = "Data Source=" + dbname + "; Version=3;";
        //带密码的数据库连接字符串
        public static string sqlconpwd = "Data Source=" + dbname + "; Version=3; Password=" + pwd;
    }
}
