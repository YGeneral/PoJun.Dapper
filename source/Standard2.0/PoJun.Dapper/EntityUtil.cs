﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper
{
    /// <summary>
	/// EntityUtil
	/// 创建人：杨江军
	/// 创建时间：2020/4/2 18:42:57
	/// </summary>
	public class EntityUtil
    {
        private static List<EntityTable> _database = new List<EntityTable>();
        private static EntityTable Build(Type type)
        {
            if (!_database.Exists(e => e.CSharpType == type))
            {
                var properties = type.GetProperties();
                var columns = new List<EntityColumn>();
                foreach (var item in properties)
                {
                    var columnName = item.Name;
                    var identity = false;
                    var columnKey = ColumnKey.None;
                    if (item.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0)
                    {
                        var columnAttribute = item.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() as ColumnAttribute;
                        if (!columnAttribute.IsColumn)
                        {
                            continue;
                        }
                        columnKey = columnAttribute.Key;
                        identity = columnAttribute.IsIdentity;
                        columnName = string.IsNullOrEmpty(columnAttribute.Name) ? item.Name : columnAttribute.Name;
                    }
                    columns.Add(new EntityColumn()
                    {
                        ColumnKey = columnKey,
                        ColumnName = columnName,
                        CSharpName = item.Name,
                        Identity = identity,
                    });
                }
                if (columns.Count > 0 && !columns.Exists(e => e.ColumnKey == ColumnKey.Primary))
                {
                    columns[0].ColumnKey = ColumnKey.Primary;
                }
                var tableName = type.Name;
                if (type.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
                {
                    tableName = (type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute).Name;
                }
                var table = new EntityTable()
                {
                    CSharpName = type.Name,
                    CSharpType = type,
                    TableName = tableName,
                    Columns = columns,
                };
                lock (_database)
                {
                    if (!_database.Exists(e => e.CSharpType == type))
                    {
                        _database.Add(table);
                    }
                }
            }
            return _database.Find(f => f.CSharpType == type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EntityTable GetTable<T>() where T : class
        {
            return Build(typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static EntityTable GetTable(Type type)
        {
            return Build(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static EntityColumn GetColumn(Type type, Func<EntityColumn, bool> func)
        {
            return Build(type).Columns.Find(f => func(f));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EntityTable
    {
        /// <summary>
        /// 
        /// </summary>
        public Type CSharpType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CSharpName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<EntityColumn> Columns { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EntityColumn
    {
        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CSharpName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Identity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ColumnKey ColumnKey { get; set; }
    }
}
