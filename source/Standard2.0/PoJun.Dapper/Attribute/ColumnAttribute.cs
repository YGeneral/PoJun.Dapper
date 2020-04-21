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
        /// 字段名称（用于映射字段名和数据库字段）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 目前实现了Primary的定义，设置为Primary的字段update实体时，默认采用该字段为更新条件
        /// </summary>
        public ColumnKey Key { get; set; }

        /// <summary>
        /// 标识该字段是否在数据库存在，用于扩展User而不在sql中生成该字段
        /// </summary>
        public bool IsColumn { get; set; }

        /// <summary>
        /// 设置未true时在Insert时不会向该字段设置任何值
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 初始化字段信息
        /// </summary>
        /// <param name="name">字段名称（用于映射字段名和数据库字段）</param>
        /// <param name="key">目前实现了Primary的定义，设置为Primary的字段update实体时，默认采用该字段为更新条件</param>
        /// <param name="isIdentity">设置未true时在Insert时不会向该字段设置任何值</param>
        /// <param name="isColumn">标识该字段是否在数据库存在，用于扩展User而不在sql中生成该字段</param>
        public ColumnAttribute(string name = null, ColumnKey key = ColumnKey.None, bool isIdentity = false, bool isColumn = true)
        {
            Name = name;
            Key = key;
            IsColumn = isColumn;
            IsIdentity = isIdentity;
        }
    }
}
