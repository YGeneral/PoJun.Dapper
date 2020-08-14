using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper
{
    /// <summary>
	/// TableAttribute
	/// 创建人：杨江军
	/// 创建时间：2020/4/2 16:54:33
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public TableAttribute(string name = null)
        {
            Name = name;
        }
    }
}
