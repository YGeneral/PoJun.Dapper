using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper
{
    /// <summary>
	/// ColumnKey
	/// 创建人：杨江军
	/// 创建时间：2020/4/2 16:54:06
	/// </summary>
	public enum ColumnKey
    {
        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 主键
        /// </summary>
        Primary,

        /// <summary>
        /// 
        /// </summary>
        Quniue,

        /// <summary>
        /// 唯一
        /// </summary>
        Unique,

        /// <summary>
        /// 外键
        /// </summary>
        Foreign
    }
}
