using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteHelp_Until
{
    public class CreateDB
    {
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbname">数据库名称</param>
        public void CreateDb(string dbname)
        {
            SqliteHelp sqliteHelp = new SqliteHelp(DBConStr.sqlcon);
            sqliteHelp.CreateDB(dbname);
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="sql">sql语句<para>create table webkey (序号 INTEGER primary key,密钥 nvarchar(50),类型 nvarchar,兑换账号 nvarchar,兑换时间 datatime)</para></param>
        public void CreteTb(string sql)
        {
            SqliteHelp sqliteHelp = new SqliteHelp(DBConStr.sqlcon);
            //string sql = "create table webkey (序号 INTEGER primary key,密钥 nvarchar(50),类型 nvarchar,兑换账号 nvarchar,兑换时间 datatime)";
            sqliteHelp.SqliteDBADU(sql);
        }

        /// <summary>
        /// 数据库中是否存在表<para>不存在返回0，存在返回1</para>
        /// </summary>
        /// <param name="tbname"></param>
        /// <returns>数据库表不存在返回0，存在返回1</returns>
        public int TableIsNo(string tbname)
        {
            string sql = $"SELECT COUNT(*) FROM sqlite_master where type='table' and name='{tbname}'";
            SqliteHelp sqliteHelp = new SqliteHelp(DBConStr.sqlcon);
            using (SQLiteDataReader sdr = sqliteHelp.SelcetDBDR(sql))
            {
                int num = 0;
                if (sdr.HasRows)
                {
                    while (sdr.Read())
                    {
                        object obj = sdr.GetValue(0);
                        num = int.Parse(obj.ToString());
                    }
                }
                sdr.Close();
                return num;
            }    
        }

        /// <summary>
        /// 添加表字段
        /// </summary>
        /// <param name="dbname">数据库名称</param>
        /// <param name="tbname">表名</param>
        /// <param name="filedname">字段名称</param>
        /// <param name="filetype">字段类型<para>如INTEGER、nvarchar、datatime</para></param>
        public void AddFiled(string dbname,string tbname,string filedname)
        {
            SqliteHelp sqliteHelp = new SqliteHelp(DBConStr.sqlcon);
            //添加字段
            string sql = "select * from " + dbname;
            SQLiteDataReader sdr = sqliteHelp.SelcetDBDR(sql);
            bool addfield = true;
            for (int i = 0; i < sdr.FieldCount; i++)
            {
                if (sdr.GetName(i).Equals(filedname))
                {
                    addfield = false;
                    break;
                }
            }
            if (addfield)
            {
                sql = $"alter table {tbname} add {filedname} nvarchar";
                sqliteHelp.SqliteDBADU(sql);
            }
            //else
            //{
            //    //string a = "已有";
            //}
        }

        /// <summary>
        /// 保存表行数据 ，自动判断是新增还是更新
        /// </summary>
        /// <param name="tbname">数据库表名称</param>
        /// <param name="rowstrs">数据库表行集合（不包含自增长唯一表行字段数据）</param>
        /// <param name="filedname">where后字段名</param>
        /// <param name="filedvalue">where后字段值</param>
        /// <param name="isnoUP">是否更新数据库<para>true 更新 ,false 不更新</para></param>
        public void SaveData(string tbname, List<string> rowstrs, string filedname, string filedvalue,bool isnoUP)
        {
            //this.DelTbRow(tbname, filedname, filedvalue);
            string sql = string.Empty;
            SqliteHelp sqliteDBHelp = null;
            if (string.IsNullOrEmpty(filedvalue))
            {
                sqliteDBHelp = new SqliteHelp(DBConStr.sqlcon);
                sql = CreateAddSql(tbname, rowstrs);
                sqliteDBHelp.SqliteDBADU(sql);
            }
            else
            {
                sqliteDBHelp = new SqliteHelp(DBConStr.sqlcon);
                string selectsql = $"select * from {tbname} where {filedname}='{filedvalue}'";
                SQLiteDataReader sQLiteDataReader = sqliteDBHelp.SelcetDBDR(selectsql);

                if (sQLiteDataReader.HasRows)
                {
                    if (isnoUP)
                    {
                        //sql = "update mskey set 类型=" + "'" + keyinfo[1] + "', " + "有效性=" + "'" + keyinfo[2] + "', " + "检测时间=" + "'" + keyinfo[3] + "' " + "where 密钥=" + "'" + keyinfo[0] + "'";
                        sql = CreateUPSql(tbname, rowstrs, filedname, filedvalue);
                    }
                }
                else
                {
                    sql = CreateAddSql(tbname, rowstrs);
                }
                sQLiteDataReader.Close();

                sqliteDBHelp = new SqliteHelp(DBConStr.sqlcon); ;
                sqliteDBHelp.SqliteDBADU(sql);
            }

        }

        /// <summary>
        /// 更新表行数据
        /// </summary>
        /// <param name="tbname">数据库表名称</param>
        /// <param name="rowstrs">数据库表行集合</param>
        /// <param name="filedname">where后字段名</param>
        /// <param name="filedvalue">where后字段值</param>
        public void UPTbRow(string tbname, List<string> rowstrs, string filedname, string filedvalue)
        {
            string sql = CreateUPSql(tbname, rowstrs, filedname, filedvalue);
            SqliteHelp sqliteDBHelp = new SqliteHelp(DBConStr.sqlcon); ;
            sqliteDBHelp.SqliteDBADU(sql);
        }

        /// <summary>
        /// 生成更新sql语句
        /// </summary>
        /// <param name="tbname">数据库表名称</param>
        /// <param name="rowstrs">表行数据集合</param>
        /// <param name="filedname">where后字段名</param>
        /// <param name="filedvalue">where后字段值</param>
        /// <returns></returns>
        private string CreateUPSql(string tbname, List<string> rowstrs, string filedname, string filedvalue)
        {
            string filedname1 = GetFiledName(tbname);
            string[] fn = filedname1.Split(',');

            string temp = string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fn.Length; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append($"{fn[i]}='{rowstrs[i]}'");
                if (i==fn.Length)
                {
                    break;
                }
            }

            string sql = $"update {tbname} set " + sb.ToString() + $" where {filedname}='{filedvalue}'";
            return sql;
        }



        /// <summary>
        /// 添加表行数据
        /// </summary>
        /// <param name="tbname">数据库表名称</param>
        /// <param name="rowstrs">数据库表行集合</param>
        public void AddTbRow(string tbname, List<string> rowstrs) 
        {
            string sql = CreateAddSql(tbname, rowstrs);
            SqliteHelp sqliteDBHelp = new SqliteHelp(DBConStr.sqlcon); ;
            sqliteDBHelp.SqliteDBADU(sql);
        }

        /// <summary>
        /// 生成添加sql语句
        /// </summary>
        /// <param name="tbname">数据库表名称</param>
        /// <param name="rowstrs">数据库表行集合</param>
        /// <returns></returns>
        private string CreateAddSql(string tbname, List<string> rowstrs)
        {
            string filedstr = GetFiledName(tbname);
            string[] fn = filedstr.Split(',');
            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (string item in rowstrs)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append($"'{item}'");
                i++;
                if (i== fn.Length)
                {
                    break;
                }
            }
            string sql = $"insert into {tbname} ({filedstr}) values ({sb.ToString()})";
            return sql;
        }

        /// <summary>
        /// 获取表字段 辅助生成sql语句
        /// </summary>
        /// <param name="tbname">表名称</param>
        /// <returns>表字段以,间隔</returns>
        private string GetFiledName(string tbname)
        {
            SqliteHelp sqliteDBHelp = new SqliteHelp(DBConStr.sqlcon);
            //string sql = $"select * from {tbname}";
            string sql = $"select * from {tbname} LIMIT 1";
            SQLiteDataReader sQLiteDataReader = sqliteDBHelp.SelcetDBDR(sql);
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < sQLiteDataReader.FieldCount; i++)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append(sQLiteDataReader.GetName(i));
            }
            sQLiteDataReader.Close();
            return sb.ToString();
        }

        /// <summary>
        /// 删除表行数据
        /// </summary>
        /// <param name="tbname">数据库表名</param>
        /// <param name="filed">where后字段名</param>
        /// <param name="filedvalue">where后字段值</param>
        public void DelTbRow(string tbname, string filed, string filedvalue)
        {
            string sql = $"delete from {tbname} where {filed}='{filedvalue}'";
            SqliteHelp sqliteDBHelp = new SqliteHelp(DBConStr.sqlcon);
            sqliteDBHelp.SqliteDBADU(sql);
        }

    }
}
