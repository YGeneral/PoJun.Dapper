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
        public string Name { get; set; }
        public ColumnKey Key { get; set; }
        public bool IsColumn { get; set; }
        public bool IsIdentity { get; set; }
        public ColumnAttribute(string name = null, ColumnKey key = ColumnKey.None, bool isIdentity = false, bool isColumn = true)
        {
            Name = name;
            Key = key;
            IsColumn = isColumn;
            IsIdentity = isIdentity;
        }
    }
}
