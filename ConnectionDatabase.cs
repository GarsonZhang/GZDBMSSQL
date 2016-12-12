using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;

namespace GZDBMSSQL
{
    /// <summary>
    /// 数据库操作对象
    /// </summary>
    public class ConnectionDatabase : IDatabase
    {
        private readonly string _ConnectionString;

        private int _commandtimeout = 30;
        public int CommandTimeout
        {
            get { return _commandtimeout; }
            set { _commandtimeout = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">指定提供程序名称的 System.Data.Common.DbProviderFactory 的一个实例</param>
        /// /// <param name="cAdapter"></param>
        public ConnectionDatabase(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        SqlTransaction trans = null;
        public ConnectionDatabase(SqlTransaction tran)
        {
            trans = tran;
        }

        /// <summary>
        /// 创建数据库连接并打开
        /// </summary>
        /// <returns></returns>
        private SqlConnection CreateConnection()
        {
            if (trans == null)
            {
                SqlConnection connection = new SqlConnection(_ConnectionString);
                connection.Open();
                return connection;
            }
            else
            {
                return trans.Connection;
            }
        }


        private SqlCommand PrepareCommand(SqlConnection conn, string sql, SqlParameterProvider parameters, CommandType CommandType)
        {
            SqlCommand Command;
            if (trans == null)
                Command = new SqlCommand(sql, conn);
            else
                Command = new SqlCommand(sql, conn, trans);
            Command.CommandType = CommandType;
            Command.CommandTimeout = CommandTimeout;
            if (parameters != null)
                SetParameters(Command, parameters);
            return Command;
        }

        private void ClearCommandParams(SqlCommand Command)
        {
            Command.Parameters.Clear();
        }

        private void SetParameters(SqlCommand Command, SqlParameterProvider parameters)
        {
            if (parameters == null) return;
            var parms = parameters.GetParms();
            var pv = parameters.GetParmArrary();

            if (parms != null && parms.Length > 0)
                Command.Parameters.AddRange(parms);

            if (pv != null)
            {
                foreach (string key in pv.Keys)
                {
                    var cp = Command.CreateParameter();
                    cp.ParameterName = key;
                    cp.Value = pv[key];
                    Command.Parameters.Add(cp);
                }
            }
        }

        #region SQL语句
        /// <summary>
        /// 执行SQL语句，并返回受影响行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, SqlParameterProvider parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, sql, parameters, CommandType.Text))
                {
                    int query = Command.ExecuteNonQuery();
                    ClearCommandParams(Command);
                    return query;
                }
            }
        }

        /// <summary>
        ///  执行存储过程，并返回受影响行数
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public int ExecuteNonQuerySP(string StoredProcedureName, SqlParameterProvider parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, StoredProcedureName, parameters, CommandType.StoredProcedure))
                {
                    int query = Command.ExecuteNonQuery();
                    ClearCommandParams(Command);
                    return query;
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，并返回指定对象集合
        /// </summary>
        /// <typeparam name="T">要返回的对象类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="action">转换委托</param>
        /// <returns></returns>
        public List<T> ExecuteDataReader<T>(string sql, SqlParameterProvider parameters, Func<DbDataReader, T> action)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, sql, parameters, CommandType.Text))
                {

                    List<T> lst = new List<T>();
                    using (var dr = Command.ExecuteReader())
                    {
                        while (dr.Read())
                            lst.Add(action.Invoke(dr));
                    }
                    ClearCommandParams(Command);
                    return lst;
                }
            }
        }

        /// <summary>
        /// 执行存储过程，并返回指定对象集合
        /// </summary>
        /// <typeparam name="T">要返回的对象类型</typeparam>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="action">转换委托</param>
        /// <returns></returns>
        public List<T> ExecuteDataReaderSP<T>(string StoredProcedureName, SqlParameterProvider parameters, Func<DbDataReader, T> action)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, StoredProcedureName, parameters, CommandType.StoredProcedure))
                {

                    List<T> lst = new List<T>();
                    using (var dr = Command.ExecuteReader())
                    {
                        while (dr.Read())
                            lst.Add(action.Invoke(dr));
                    }
                    ClearCommandParams(Command);
                    return lst;
                }
            }
        }


        /// <summary>
        /// 执行SQL语句，委托处理结果
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="action">处理委托</param>
        public void ExecuteDataReader(string sql, SqlParameterProvider parameters, Action<DbDataReader> action)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, sql, parameters, CommandType.Text))
                {
                    using (var dr = Command.ExecuteReader())
                    {
                        while (dr.Read())
                            action.Invoke(dr);
                    }
                    ClearCommandParams(Command);
                }
            }
        }

        /// <summary>
        /// 执行存储过程，委托处理结果
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters"></param>
        /// <param name="action"></param>
        public void ExecuteDataReaderSP(string StoredProcedureName, SqlParameterProvider parameters, Action<DbDataReader> action)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, StoredProcedureName, parameters, CommandType.StoredProcedure))
                {
                    using (var dr = Command.ExecuteReader())
                    {
                        while (dr.Read())
                            action.Invoke(dr);
                    }
                    ClearCommandParams(Command);
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回DataTable结构
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="TableName">表名</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public DataTable GetTable(string sql, string TableName, SqlParameterProvider parameters = null)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, sql, parameters, CommandType.Text))
                {
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                    DataTable dt = new DataTable(TableName);
                    DataAdapter.Fill(dt);
                    ClearCommandParams(Command);
                    return dt;
                }
            }
        }
        /// <summary>
        /// 执行存储过程，返回DataTable结构
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="TableName">表名</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public DataTable GetTableSP(string StoredProcedureName, string TableName, SqlParameterProvider parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, StoredProcedureName, parameters, CommandType.StoredProcedure))
                {
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                    DataTable dt = new DataTable(TableName);
                    DataAdapter.Fill(dt);
                    ClearCommandParams(Command);
                    return dt;
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回DataSet结构
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, SqlParameterProvider parameters = null)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, sql, parameters, CommandType.Text))
                {
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                    DataSet ds = new DataSet();
                    DataAdapter.Fill(ds);
                    ClearCommandParams(Command);
                    return ds;
                }
            }
        }

        /// <summary>
        /// 执行存储过程，返回DataSet结构
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public DataSet GetDataSetSP(string StoredProcedureName, SqlParameterProvider parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, StoredProcedureName, parameters, CommandType.StoredProcedure))
                {
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                    DataSet ds = new DataSet();
                    DataAdapter.Fill(ds);
                    ClearCommandParams(Command);
                    return ds;
                }
            }
        }

        /// <summary>
        /// 执行SQL语句，返回第一行第一列
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, SqlParameterProvider parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, sql, parameters, CommandType.Text))
                {
                    var v = Command.ExecuteScalar();
                    ClearCommandParams(Command);
                    if (v == DBNull.Value || v == null)
                        return default(T);
                    else
                        return (T)v;
                }
            }

        }

        /// <summary>
        /// 执行存储过程，返回第一行第一列
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        public T ExecuteScalarSP<T>(string StoredProcedureName, SqlParameterProvider parameters)
        {
            using (var connection = CreateConnection())
            {
                using (var Command = PrepareCommand(connection, StoredProcedureName, parameters, CommandType.StoredProcedure))
                {
                    var v = Command.ExecuteScalar();
                    ClearCommandParams(Command);
                    if (v == DBNull.Value || v == null)
                        return default(T);
                    else
                        return (T)v;
                }
            }

        }


        #endregion SQL语句


        /// <summary>
        /// 在事物内执行
        /// </summary>
        /// <param name="action"></param>
        public void ExecuteTransaction(Action<IDatabase> action)
        {
            using (var connection = CreateConnection())
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var db = new ConnectionDatabase(transaction);
                        if (action != null)
                            action.Invoke(this);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 重载方法，返回链接字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _ConnectionString;
        }


    }
}