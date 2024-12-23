using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace SqliteHelp_Until
{
    public class SqliteHelp
    {
        private SQLiteConnection sqlitecon;
        //SELECT COUNT(*) FROM sqlite_master where type='table' and name='表名';  判断表是否存在 返回1存在，返回0不存在     
        /// <summary>
        /// 构造函数 无密码
        /// </summary>
        /// <param name="sqlcon"></param>
        public SqliteHelp(string sqlcon)
        {
            sqlitecon = new SQLiteConnection(sqlcon);
        }
        //public SqliteHelp(string dbname)
        //{
        //    sqlitecon = new SQLiteConnection();
        //    SQLiteConnectionStringBuilder scsb = new SQLiteConnectionStringBuilder();
        //    scsb.DataSource = dbname;
        //    sqlitecon.ConnectionString = scsb.ToString();
        //}

        /// <summary>
        /// 构造函数 带密码
        /// </summary>
        /// <param name="sqlcon"></param>
        /// <param name="pwd"></param>
        public SqliteHelp(string dbname, string pwd)
        {
            sqlitecon = new SQLiteConnection();
            SQLiteConnectionStringBuilder scsb = new SQLiteConnectionStringBuilder();
            scsb.DataSource = dbname;
            scsb.Password = pwd;
            sqlitecon.ConnectionString = scsb.ToString();
        }

        //创建数据库
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbname"></param>
        public void CreateDB(string dbname)
        {
            SQLiteConnection.CreateFile(dbname);
        }

        ///// <summary>
        ///// 创建数据库密码
        ///// </summary>
        ///// <param name="pwd"></param>
        //public void DBSetPWD(string pwd)
        //{
        //    try
        //    {
        //        if (sqlitecon.State == ConnectionState.Closed)
        //        {
        //            sqlitecon.Open();
        //            sqlitecon.SetPassword(pwd);
        //            sqlitecon.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        sqlitecon.Close();
        //    }
        //}

        ///// <summary>
        ///// 修改数据库密码
        ///// </summary>
        ///// <param name="newpwd"></param>
        //public void DBChangePWD(string newpwd)
        //{
        //    try
        //    {
        //        if (sqlitecon.State == ConnectionState.Closed)
        //        {
        //            sqlitecon.Open();
        //            sqlitecon.ChangePassword(newpwd);
        //            sqlitecon.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        sqlitecon.Close();
        //    }
        //}

        //增删改
        /// <summary>
        /// 增删改
        /// </summary>
        /// <param name="sql"></param>
        public void SqliteDBADU(String sql)
        {
            try
            {
                object obj = new object();
                lock (obj)
                {
                    if (sqlitecon.State == ConnectionState.Closed)
                    {
                        sqlitecon.Open();
                    }
                    using (SQLiteCommand sqlcom = new SQLiteCommand(sql, sqlitecon))
                    {
                        sqlcom.ExecuteNonQueryAsync();
                    }
                    sqlitecon.Close();
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                sqlitecon.Close();
            }
        }

        /// <summary>
        /// 查询返回SQLiteDataAdapter
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SQLiteDataAdapter SelectDBDAP(string sql)
        {
            try
            {
                object obj = new object();
                lock (obj)
                {
                    if (sqlitecon.State == ConnectionState.Closed)
                    {
                        sqlitecon.Open();
                    }
                    SQLiteDataAdapter sQLiteDataAdapter = new SQLiteDataAdapter(sql, sqlitecon);
                    return sQLiteDataAdapter;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlitecon.Close();
            }
        }

        /// <summary>
        /// 查询返回SQLiteDataReader <para>！！！注意外部调用使用完后需将SQLiteDataReader关闭掉</para>
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public SQLiteDataReader SelcetDBDR(string sql)
        {
            try
            {
                object obj = new object();
                lock (obj)
                {
                    if (sqlitecon.State == ConnectionState.Closed)
                    {
                        sqlitecon.Open();
                    }
                    SQLiteCommand sQLiteCommand = new SQLiteCommand(sql, sqlitecon);
                    SQLiteDataReader sQLiteDataReader = sQLiteCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    return sQLiteDataReader;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 异步查询返回Task<SQLiteDataReader>
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<SQLiteDataReader> SelcetDBDRAsync(string sql)
        {
            try
            {
                object obj = new object();

                if (sqlitecon.State == ConnectionState.Closed)
                {
                    sqlitecon.Open();
                }
                using (SQLiteCommand sQLiteCommand = new SQLiteCommand(sql, sqlitecon))
                {
                    SQLiteDataReader drAsync = (SQLiteDataReader)await sQLiteCommand.ExecuteReaderAsync();
                    return drAsync;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlitecon.Close();
            }

        }
    }
}
