using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XXX.Plugin.Web.Manager
{
    public interface IUserService
    {
        Task<int> DeleteAsync(uint id);
        Task<int> InsertAsync();
        Task<PagedList<User>> SearchAsync(UserSearchAo search);
        Task<int> UpdatePatchAsync(uint id, uint age);
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IFreeSql _fsql;
        private readonly IMapper _mapper;
        public UserService(ILogger<UserService> logger
            , IFreeSql fsql
            , IMapper mapper)
        {
            _logger = logger;
            _fsql = fsql;
            _mapper = mapper;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <returns></returns>
        public async Task<int> InsertAsync()
        {
            var affrows = await _fsql.Insert(new User
            {
                Age = 10,
                CreateTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Name = "姓名",
                NickName = "昵称",
                Pwd = "pwd"
            }).ExecuteAffrowsAsync();
            _logger.LogInformation("新增成功");
            return affrows;
        }

        /// <summary>
        /// 根据id删除
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteAsync(uint id)
        {
            var affrows = await _fsql.Delete<User>(id)
                .Where(x => x.Id == id)
                .ExecuteAffrowsAsync();
            _logger.LogError("删除成功 ");
            return affrows;
        }

        /// <summary>
        /// 更新单个值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="age"></param>
        /// <returns></returns>
        public async Task<int> UpdatePatchAsync(uint id, uint age)
        {
            var affrows = await _fsql.GetGuidRepository<User>().UpdateDiy
                .Set(s => new User { Age = age })
                .Where(x => x.Id == id)
                .ExecuteAffrowsAsync();

            _logger.LogError("更新单个值成功");
            return affrows;
        }

        /// <summary>
        ///  查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PagedList<User>> SearchAsync(UserSearchAo search)
        {
            var list = await _fsql.GetRepository<User>().Select.From<UserRelation, Company>((user, userRe, comp) => user
             .InnerJoin(a => a.Id == userRe.UserId).InnerJoin(a => userRe.CompanyId == comp.Id))
            .WhereIf(search.Age > 0, (a, s, d) => a.Age == search.Age)//查询条件
            .OrderByDescending((a, s, d) => a.Id)                   //排序：以User.Id 降序排序
            .Page(search.PageIndex, search.PageSize)            //分页
            .Count(out long totalCount)                     //返回结果总数
            .ToListAsync((u, r, c) => new User
            {
                Id = r.Id,
                Age = u.Age,
                NickName = u.NickName
            });
            _logger.LogError("查询列表成功");
            var result = new PagedList<User>(list, search.PageIndex, search.PageSize, totalCount);
            return result;
        }
    }
}
