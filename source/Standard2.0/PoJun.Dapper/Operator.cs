using PoJun.Dapper.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper
{
    /// <summary>
	/// Operator
	/// 创建人：杨江军
	/// 创建时间：2020/4/2 16:50:21
	/// </summary>
	public static class Operator
    {
        #region extension

        /// <summary>
        /// 
        /// </summary>
        public static bool In(ValueType column, IEnumerable enumerable)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool In(ValueType column, params ValueType[] value)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool In(ValueType column, ISubQuery subuery)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotIn(ValueType column, IEnumerable enumerable)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotIn(ValueType column, params ValueType[] value)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotIn(ValueType column, ISubQuery subuery)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool In(string column, IEnumerable enumerable)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool In(string column, params string[] value)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool In(string column, ISubQuery subquery)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotIn(string column, IEnumerable enumerable)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotIn(string column, params string[] value)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotIn(string column, ISubQuery subquery)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static T Any<T>(T subquery) where T : ISubQuery
        {
            return default;
        }
        /// <summary>
        /// 
        /// </summary>
        public static T All<T>(T subuery) where T : ISubQuery
        {
            return default;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool Exists(ISubQuery subquery)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotExists(ISubQuery subuery)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool Contains(string column, string text)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotContains(string column, string text)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool StartsWith(string column, string text)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotStartsWith(string column, string text)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool EndsWith(string column, string text)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotEndsWith(string column, string text)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool Regexp(string column, string regexp)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotRegexp(string column, string regexp)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool IsNull<T>(T column)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool IsNotNull<T>(T column)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool Between<T>(T column, T value1, T value2)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool NotBetween<T>(T column, T value1, T value2)
        {
            return true;
        }
        #endregion

        #region utils
        /// <summary>
        /// 
        /// </summary>
        public static string GetOperator(string operatorType)
        {
            switch (operatorType)
            {
                case nameof(Operator.In):
                    operatorType = "IN";
                    break;
                case nameof(Operator.NotIn):
                    operatorType = "NOT IN";
                    break;
                case nameof(Operator.Any):
                    operatorType = "ANY";
                    break;
                case nameof(Operator.All):
                    operatorType = "ALL";
                    break;
                case nameof(Operator.Exists):
                    operatorType = "EXISTS";
                    break;
                case nameof(Operator.NotExists):
                    operatorType = "NOT EXISTS";
                    break;
                case nameof(Operator.Contains):
                case nameof(Operator.StartsWith):
                case nameof(Operator.EndsWith):
                    operatorType = "LIKE";
                    break;
                case nameof(Operator.NotContains):
                case nameof(Operator.NotStartsWith):
                case nameof(Operator.NotEndsWith):
                    operatorType = "NOT LIKE";
                    break;
                case nameof(Operator.IsNull):
                    operatorType = "IS NULL";
                    break;
                case nameof(Operator.IsNotNull):
                    operatorType = "IS NOT NULL";
                    break;
                case nameof(Operator.Between):
                    operatorType = "BETWEEN";
                    break;
                case nameof(Operator.NotBetween):
                    operatorType = "NOT BETWEEN";
                    break;
                case nameof(Operator.Regexp):
                    operatorType = "REGEXP";
                    break;
                case nameof(Operator.NotRegexp):
                    operatorType = "NOT REGEXP";
                    break;
            }
            return operatorType;
        }
        /// <summary>
        /// 
        /// </summary>
        public static string GetOperator(ExpressionType type)
        {
            var condition = string.Empty;
            switch (type)
            {
                case ExpressionType.Add:
                    condition = "+";
                    break;
                case ExpressionType.Subtract:
                    condition = "-";
                    break;
                case ExpressionType.Multiply:
                    condition = "*";
                    break;
                case ExpressionType.Divide:
                    condition = "/";
                    break;
                case ExpressionType.Modulo:
                    condition = "%";
                    break;
                case ExpressionType.Equal:
                    condition = "=";
                    break;
                case ExpressionType.NotEqual:
                    condition = "<>";
                    break;
                case ExpressionType.GreaterThan:
                    condition = ">";
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    condition = ">=";
                    break;
                case ExpressionType.LessThan:
                    condition = "<";
                    break;
                case ExpressionType.LessThanOrEqual:
                    condition = "<=";
                    break;
                case ExpressionType.OrElse:
                    condition = "OR";
                    break;
                case ExpressionType.AndAlso:
                    condition = "AND";
                    break;
                case ExpressionType.Not:
                    condition = "NOT";
                    break;
            }
            return condition;
        }
        #endregion
    }
}
