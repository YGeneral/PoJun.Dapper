using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper
{
    /// <summary>
	/// LockType
	/// 创建人：杨江军
	/// 创建时间：2020/4/2 16:54:59
	/// </summary>
	public enum LockType
    {
        /// <summary>
        /// 
        /// </summary>
        FOR_UPADTE,
        /// <summary>
        /// 
        /// </summary>
        LOCK_IN_SHARE_MODE,
        /// <summary>
        /// 
        /// </summary>
        UPDLOCK,
        /// <summary>
        /// 
        /// </summary>
        NOLOCK
    }
}
