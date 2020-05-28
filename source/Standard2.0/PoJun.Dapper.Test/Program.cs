using PoJun.Dapper.IRepository;
using PoJun.Dapper.Repository.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoJun.Dapper.Test
{
    class Program
    {
        public static UserRepository userRepository = new UserRepository();

        static async Task Main(string[] args)
        {
            var name = "123";
            var result161 = userRepository.initLinq().Where(a => a.Name == name).Select().ToList();
            #region 新增

            //普通新增
            var resut1 = userRepository.initLinq().Insert(new User() { Name = $"PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}", Age = 30 });
            //普通新增
            var result2 = await userRepository.initLinq().InsertAsync(new User() { Name = $"PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}", Age = 30 });
            //新增返回自增ID
            var result3 = await userRepository.initLinq().InsertReturnIdAsync(new User() { Name = $"PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}", Age = 30 });
            //新增返回自增ID
            var result4 = userRepository.initLinq().InsertReturnId(new User() { Name = $"PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}", Age = 30 });

            //批量新增
            var addList = new List<User>();
            addList.Add(new User() { Name = $"PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}", Age = 30 });
            addList.Add(new User() { Name = $"PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}", Age = 30 });
            addList.Add(new User() { Name = $"PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}", Age = 30 });
            var result5 = await userRepository.initLinq().InsertAsync(addList);

            #endregion

            #region 修改

            //单条主键更新（并且不更新Age字段）
            var result6 = await userRepository.initLinq().Filter(x => x.Age).UpdateAsync(new User() { Id = 1, Name = $"修改后的PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}", Sex = 2 });
            //单条主键更新（并且不更新Age、Sex字段）
            var result7 = await userRepository.initLinq().Filter(x => x.Age).Filter(x => x.Sex).UpdateAsync(new User() { Id = 2, Name = $"修改后的PoJun{DateTime.Now.ToString("yyyyMMddHHmmss")}" });
            //动态更新字段
            var result8 = await userRepository.initLinq().Where(x => x.IsDelete == false && x.Id > 30 && x.Id < 50).Set(x => x.IsDelete, true).UpdateAsync();

            #endregion

            #region 删除

            //var result9 = await userRepository.initLinq().Where(x => Operator.In(x.Id, new int[] { 3, 4, 5 })).DeleteAsync();

            //var idlist = new List<int>() { 6, 7 };
            //var result10 = await userRepository.initLinq().Where(x => Operator.In(x.Id, idlist)).DeleteAsync();

            #endregion

            #region 查询

            #region 查询单条数据

            //查询数据库中的第一条记录
            var result11 = await userRepository.initLinq().SingleAsync();
            //查询数据库中的最后一条记录
            var result12 = userRepository.initLinq().OrderByDescending(a => a.Id).Single();
            //查询数据库中的第一条记录
            var result13 = userRepository.initLinq().OrderBy("Id").Single();
            //查询ID等于24的Name字段
            var result14 = userRepository.initLinq().Where(a => a.Id == 24).Single(s => s.Name); 

            #endregion

            #region 查询多条数据

            //查询多条数据
            var result15 = userRepository.initLinq().Select().ToList();
            var result16 = userRepository.initLinq().Where(a => a.Age > 28).Select().ToList();
            
            var result17 = userRepository.initLinq().OrderBy(a => a.Id).OrderByDescending(a => a.Name).Select().ToList();
            var result18 = userRepository.initLinq().Take(4).Select().ToList();
            var result19 = userRepository.initLinq().Take(4).Skip(2, 2).Select().ToList();
            var result20 = userRepository.initLinq().Select(s => new { s.Id, s.Name }).ToList();
            var result21 = userRepository.initLinq().Select(s => new UserModel { Id = s.Id, Name = s.Name }).ToList(); 

            #endregion

            #region 分页查询

            //第一种方式
            var result22 = userRepository.initLinq().Page(1, 2, out long total1).Select().ToList();
            var result23 = userRepository.initLinq().Page(2, 2, out long total2).Select().ToList();
            //第二种方式
            var id = 24;
            long total = 0;
            var result24 = userRepository.initLinq().Where(a => a.Id > id).Page(1, 2, out total).Select();
            var result25 = userRepository.initLinq().Where(a => a.Id > id).Page(2, 2, out total).Select();
            //分组分页 
            var result26 = userRepository.initLinq().GroupBy(a => a.Name).Page(1, 2, out total).Select(s => new { s.Name });
            var result27 = userRepository.initLinq().GroupBy(a => a.Name).Page(2, 2, out total).Select(s => new { s.Name }); 

            #endregion

            
            var idlists = new List<int>() { 30,31,32,33,34,35 };
            var result28 = userRepository.initLinq().Where(x => Operator.In(x.Id, idlists)).Select().ToList();


            var id2ists = new List<string>() { "123" };
            var result29 = userRepository.initLinq().Where(x => Operator.In(x.Name, id2ists)).Select().ToList();

            #endregion

            Console.WriteLine(resut1);
        }
    }

    public class UserModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// 【XXX业务库】基础仓储(MySql)
    /// </summary>
    public class BaseRepositoryToPoJun_Shadow<T> : BaseRepository<T>, IBaseRepository<T> where T : class
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public BaseRepositoryToPoJun_Shadow() : base("Server=192.168.0.49;Database=test_pojun;Uid=user;Pwd=Asher1234;pooling=true;charset=utf8;Convert Zero Datetime=True;port=3306;Connection Timeout=120;SslMode = none;")
        {

        }
    }

    public interface IUserRepository : IBaseRepository<User>
    {

    }

    public class UserRepository : BaseRepositoryToPoJun_Shadow<User>, IUserRepository
    {

    }

    public class User
    {
        [Column("`Id`", ColumnKey.Primary, true)]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }


        public int Sex { get; set; }

        public bool IsDelete { get; set; }
    }
}
