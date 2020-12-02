using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using Young.Core.Common;

namespace Young.Core.Data
{
    /// <summary>
    /// 说明：这是一个针对System.Data.SQLite的数据库常规操作封装的通用类。
    /// Version:0.1
    /// </summary>
    public class SQLiteDBHelper
    {
        private string connectionString = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbPath">SQLite数据库文件路径</param>
        public SQLiteDBHelper(string dbPath)
        {
            this.connectionString = "Data Source=" + dbPath + ";Pooling=true;FailIfMissing=false";
        }

        #region 创建 数据库

        /// <summary>
        /// 创建SQLite数据库文件
        /// </summary>
        /// <param name="dbPath">要创建的SQLite数据库文件路径</param>
        public static int _CreateDB(string dbPath)
        {
            int res = 0;
            //try
            //{
            SQLiteConnection.CreateFile(dbPath);  // 创建数据文件
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + dbPath + ";Pooling=true;FailIfMissing=false"))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = _CreateTableSQL();
                    res = command.ExecuteNonQuery();
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("SQLiteDBHelper.CreateDB()", "message:" + ex.Message + "\r\nsource:" + ex.Source + "\r\nStackTrace:" + ex.StackTrace);
            //}
            return res;
        }

        #endregion

        #region (public) 对SQLite数据库执行增删改操作，返回受影响的行数。ExecuteNonQuery

        /// <summary>
        /// 对SQLite数据库执行增删改操作，返回受影响的行数。
        /// </summary>
        /// <param name="sqlLinkString">SQLiteConnection 数据库连接字符串</param>
        /// <param name="sql">要执行的增删改的SQL语句</param>
        /// <param name="parameters">执行增删改语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sqlLinkString, string sql, SQLiteParameter[] parameters)
        {
            int affectedRows = 0;

            //try
            //{
            using (SQLiteConnection connection = new SQLiteConnection(sqlLinkString))
            {
                connection.Open();
                using (DbTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = sql;

                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        affectedRows = command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("SQLiteDBHelper.ExecuteNonQuery()", "message:" + ex.Message + "\r\nsource:" + ex.Source + "\r\nStackTrace:" + ex.StackTrace);
            //}

            return affectedRows;
        }

        #endregion

        #region (public) 执行一个查询语句，返回一个包含查询结果的DataTable ExecuteDataTable

        /// <summary>
        /// 执行一个查询语句，返回一个包含查询结果的DataTable
        /// </summary>
        /// <param name="sqlLinkString">SQLiteConnection 数据库连接字符串</param>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string sqlLinkString, string sql, SQLiteParameter[] parameters)
        {
            DataTable data = new DataTable();
            //try
            //{
            using (SQLiteConnection connection = new SQLiteConnection(sqlLinkString))
            {
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, connection))
                {
                    if (parameters != null)
                    {
                        adapter.SelectCommand.Parameters.AddRange(parameters);
                    }
                    adapter.Fill(data);
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("SQLiteDBHelper.ExecuteDataTable()", "message:" + ex.Message + "\r\nsource:" + ex.Source + "\r\nStackTrace:" + ex.StackTrace);
            //}
            return data;
        }

        #endregion

        /// <summary>
        /// 执行一个查询语句，返回查询结果的第一行第一列
        /// </summary>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param>
        /// <returns></returns>
        public Object ExecuteScalar(string sql, SQLiteParameter[] parameters)
        {
            DataTable data = new DataTable();
            //try
            //{
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);

                    adapter.Fill(data);
                    adapter.Dispose();
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("SQLiteDBHelper.ExecuteScalar()", "message:" + ex.Message + "\r\nsource:" + ex.Source + "\r\nStackTrace:" + ex.StackTrace);
            //}
            return data;
        }

        /// <summary>
        /// 执行一个查询语句，返回一个关联的SQLiteDataReader实例
        /// </summary>
        /// <param name="sqlLinkString">SQLiteConnection 数据库连接字符串</param>
        /// <param name="sql">要执行的查询语句</param>
        /// <param name="parameters">执行SQL查询语句所需要的参数，参数必须以它们在SQL语句中的顺序为准</param>
        /// <returns></returns>
        public static SQLiteDataReader ExecuteReader(string sqlLinkString, string sql, SQLiteParameter[] parameters)
        {
            SQLiteConnection connection = new SQLiteConnection(sqlLinkString);
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            connection.Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// 查询数据库中的所有数据类型信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetSchema()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                DataTable data = connection.GetSchema("TABLES");
                connection.Close();
                return data;
            }
        }

        /// <summary>
        /// 建表语句
        /// </summary>
        /// <returns></returns>
        private static string _CreateTableSQL()
        {
            return @"

CREATE TABLE IF NOT EXISTS 检测信息 (
    检测编号 VARCHAR(30) PRIMARY KEY not null, 
    委托单位 VARCHAR(200),
    单位地址 VARCHAR(500), 
    单位电话 VARCHAR(30), 
    样品名称 VARCHAR(200),
    规格型号 VARCHAR(200), 
    商标 VARCHAR(200),
    生产单位 VARCHAR(500), 
    送样日期 VARCHAR(50),
    送样地点 VARCHAR(200),
    工程名称 VARCHAR(200),
    检验项目 VARCHAR(200),
    检验数量 VARCHAR(10),
    检验地点 VARCHAR(200),
    检验日期 VARCHAR(50),
    检验依据 VARCHAR(500),
    检验设备 VARCHAR(500)
);

CREATE TABLE IF NOT EXISTS ComputeResult(
    [id] integer PRIMARY KEY autoincrement,
    [invalue] varchar(50),
    [outvalue] varchar(50),
    [hotq] varchar(50),
    [hotr] varchar(50),
    [hotk] varchar(50),
    [starttime] varchar(50),
    [endtime] varchar(50),
    [name] varchar(200),
    [testinfocode] varchar(30),
    [createtime] varchar(50),
    [context] text
);

CREATE TABLE IF NOT EXISTS Version(

    [key] varchar(50),
    [exptime] varchar(100),
    [ischeck] varchar(10)

);

CREATE TABLE IF NOT EXISTS Admin(
    [username] varchar(50),
    [password] varchar(50)
);

insert into Admin(username,password) values('Administrator','123456')

";

        }

    }
}