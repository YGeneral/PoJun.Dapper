using Dapper;
using Npgsql;
using PoJun.Dapper.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper.Repository.PostgreSQL
{
    /// <summary>
    /// PostgreSQL基础仓储【待测试】
    /// 创建人：杨江军
    /// 创建时间：2021/1/7/星期四 16:30:44
    /// </summary>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region 初始化

        /// <summary>
        /// 数据库链接字符串
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// 参数前缀
        /// </summary>
        private string _prefix { get { return "@"; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库链接字符串</param>
        public BaseRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        #endregion

        #region 纯SQL执行

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
            sql = $"{sql};set @id= RETURNING id;";
            var newParam = new DynamicParameters();
            newParam.AddDynamicParams(param);
            newParam.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
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
        public async Task<int> InsertAsync(string sql, object param = null, int? commandTimeout = null)
        {
            sql = $"DELIMITER;{sql};set @id= LAST_INSERT_ID();DELIMITER;";
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var result = await conn.ExecuteScalarAsync<int>(sql, param, commandTimeout: commandTimeout);
                return result;
            }
        }

        #endregion

        #region 执行增（INSERT）删（DELETE）改（UPDATE）语句或存储过程【单条SQL语句，可以获得输出参数】【同步】

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句或存储过程【单条SQL语句，可以获得输出参数】【同步】
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="outParam">输出参数[参数名(@name)] </param>
        /// <param name="commandType">如何解释命令字符串</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>Tuple(受影响的行数, Dictionary(输出参数名(@name), 输出参数))</returns>
        public Tuple<int, Dictionary<string, string>> Execute(string sql, DynamicParameters param, List<string> outParam, CommandType? commandType = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
            {
                var rows = conn.Execute(sql, param, commandTimeout: commandTimeout, commandType: commandType);
                var outDic = new Dictionary<string, string>();
                foreach (var item in outParam)
                {
                    outDic.Add(item, param.Get<string>(item));
                }
                return new Tuple<int, Dictionary<string, string>>(rows, outDic);
            }
        }

        #endregion

        #region 执行增（INSERT）删（DELETE）改（UPDATE）语句或存储过程【单条SQL语句，可以获得输出参数】【异步】

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句或存储过程【单条SQL语句，可以获得输出参数】【异步】
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="outParam">输出参数[参数名(@name)] </param>
        /// <param name="commandType">如何解释命令字符串</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>Tuple(受影响的行数, Dictionary(输出参数名(@name), 输出参数))</returns>
        public async Task<Tuple<int, Dictionary<string, string>>> ExecuteAsync(string sql, DynamicParameters param, List<string> outParam, CommandType? commandType = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var rows = await conn.ExecuteAsync(sql, param, commandTimeout: commandTimeout, commandType: commandType);
                var outDic = new Dictionary<string, string>();
                foreach (var item in outParam)
                {
                    outDic.Add(item, param.Get<string>(item));
                }
                return new Tuple<int, Dictionary<string, string>>(rows, outDic);
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
        public int Execute(string sql, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
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
        public async Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var result = await conn.ExecuteAsync(sql, param, commandTimeout: commandTimeout);
                return result;
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
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
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

        #region 执行增（INSERT）删（DELETE）改（UPDATE）语句【带事务，可以同时执行多条SQL】【异步】

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【带事务，可以同时执行多条SQL】【异步】
        /// </summary>
        /// <param name="sqlDic">SQL + 参数【key：sql语句 value：参数】</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public async Task ExecuteToTransactionAsync(Dictionary<string, object> sqlDic, int? commandTimeout = null)
        {
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
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
        /// 未实现方法【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public Tuple<IEnumerable<T>, int> ExecuteToPaginationProcdeure<T>(DynamicParameters param = null, int? commandTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 执行分页存储过程（Procdeure）【异步】

        /// <summary>
        /// 未实现方法【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public Task<Tuple<IEnumerable<T>, int>> ExecuteToPaginationProcdeureAsync<T>(DynamicParameters param = null, int? commandTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 执行存储过程（Procdeure）【同步】

        /// <summary>
        /// 未实现方法【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="porcdeureName">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteToProcdeure<T>(string porcdeureName, object param = null, int? commandTimeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 执行存储过程（Procdeure）【异步】

        /// <summary>
        /// 未实现方法【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="porcdeureName">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> ExecuteToProcdeureAsync<T>(string porcdeureName, object param = null, int? commandTimeout = null)
        {
            throw new NotImplementedException();
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
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
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
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var result = await conn.QueryAsync<T>(sql, param: param, commandTimeout: commandTimeout);
                return result;
            }

        }

        #endregion

        #region 分页查询【同步】

        /// <summary>
        /// 分页查询【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="sql">查询数据的SQL(请在select 后面加上 SQL_CALC_FOUND_ROWS 关键字)</param>
        /// <param name="totalSql">查询数据数量的SQL(默认值：select FOUND_ROWS() AS Count)</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>Data：查询出的数据；Count：当前查询条件下数据的数量</returns>
        public (IEnumerable<T> Data, long Count) QueryPageList<T>(string sql, string totalSql = "select FOUND_ROWS() AS Count", object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                //去除第一句SQL结尾处的分号
                sql = RemoveEnd(sql.Trim(), ";");
                //生成新的SQL
                sql = $"{sql};{totalSql.Trim()}";

                //执行查询
                using (var multi = conn.QueryMultiple(sql, param, commandTimeout: commandTimeout))
                {
                    var data = multi.Read<T>();
                    var count = (multi.Read<long>()).SingleOrDefault();
                    return (data, count);
                }
            }
        }

        #endregion

        #region 分页查询【异步】

        /// <summary>
        /// 分页查询【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="sql">查询数据的SQL(请在select 后面加上 SQL_CALC_FOUND_ROWS 关键字)</param>
        /// <param name="totalSql">查询数据数量的SQL(默认值：select FOUND_ROWS() AS Count)</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>Data：查询出的数据；Count：当前查询条件下数据的数量</returns>
        public async Task<(IEnumerable<T> Data, long Count)> QueryPageListAsync<T>(string sql, string totalSql = "select FOUND_ROWS() AS Count", object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new NpgsqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                //去除第一句SQL结尾处的分号
                sql = RemoveEnd(sql.Trim(), ";");
                //生成新的SQL
                sql = $"{sql};{totalSql.Trim()}";

                //执行查询
                using (var multi = await conn.QueryMultipleAsync(sql, param, commandTimeout: commandTimeout))
                {
                    var data = await multi.ReadAsync<T>();
                    var count = (await multi.ReadAsync<long>()).SingleOrDefault();
                    return (data, count);
                }
            }
        }

        #endregion

        #endregion

        #region 工具

        #region RemoveEnd(移除末尾字符串)

        /// <summary>
        /// 移除末尾字符串
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="removeValue">要移除的值</param>
        private static string RemoveEnd(string value, string removeValue)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            if (string.IsNullOrWhiteSpace(removeValue))
                return SafeString(value);
            if (value.ToLower().EndsWith(removeValue.ToLower()))
                return value.Remove(value.Length - removeValue.Length, removeValue.Length);
            return value;
        }

        #endregion

        #region 安全转换为字符串，去除两端空格，当值为null时返回""

        /// <summary>
        /// 安全转换为字符串，去除两端空格，当值为null时返回""
        /// </summary>
        /// <param name="input">输入值</param>
        private static string SafeString(object input)
        {
            return input == null ? string.Empty : input.ToString().Trim();
        }

        #endregion

        #endregion

        #region Linq执行

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <returns></returns>
        public IBaseRepository<T> initLinq()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string BuildInsert(Expression expression = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="allColumn"></param>
        /// <returns></returns>
        public string BuildUpdate(bool allColumn = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string BuildUpdate(Expression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <returns></returns>
        public string BuildDelete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <returns></returns>
        public string BuildSelect()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <returns></returns>
        public string BuildCount()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <returns></returns>
        public string BuildExists()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <returns></returns>
        public string BuildSum()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column"></param>
        /// <param name="subquery"></param>
        /// <returns></returns>
        public IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, ISubQuery subquery)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="column"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> GroupBy(string expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> Where(string expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> Where(Expression<Func<T, bool?>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public IBaseRepository<T> OrderBy(string orderBy)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IBaseRepository<T> Skip(int index, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IBaseRepository<T> Take(int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public IBaseRepository<T> Page(int index, int count, out long total)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> Having(string expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IBaseRepository<T> Having(Expression<Func<T, bool?>> expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <returns></returns>
        public IBaseRepository<T> Filter<TResult>(Expression<Func<T, TResult>> columns)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="lockType"></param>
        /// <returns></returns>
        public IBaseRepository<T> With(string lockType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="lockType"></param>
        /// <returns></returns>
        public IBaseRepository<T> With(LockType lockType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <returns></returns>
        public IBaseRepository<T> Distinct()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="buffered"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public T Single(string columns = null, bool buffered = true, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<T> SingleAsync(string columns = null, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="buffered"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="buffered"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public TResult Single<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="colums"></param>
        /// <param name="buffered"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="colums"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<IEnumerable<T>> SelectAsync(string colums = null, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="colums"></param>
        /// <param name="buffered"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IEnumerable<TResult> Select<TResult>(string colums = null, bool buffered = true, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="colums"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(string colums = null, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="buffered"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="columns"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int Insert(T entity, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int Insert(Expression<Func<T, T>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(Expression<Func<T, T>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public long InsertReturnId(Expression<Func<T, T>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<long> InsertReturnIdAsync(Expression<Func<T, T>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(T entity, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public long InsertReturnId(T entity, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<long> InsertReturnIdAsync(T entity, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int Insert(IEnumerable<T> entitys, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(IEnumerable<T> entitys, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int Update(int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int Update(T entity, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int Update(Expression<Func<T, T>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(Expression<Func<T, T>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(T entity, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int Update(IEnumerable<T> entitys, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="entitys"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(IEnumerable<T> entitys, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public int Delete(int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Exists(int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<bool> ExistsAsync(int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public long Count(string columns = null, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<long> CountAsync(string columns = null, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public long Count<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<long> CountAsync<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 未实现方法
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
