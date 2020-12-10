using FreeSql.Internal.Model;
using Leon.XXX.Repository;
using NetPro.Web.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leon.XXX.Domain
{
    /// <summary>
    /// 数据库 简单操作操作范例
    /// 更详细FreeSql操作请跳转至 https://www.cnblogs.com/FreeSql/ 学习了解
    /// </summary>
    public interface IDataBaseOptionService
    {
        /// <summary>
        /// 增加实体
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult<XXXDo>> AddAsync(XXXDo xXXDo);

        /// <summary>
        /// 根据主键id删除，也可根据复杂条件删除
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult> DeleteAsync(uint id);

        /// <summary>
        /// 更新整个实体，也可更新多个值而不是整个实体
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult> UpdateAsync(XXXAo xXXAo);

        /// <summary>
        /// 更新单个值 
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult> UpdateSetAsync(string singleValue);

        /// <summary>
        /// 插入或者更新整个实体
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult> InsertOrUpdateAsync(XXXAo xXXAo);

        /// <summary>
        ///  根据主键id查找
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult<XXXAo>> FindAsync(uint id);

        /// <summary>
        ///  根据主键id先从redis查找，查找不到自动到数据库查找并插入到Redis中
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult<XXXAo>> FindByRedisAsync(uint id);

        /// <summary>
        /// 动态条件查询，由客户端自由组织查询条件类似jira条件查询
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult<IList<XXXAo>>> DynamicQueryAsync(DynamicFilterInfo dynamicFilter);

    }
}