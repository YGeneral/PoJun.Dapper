using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper.IRepository
{
    /// <summary>
    /// 基础仓储服务接口
    /// </summary>
    public interface IBaseRepository<T>
    {
        #region 纯SQL执行

        /// <summary>
        /// 执行增（INSERT）语句【执行单条SQL语句，返回自增列】【同步】
        /// </summary>
        /// <param name="sql">SQL语句（本方法会自动在SQL的结尾增加分号，并增加查询ID的语句）</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>自增ID</returns>
        int Insert(string sql, object param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行增（INSERT）语句【执行单条SQL语句，返回自增列】【异步】
        /// </summary>
        /// <param name="sql">SQL语句（本方法会自动在SQL的结尾增加分号，并增加查询ID的语句）</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>自增ID</returns>
        Task<int> InsertAsync(string sql, object param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句或存储过程【单条SQL语句，可以获得输出参数】【同步】
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="outParam">输出参数[参数名(@name)] </param>
        /// <param name="commandType">如何解释命令字符串</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>Tuple(受影响的行数, Dictionary(输出参数名(@name), 输出参数))</returns>
        Tuple<int, Dictionary<string, string>> Execute(string sql, DynamicParameters param, List<string> outParam, CommandType? commandType = null, int? commandTimeout = null);

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句或存储过程【单条SQL语句，可以获得输出参数】【异步】
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="outParam">输出参数[参数名(@name)] </param>
        /// <param name="commandType">如何解释命令字符串</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>Tuple(受影响的行数, Dictionary(输出参数名(@name), 输出参数))</returns>
        Task<Tuple<int, Dictionary<string, string>>> ExecuteAsync(string sql, DynamicParameters param, List<string> outParam, CommandType? commandType = null, int? commandTimeout = null);

        /// <returns></returns>
        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【执行单条SQL语句】【同步】
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>受影响的行数</returns>
        int Execute(string sql, object param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【执行单条SQL语句】【异步】
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>受影响的行数</returns>
        Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【带事务，可以同时执行多条SQL】【同步】
        /// </summary>
        /// <param name="sqlDic">SQL + 参数【key：sql语句 value：参数】</param>ConnectionString
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        void ExecuteToTransaction(Dictionary<string, object> sqlDic, int? commandTimeout = null);

        /// <summary>
        /// 执行增（INSERT）删（DELETE）改（UPDATE）语句【带事务，可以同时执行多条SQL】【异步】【待测试】
        /// </summary>
        /// <param name="sqlDic">SQL + 参数【key：sql语句 value：参数】</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task ExecuteToTransactionAsync(Dictionary<string, object> sqlDic, int? commandTimeout = null);

        /// <summary>
        /// 执行分页存储过程（Procdeure）【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Tuple<IEnumerable<T>, int> ExecuteToPaginationProcdeure<T>(DynamicParameters param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行分页存储过程（Procdeure）【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<Tuple<IEnumerable<T>, int>> ExecuteToPaginationProcdeureAsync<T>(DynamicParameters param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行存储过程（Procdeure）【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="porcdeureName">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        IEnumerable<T> ExecuteToProcdeure<T>(string porcdeureName, object param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行存储过程（Procdeure）【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="porcdeureName">存储过程名称</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<IEnumerable<T>> ExecuteToProcdeureAsync<T>(string porcdeureName, object param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行查询【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(string sql, object param = null, int? commandTimeout = null);

        /// <summary>
        /// 执行查询【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int? commandTimeout = null);

        /// <summary>
        /// 分页查询【同步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="sql">查询数据的SQL(请在select 后面加上 SQL_CALC_FOUND_ROWS 关键字)</param>
        /// <param name="totalSql">查询数据数量的SQL(默认值：select FOUND_ROWS() AS Count)</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>Data：查询出的数据；Count：当前查询条件下数据的数量</returns>
        (IEnumerable<T> Data, long Count) QueryPageList<T>(string sql, string totalSql = "select FOUND_ROWS() AS Count", object param = null, int? commandTimeout = null);

        /// <summary>
        /// 分页查询【异步】
        /// </summary>
        /// <typeparam name="T">返回结果的类型</typeparam>
        /// <param name="sql">查询数据的SQL(请在select 后面加上 SQL_CALC_FOUND_ROWS 关键字)</param>
        /// <param name="totalSql">查询数据数量的SQL(默认值：select FOUND_ROWS() AS Count)</param>
        /// <param name="param">参数</param>
        /// <param name="commandTimeout">超时时间</param>
        /// <returns>Data：查询出的数据；Count：当前查询条件下数据的数量</returns>
        Task<(IEnumerable<T> Data, long Count)> QueryPageListAsync<T>(string sql, string totalSql = "select FOUND_ROWS() AS Count", object param = null, int? commandTimeout = null);

        #endregion

        #region Linq执行

        /// <summary>
        /// 初始化Linq查询
        /// </summary>
        /// <returns></returns>
        IBaseRepository<T> initLinq();
        /// <summary>
        /// 
        /// </summary>
        string BuildInsert(Expression expression = null);
        /// <summary>
        /// 
        /// </summary>
        string BuildUpdate(bool allColumn = true);
        /// <summary>
        /// 
        /// </summary>
        string BuildUpdate(Expression expression);
        /// <summary>
        /// 
        /// </summary>
        string BuildDelete();
        /// <summary>
        /// 
        /// </summary>
        string BuildSelect();
        /// <summary>
        /// 
        /// </summary>
        string BuildCount();
        /// <summary>
        /// 
        /// </summary>
        string BuildExists();
        /// <summary>
        /// 
        /// </summary>
        string BuildSum();
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, ISubQuery subquery);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, TResult value);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Set<TResult>(Expression<Func<T, TResult>> column, Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> GroupBy(string expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> GroupBy<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Where(string expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Where(Expression<Func<T, bool?>> expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> OrderBy(string orderBy);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> OrderBy<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> OrderByDescending<TResult>(Expression<Func<T, TResult>> expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Skip(int index, int count);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Take(int count);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Page(int index, int count, out long total);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Having(string expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Having(Expression<Func<T, bool?>> expression);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Filter<TResult>(Expression<Func<T, TResult>> columns);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> With(string lockType);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> With(LockType lockType);
        /// <summary>
        /// 
        /// </summary>
        IBaseRepository<T> Distinct();
        /// <summary>
        /// 
        /// </summary>
        T Single(string columns = null, bool buffered = true, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<T> SingleAsync(string columns = null, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        TResult Single<TResult>(string columns = null, bool buffered = true, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<TResult> SingleAsync<TResult>(string columns = null, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        TResult Single<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<TResult> SingleAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<T> Select(string colums = null, bool buffered = true, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<IEnumerable<T>> SelectAsync(string colums = null, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<TResult> Select<TResult>(string colums = null, bool buffered = true, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<IEnumerable<TResult>> SelectAsync<TResult>(string colums = null, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> columns, bool buffered = true, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<IEnumerable<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> columns, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        int Insert(T entity, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        int Insert(Expression<Func<T, T>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<int> InsertAsync(Expression<Func<T, T>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        long InsertReturnId(Expression<Func<T, T>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<long> InsertReturnIdAsync(Expression<Func<T, T>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<int> InsertAsync(T entity, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        long InsertReturnId(T entity, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<long> InsertReturnIdAsync(T entity, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        int Insert(IEnumerable<T> entitys, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<int> InsertAsync(IEnumerable<T> entitys, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        int Update(int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<int> UpdateAsync(int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        int Update(T entity, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        int Update(Expression<Func<T, T>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<int> UpdateAsync(Expression<Func<T, T>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<int> UpdateAsync(T entity, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        int Update(IEnumerable<T> entitys, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<int> UpdateAsync(IEnumerable<T> entitys, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        int Delete(int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<int> DeleteAsync(int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        bool Exists(int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<bool> ExistsAsync(int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        long Count(string columns = null, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<long> CountAsync(string columns = null, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        long Count<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<long> CountAsync<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        TResult Sum<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null);
        /// <summary>
        /// 
        /// </summary>
        Task<TResult> SumAsync<TResult>(Expression<Func<T, TResult>> expression, int? timeout = null);

        #endregion
    }
}
