using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace GZDBMSSQL
{
    /// <summary>
    /// 数据库操作接口
    /// </summary>
    public interface IDatabase
    {
        int CommandTimeout { get; set; }
        
        ///// <summary>
        ///// 这些方法用于创建提供程序对数据源类的实现的实例
        ///// </summary>
        //DbProviderFactory ProviderFactory { get; } 

        #region SQL语句
        /// <summary>
        /// 执行SQL语句，并返回受影响行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        int ExecuteNonQuery(string sql, SqlParameterProvider parameters);

        /// <summary>
        /// 执行SQL语句，并返回指定对象集合
        /// </summary>
        /// <typeparam name="T">要返回的对象类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="action">转换委托</param>
        /// <returns></returns>
        List<T> ExecuteDataReader<T>(string sql, SqlParameterProvider parameters, Func<DbDataReader, T> action);

        /// <summary>
        /// 执行SQL语句，委托处理结果
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="action">处理委托</param>
        void ExecuteDataReader(string sql, SqlParameterProvider parameters, Action<DbDataReader> action);

        /// <summary>
        /// 执行SQL语句，返回第一行第一列
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        T ExecuteScalar<T>(string sql, SqlParameterProvider parameters);
        /// <summary>
        /// 执行SQL语句，返回DataTable结构
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="TableName">表名</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        DataTable GetTable(string sql, string TableName, SqlParameterProvider parameters);
        /// <summary>
        /// 执行SQL语句，返回DataSet结构
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        DataSet GetDataSet(string sql, SqlParameterProvider parameters);
 

        #endregion


        #region 存储过程
        /// <summary>
        ///  执行存储过程，并返回受影响行数
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        int ExecuteNonQuerySP(string StoredProcedureName, SqlParameterProvider parameters);
        /// <summary>
        /// 执行存储过程，并返回指定对象集合
        /// </summary>
        /// <typeparam name="T">要返回的对象类型</typeparam>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="action">转换委托</param>
        /// <returns></returns>
        List<T> ExecuteDataReaderSP<T>(string StoredProcedureName, SqlParameterProvider parameters, Func<DbDataReader, T> action);
        /// <summary>
        /// 执行存储过程，委托处理结果
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters"></param>
        /// <param name="action"></param>
        void ExecuteDataReaderSP(string StoredProcedureName, SqlParameterProvider parameters, Action<DbDataReader> action);

        /// <summary>
        /// 执行存储过程，返回第一行第一列
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        T ExecuteScalarSP<T>(string StoredProcedureName, SqlParameterProvider parameters);
        /// <summary>
        /// 执行存储过程，返回DataTable结构
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="TableName">表名</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        DataTable GetTableSP(string StoredProcedureName, string TableName, SqlParameterProvider parameters);
        /// <summary>
        /// 执行存储过程，返回DataSet结构
        /// </summary>
        /// <param name="StoredProcedureName">存储过程名称</param>
        /// <param name="parameters">查询参数</param>
        /// <returns></returns>
        DataSet GetDataSetSP(string StoredProcedureName, SqlParameterProvider parameters);
   
        #endregion

        /// <summary>
        /// 在事物内执行
        /// </summary>
        /// <param name="action"></param>
        void ExecuteTransaction(Action<IDatabase> action);
    }
}