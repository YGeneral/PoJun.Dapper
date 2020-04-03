using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoJun.Dapper.IRepository
{
	/// <summary>
	/// ISqlBuilder
	/// 创建人：杨江军
	/// 创建时间：2020/4/2 16:46:40
	/// </summary>
	public interface ISqlBuilder
	{
		string Build(Dictionary<string, object> values, string prefix);
	}

	/// <summary>
	/// ISubQuery
	/// 创建人：杨江军
	/// 创建时间：2020/4/2 16:46:40
	/// </summary>
	public interface ISubQuery : ISqlBuilder
	{
	}
}
