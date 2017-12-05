using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper.Repository.SqlServer
{
    /// <summary>
    /// SqlServer基础仓储
    /// </summary>
    public class BaseRepository
    {
        #region 初始化

        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库链接字符串</param>
        public BaseRepository(string connectionString)
        {
            ConnectionString = connectionString;
        } 

        #endregion

        #region 执行增（INSERT）语句【执行单条SQL语句，返回自增列】【同步】

        /// <summary>
        /// 执行增（INSERT）语句【执行单条SQL语句，返回自增列】【同步】
        /// </summary>
        /// <param name="sql">SQL语句（本方法会自动在SQL的结尾增加分号，并增加查询ID的语句）</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>自增ID</returns>
        public int Insert(string sql, object param = null, int? commandTimeout = null)
        {
            sql = sql + ";select @id= SCOPE_IDENTITY();";
            var newParam = new DynamicParameters();
            newParam.AddDynamicParams(param);
            newParam.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Execute(sql, newParam, commandTimeout: commandTimeout);
                var id = newParam.Get<int>("@id");
                return id;
            }
        }

        #endregion

        #region 执行增（INSERT）语句【执行单条SQL语句，返回自增列】【异步】

        /// <summary>
        /// 执行增（INSERT）语句【执行单条SQL语句，返回自增列】【异步】
        /// </summary>
        /// <param name="sql">SQL语句（本方法会自动在SQL的结尾增加分号，并增加查询ID的语句）</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>自增ID</returns>
        public Task<int> InsertAsync(string sql, object param = null, int? commandTimeout = null)
        {
            sql = sql + ";select @id= SCOPE_IDENTITY();";
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                return conn.ExecuteScalarAsync<int>(sql, param, commandTimeout: commandTimeout);
            }
        }

        #endregion

        #region 执行增（INSERT）删（DELETE）改（UPDATE）语句【执行单条SQL语句】【同步】

        /// <returns></returns>
        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【执行单条SQL语句】【同步】
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>受影响的行数</returns>
        public int Execute(string sql,  object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                return conn.Execute(sql, param, commandTimeout: commandTimeout);
            }
        }

        #endregion

        #region 执行增（INSERT）删（DELETE）改（UPDATE）语句【执行单条SQL语句】【异步】

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【执行单条SQL语句】【异步】
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>受影响的行数</returns>
        public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                return conn.ExecuteAsync(sql, param, commandTimeout: commandTimeout);
            }
        }

        #endregion

        #region 执行增（INSERT）删（DELETE）改（UPDATE）语句【带事务，可以同时执行多条SQL】【同步】

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【带事务，可以同时执行多条SQL】【同步】
        /// </summary>
        /// <param name="sqlDic">SQL + 参数【key：sql语句 value：参数】</param>ConnectionString
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public void ExecuteToTransaction(Dictionary<string, object> sqlDic, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    foreach (var item in sqlDic)
                    {
                        conn.Execute(item.Key, item.Value, transaction: trans, commandTimeout: commandTimeout);
                    }
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region 执行增（INSERT）删（DELETE）改（UPDATE）语句【带事务，可以同时执行多条SQL】【异步】【待测试】

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【带事务，可以同时执行多条SQL】【异步】【待测试】
        /// </summary>
        /// <param name="sqlDic">SQL + 参数【key：sql语句 value：参数】</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public async Task ExecuteToTransactionAsync(Dictionary<string, object> sqlDic, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    foreach (var item in sqlDic)
                    {
                        await conn.ExecuteAsync(item.Key, item.Value, transaction: trans, commandTimeout: commandTimeout);
                    }
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        #endregion

        #region 执行分页存储过程（Procdeure）【同步】

        /// <summary>
        /// 执行分页存储过程（Procdeure）【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public Tuple<IEnumerable<T>, int> ExecuteToPaginationProcdeure<T>(DynamicParameters param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                var dataList = conn.Query<T>("proc_sql_Paging", param: param, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
                var count = param.Get<int>("@recordCount");
                return new Tuple<IEnumerable<T>, int>(dataList, count);
            }
        }

        #endregion

        #region 执行分页存储过程（Procdeure）【异步】

        /// <summary>
        /// 执行分页存储过程（Procdeure）【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public async Task<Tuple<IEnumerable<T>, int>> ExecuteToPaginationProcdeureAsync<T>( DynamicParameters param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                var dataList = await conn.QueryAsync<T>("proc_sql_Paging", param: param, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
                var count = param.Get<int>("@recordCount");
                return new Tuple<IEnumerable<T>, int>(dataList, count);
            }
        }

        #endregion

        #region 执行存储过程（Procdeure）【同步】

        /// <summary>
        /// 执行存储过程（Procdeure）【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="porcdeureName">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteToProcdeure<T>(string porcdeureName, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                return conn.Query<T>(porcdeureName, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure, param: param);
            }
        }

        #endregion

        #region 执行存储过程（Procdeure）【异步】

        /// <summary>
        /// 执行存储过程（Procdeure）【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="porcdeureName">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> ExecuteToProcdeureAsync<T>(string porcdeureName, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Query("");
                return conn.QueryAsync<T>(porcdeureName, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure, param: param);
            }
        }

        #endregion

        #region 执行查询【同步】

        /// <summary>
        /// 执行查询【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                return conn.Query<T>(sql, param: param, commandTimeout: commandTimeout);
            }
        }

        #endregion

        #region 执行查询【异步】

        /// <summary>
        /// 执行查询【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new SqlConnection(ConnectionString))
            {
                return conn.QueryAsync<T>(sql, param: param, commandTimeout: commandTimeout);
            }
        }

        #endregion
    }
}
