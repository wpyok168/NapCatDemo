using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WinDownLoad.MyDB
{
    public class CreateCQDb
    {
        public void createDB()
        {
            //string path = common.CQApi.AppDirectory + $"{SqliteHelp_Until.DBConStr.dbname}";
            if (!File.Exists(SqliteHelp_Until.DBConStr.dbname))
            {
                this.DBInitialization();
            }

        }
        private void DBInitialization()
        {
            //创建数据库和表
            SqliteHelp_Until.CreateDB CDb = new SqliteHelp_Until.CreateDB();
            //string path = common.CQApi.AppDirectory + $"{SqliteHelp_Until.DBConStr.dbname}";
            CDb.CreateDb(SqliteHelp_Until.DBConStr.dbname);
            //密钥数据库
            string sql = "create table isoaddr (序号 INTEGER primary key,密钥 nvarchar(50),版本信息 nvarchar,下载地址 nvarchar,下载时间 INTEGER)";
            //string sql = "create table mskey (序号 INTEGER primary key,密钥 nvarchar(50),版本信息 nvarchar,错误代码 nvarchar,检测时间 datatime,激活次数 datatime)";
            CDb.CreteTb(sql);
        }
    }
}
