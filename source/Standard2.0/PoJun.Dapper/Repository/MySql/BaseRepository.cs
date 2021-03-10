using Dapper;
using MySql.Data.MySqlClient;
using PoJun.Dapper.IRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper.Repository.MySql
{
    /// <summary>
    /// MySql基础仓储
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
            sql = $"{sql};select LAST_INSERT_ID();";
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return (conn.Query<int>(sql, param, commandTimeout: commandTimeout, commandType: CommandType.Text)).FirstOrDefault();
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
            sql = $"{sql};select LAST_INSERT_ID();";
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                return (await conn.QueryAsync<int>(sql, param, commandTimeout: commandTimeout, commandType: CommandType.Text)).FirstOrDefault();
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
        /// 执行分页存储过程（Procdeure）【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        public Tuple<IEnumerable<T>, int> ExecuteToPaginationProcdeure<T>(DynamicParameters param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var dataList = conn.Query<T>("proc_sql_Paging", param: param, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
                var count = param.Get<int>("@_recordCount");
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
        public async Task<Tuple<IEnumerable<T>, int>> ExecuteToPaginationProcdeureAsync<T>(DynamicParameters param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var dataList = await conn.QueryAsync<T>("proc_sql_Paging", param: param, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure);
                var count = param.Get<int>("@_recordCount");
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
        public async Task<IEnumerable<T>> ExecuteToProcdeureAsync<T>(string porcdeureName, object param = null, int? commandTimeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                var result = await conn.QueryAsync<T>(porcdeureName, commandTimeout: commandTimeout, commandType: CommandType.StoredProcedure, param: param);
                return result;
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
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


        #region implement
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> With(string lockType)
        {
            _lock.Append(lockType);
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> With(LockType lockType)
        {
            if (lockType == LockType.FOR_UPADTE)
            {
                With("FOR UPDATE");
            }
            else if (lockType == LockType.LOCK_IN_SHARE_MODE)
            {
                With("LOCK IN SHARE MODE");
            }
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Distinct()
        {
            _distinctBuffer.Append("DISTINCT");
            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Filter<TResult>(Expression<Func<T, TResult>> columns)
        {
            _filters.AddRange(ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => s.Value));

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> GroupBy(string expression)
        {
            if (_groupBuffer.Length > 0)
            {
                _groupBuffer.Append(",");
            }
            _groupBuffer.Append(expression);

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression)
        {
            GroupBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)));

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Having(string expression)
        {
            _havingBuffer.Append(expression);

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Having(Expression<Func<T, bool?>> expression)
        {
            Having(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)));

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> OrderBy(string orderBy)
        {
            if (_orderBuffer.Length > 0)
            {
                _orderBuffer.Append(",");
            }
            _orderBuffer.Append(orderBy);

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression)
        {
            OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => string.Format("{0} ASC", s.Value))));

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression)
        {
            OrderBy(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => string.Format("{0} DESC", s.Value))));

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Page(int index, int count, out long total)
        {
            total = 0;

            Skip(count * (index - 1), count);
            total = Count();

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, ISubQuery subquery)
        {
            if (_setBuffer.Length > 0)
            {
                _setBuffer.Append(",");
            }
            var columns = ExpressionUtil.BuildColumn(column, _param, _prefix).First();
            _setBuffer.AppendFormat("{0} = {1}", columns.Value, subquery.Build(_param, _prefix));

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value)
        {
            if (_setBuffer.Length > 0)
            {
                _setBuffer.Append(",");
            }
            var columns = ExpressionUtil.BuildColumn(column, _param, _prefix).First();
            if (_param != null)
            {
                var key = string.Format("{0}{1}", columns.Key, _param.Count);
                _param.Add(key, value);
            }

            switch (columns.Value)
            {
                case "System.String":
                    _setBuffer.AppendFormat("{0} = '{1}'", columns.Key, value);
                    break;
                case "System.DateTime":
                    _setBuffer.AppendFormat("{0} = '{1}'", columns.Key, (Convert.ToDateTime(value)).ToString("yyyy/MM/dd HH:mm:ss"));
                    break;
                case "System.Enum":
                    _setBuffer.AppendFormat("{0} = {1}", columns.Key, ExpressionUtil.GetEnumValue((column as LambdaExpression).Body.Type, value.ToString()));
                    break;
                default:
                    if (columns.Value.IndexOf("System.Nullable") >= 0 && columns.Value.IndexOf("System.DateTime") >= 0)
                        _setBuffer.AppendFormat("{0} = '{1}'", columns.Key, (Convert.ToDateTime(value)).ToString("yyyy/MM/dd HH:mm:ss"));
                    else if (columns.Value.IndexOf("System.Nullable") >= 0)
                        _setBuffer.AppendFormat("{0} = '{1}'", columns.Key, value);
                    else
                        _setBuffer.AppendFormat("{0} = {1}", columns.Key, value);
                    break;
            }

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> value)
        {
            if (_setBuffer.Length > 0)
            {
                _setBuffer.Append(",");
            }
            var columnName = ExpressionUtil.BuildColumn(column, _param, _prefix).First().Value;
            var expression = ExpressionUtil.BuildExpression(value, _param, _prefix);
            _setBuffer.AppendFormat("{0} = {1}", columnName, expression);

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Skip(int index, int count)
        {
            pageIndex = index;
            pageCount = count;

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Take(int count)
        {
            Skip(0, count);

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Where(string expression)
        {
            if (_whereBuffer.Length > 0)
            {
                _whereBuffer.AppendFormat(" {0} ", Operator.GetOperator(ExpressionType.AndAlso));
            }
            _whereBuffer.Append(expression);

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public IBaseRepository<T> Where(Expression<Func<T, bool?>> expression)
        {
            Where(ExpressionUtil.BuildExpression(expression, _param, _prefix));

            return this;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Delete(int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildDelete();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Execute(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<int> DeleteAsync(int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildDelete();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteAsync(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Insert(Expression<Func<T, T>> expression, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert(expression);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Execute(sql, _param, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// 新增并返回自增ID
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public long InsertReturnId(Expression<Func<T, T>> expression, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert(expression);
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.ExecuteScalar<long>(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<int> InsertAsync(Expression<Func<T, T>> expression, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert(expression);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteAsync(sql, _param, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// 新增并返回自增ID
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<long> InsertReturnIdAsync(Expression<Func<T, T>> expression, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert(expression);
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteScalarAsync<long>(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Insert(T entity, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Execute(sql, entity, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<int> InsertAsync(T entity, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteAsync(sql, entity, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// 新增并返回自增ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public long InsertReturnId(T entity, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.ExecuteScalar<long>(sql, entity, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// 新增并返回自增ID
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public async Task<long> InsertReturnIdAsync(T entity, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert();
                sql = string.Format("{0};SELECT @@IDENTITY;", sql);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteScalarAsync<long>(sql, entity, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Insert(IEnumerable<T> entitys, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Execute(sql, entitys, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<int> InsertAsync(IEnumerable<T> entitys, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildInsert();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteAsync(sql, entitys, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Update(int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                if (_setBuffer.Length < 1)
                    return 0;
                var sql = BuildUpdate(false);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Execute(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<int> UpdateAsync(int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                if (_setBuffer.Length < 1)
                    return 0;
                var sql = BuildUpdate(false);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteAsync(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Update(T entity, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildUpdate();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Execute(sql, entity, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Update(Expression<Func<T, T>> expression, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildUpdate(expression);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Execute(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<int> UpdateAsync(Expression<Func<T, T>> expression, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildUpdate(expression);
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteAsync(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<int> UpdateAsync(T entity, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildUpdate();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteAsync(sql, entity, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Update(IEnumerable<T> entitys, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildUpdate();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Execute(sql, entitys, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<int> UpdateAsync(IEnumerable<T> entitys, int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildUpdate();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteAsync(sql, entitys, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public T Single(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select(columns, buffered, timeout).SingleOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<T> SingleAsync(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync(columns, timeout)).SingleOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        public TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            Take(1);
            return Select<TResult>(columns, buffered, timeout).SingleOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null)
        {
            Take(1);
            return (await SelectAsync<TResult>(columns, timeout)).SingleOrDefault();
        }
        /// <summary>
        /// 
        /// </summary>
        public TResult Single<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            var columnstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Single<TResult>(columnstr, buffered, timeout);
        }
        /// <summary>
        /// 
        /// </summary>
        public Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            var columnstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return SingleAsync<TResult>(columnstr, timeout);
        }


        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {

                var sql = BuildSelect();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Query<T>(sql, _param, buffered: buffered, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<IEnumerable<T>> SelectAsync(string colums = null, int? timeout = null)
        {
            if (colums != null)
            {
                _columnBuffer.Append(colums);
            }
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildSelect();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.QueryAsync<T>(sql, _param, commandTimeout: timeout);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TResult> Select<TResult>(string columns = null, bool buffered = true, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildSelect();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.Query<TResult>(sql, _param, buffered: buffered, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<IEnumerable<TResult>> SelectAsync<TResult>(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildSelect();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.QueryAsync<TResult>(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return Select<TResult>(columstr, buffered, timeout);
        }
        /// <summary>
        /// 
        /// </summary>
        public Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null)
        {
            var columstr = string.Join(",",
                ExpressionUtil.BuildColumns(columns, _param, _prefix).Select(s => string.Format("{0} AS {1}", s.Value, s.Key)));
            return SelectAsync<TResult>(columstr, timeout);
        }
        /// <summary>
        /// 
        /// </summary>
        public long Count(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildCount();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.ExecuteScalar<long>(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<long> CountAsync(string columns = null, int? timeout = null)
        {
            if (columns != null)
            {
                _columnBuffer.Append(columns);
            }
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildCount();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteScalarAsync<long>(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public long Count<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null)
        {
            return Count(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)), timeout);
        }
        /// <summary>
        /// 
        /// </summary>
        public Task<long> CountAsync<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null)
        {
            return CountAsync(string.Join(",", ExpressionUtil.BuildColumns(expression, _param, _prefix).Select(s => s.Value)), timeout);
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Exists(int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildExists();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.ExecuteScalar<int>(sql, _param, commandTimeout: timeout) > 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> ExistsAsync(int? timeout = null)
        {
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildExists();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteScalarAsync<int>(sql, _param, commandTimeout: timeout) > 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public TResult Sum<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null)
        {
            var column = ExpressionUtil.BuildExpression(expression, _param, _prefix);
            _sumBuffer.AppendFormat("{0}", column);
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildSum();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return conn.ExecuteScalar<TResult>(sql, _param, commandTimeout: timeout);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public async Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null)
        {
            var column = ExpressionUtil.BuildExpression(expression, _param, _prefix);
            _sumBuffer.AppendFormat("{0}", column);
            using (IDbConnection conn = new MySqlConnection(ConnectionString))
            {
                var sql = BuildSum();
                System.Diagnostics.Trace.WriteLine($"Dapper生成的SQL语句-{DateTime.Now:yyyy-MM-dd HH:mm:ss}：【{sql}】");
                return await conn.ExecuteScalarAsync<TResult>(sql, _param, commandTimeout: timeout);
            }
        }
        #endregion

        #region property

        /// <summary>
        /// 初始化Linq查询的变量
        /// </summary>
        /// <returns></returns>
        public IBaseRepository<T> initLinq()
        {
            _columnBuffer = new StringBuilder();
            _filters = new List<string>();
            _setBuffer = new StringBuilder();
            _havingBuffer = new StringBuilder();
            _whereBuffer = new StringBuilder();
            _groupBuffer = new StringBuilder();
            _orderBuffer = new StringBuilder();
            _distinctBuffer = new StringBuilder();
            _countBuffer = new StringBuilder();
            _sumBuffer = new StringBuilder();
            _lock = new StringBuilder();
            _table = EntityUtil.GetTable<T>();
            pageIndex = null;
            pageCount = null;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> _param { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _columnBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public List<string> _filters = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _setBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _havingBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _whereBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _groupBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _orderBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _distinctBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _countBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _sumBuffer = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public StringBuilder _lock = new StringBuilder();
        /// <summary>
        /// 
        /// </summary>
        public EntityTable _table = EntityUtil.GetTable<T>();
        /// <summary>
        /// 
        /// </summary>
        public int? pageIndex = null;
        /// <summary>
        /// 
        /// </summary>
        public int? pageCount = null;
        #endregion

        #region build
        /// <summary>
        /// 
        /// </summary>
        public string BuildInsert(Expression expression = null)
        {
            if (expression == null)
            {
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})"
                    , _table.TableName
                    , string.Join(",", _table.Columns.FindAll(f => f.Identity == false && !_filters.Exists(e => e == f.ColumnName)).Select(s => s.ColumnName))
                    , string.Join(",", _table.Columns.FindAll(f => f.Identity == false && !_filters.Exists(e => e == f.ColumnName)).Select(s => string.Format("@{0}", s.CSharpName))));
                return sql;
            }
            else
            {
                var columns = ExpressionUtil.BuildColumnAndValues(expression, _param, _prefix);
                var sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})"
                    , _table.TableName
                    , string.Join(",", columns.Keys)
                    , string.Join(",", columns.Values));
                return sql;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BuildUpdate(bool allColumn = true)
        {
            if (allColumn)
            {
                var keyColumn = _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary);
                var columns = _table.Columns.FindAll(f => f.ColumnKey != ColumnKey.Primary && !_filters.Exists(e => e == f.ColumnName));
                var sql = string.Format("UPDATE {0} SET {1} WHERE {2}"
                    , _table.TableName
                    , string.Join(",", columns.Select(s => string.Format("{0} = @{1}", s.ColumnName, s.CSharpName)))
                    , _whereBuffer.Length > 0 ? _whereBuffer.ToString() : string.Format("{0} = @{1}", keyColumn.ColumnName, keyColumn.CSharpName)
                    );
                return sql;
            }
            else
            {
                var sql = string.Format("UPDATE {0} SET {1}{2}"
                    , _table.TableName
                    , _setBuffer
                    , _whereBuffer.Length > 0 ? string.Format(" WHERE {0}", _whereBuffer) : "");
                return sql;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public string BuildUpdate(Expression expression)
        {
            var keyColumn = _table.Columns.Find(f => f.ColumnKey == ColumnKey.Primary);
            var columns = ExpressionUtil.BuildColumnAndValues(expression, _param, _prefix).Where(a => a.Key != keyColumn.ColumnName);
            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                    _table.TableName,
                    string.Join(",", columns.Select(s => string.Format("{0} = {1}", s.Key, s.Value))),
                    _whereBuffer.Length > 0 ? _whereBuffer.ToString() : string.Format("{0} = @{1}", keyColumn.ColumnName, keyColumn.CSharpName)
                    );
            return sql;
        }
        /// <summary>
        /// 
        /// </summary>
        public string BuildDelete()
        {
            var sql = string.Format("DELETE FROM {0}{1}",
                _table.TableName,
                _whereBuffer.Length > 0 ? string.Format(" WHERE {0}", _whereBuffer) : "");
            return sql;
        }
        /// <summary>
        /// 
        /// </summary>
        public string BuildSelect()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_distinctBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _distinctBuffer);
            }
            if (_columnBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _columnBuffer);
            }
            else
            {
                sqlBuffer.AppendFormat(" {0}", string.Join(",", _table.Columns.FindAll(f => !_filters.Exists(e => e == f.ColumnName)).Select(s => string.Format("{0} AS {1}", s.ColumnName, s.CSharpName))));
            }
            sqlBuffer.AppendFormat(" FROM {0}", _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_orderBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" ORDER BY {0}", _orderBuffer);
            }
            if (pageIndex != null && pageCount != null)
            {
                sqlBuffer.AppendFormat(" LIMIT {0},{1}", pageIndex, pageCount);
            }
            if (_lock.Length > 0)
            {
                sqlBuffer.AppendFormat(" {0}", _lock);
            }
            var sql = sqlBuffer.ToString();
            return sql;
        }
        /// <summary>
        /// 
        /// </summary>
        public string BuildCount()
        {
            var sqlBuffer = new StringBuilder("SELECT");
            if (_columnBuffer.Length > 0)
            {

                sqlBuffer.Append(" COUNT(");
                if (_distinctBuffer.Length > 0)
                {
                    sqlBuffer.AppendFormat("{0} ", _distinctBuffer);
                }
                sqlBuffer.AppendFormat("{0})", _columnBuffer);
            }
            else
            {
                if (_groupBuffer.Length > 0)
                {
                    sqlBuffer.Append(" 1 AS COUNT");
                }
                else
                {
                    sqlBuffer.AppendFormat(" COUNT(1)");
                }
            }
            sqlBuffer.AppendFormat(" FROM {0}", _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                return string.Format("SELECT COUNT(1) FROM ({0}) AS T", sqlBuffer);
            }
            else
            {
                return sqlBuffer.ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BuildExists()
        {
            var sqlBuffer = new StringBuilder();

            sqlBuffer.AppendFormat("SELECT 1 FROM {0}", _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            if (_groupBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" GROUP BY {0}", _groupBuffer);
            }
            if (_havingBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" HAVING {0}", _havingBuffer);
            }
            var sql = string.Format("SELECT EXISTS({0})", sqlBuffer);
            return sql;
        }
        /// <summary>
        /// 
        /// </summary>
        public string BuildSum()
        {
            var sqlBuffer = new StringBuilder();
            sqlBuffer.AppendFormat("SELECT SUM({0}) FROM {1}", _sumBuffer, _table.TableName);
            if (_whereBuffer.Length > 0)
            {
                sqlBuffer.AppendFormat(" WHERE {0}", _whereBuffer);
            }
            return sqlBuffer.ToString();
        }
        #endregion

        #endregion
    }
}
