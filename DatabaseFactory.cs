using System;
using System.Data.Common;
namespace GZDBMSSQL
{



    /// <summary>
    /// 数据库操作对象生成工厂
    /// </summary>
    public class DatabaseFactory
    {
        /// <summary>
        /// 创建数据库操作对象
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        /// <param name="CustomerDbDataAdapter"></param>
        /// <returns></returns>
        public static IDatabase CreateDatabase(string connectionString)
        {
            return new ConnectionDatabase(connectionString);
        }
        

    }
}