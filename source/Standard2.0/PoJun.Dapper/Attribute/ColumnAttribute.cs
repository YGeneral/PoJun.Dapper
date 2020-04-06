using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper
{
    /// <summary>
	/// ColumnAttribute
	/// 创建人：杨江军
	/// 创建时间：2020/4/2 16:53:29
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ColumnKey Key { get; set; }

        /// <summary>
        /// 是否是字段(如果等于false则不参与SQL的生成)
        /// </summary>
        public bool IsColumn { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 初始化字段信息
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="key"></param>
        /// <param name="isIdentity">是否自增</param>
        /// <param name="isColumn">是否是字段(如果等于false则不参与SQL的生成)</param>
        public ColumnAttribute(string name = null, ColumnKey key = ColumnKey.None, bool isIdentity = false, bool isColumn = true)
        {
            Name = name;
            Key = key;
            IsColumn = isColumn;
            IsIdentity = isIdentity;
        }
    }
}
