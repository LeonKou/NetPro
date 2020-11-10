using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetPro.Dapper.Repositories
{
    public interface IGeneralRepository<Table> where Table : class
    {
        /// <summary>
        /// 设置数据库连接
        /// </summary>
        /// <param name="serverId"></param>
        /// <remarks>默认没有实现，由调用者重写设置连接逻辑</remarks>
        void SetMySqlConnectioin(int serverId);

        #region 同步

        /// <summary>
        ///  插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int? Insert(Table entity);

        /// <summary>
        ///  根据主键，删除唯一的一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>受影响行数</returns>
        int Delete(Table entity);

        /// <summary>
        /// 根据主键，删除唯一的一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete(object id);

        /// <summary>
        /// 根据主键，更新唯一的一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(Table entity);

        /// <summary>
        /// 依据条件查询数据
        ///  <![CDATA[例如：QueryList<User>("age = @Age or Name like @Name", new {Age = 10, Name = likename})]]>
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns>受影响的行数</returns>
        IList<Table> QueryList(string conditions, DynamicParameters parame);

        /// <summary>
        /// 查询分页列表(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        IList<Table> QueryListPagedByDynamic(PageFilterDto pageFilterDto);

        /// <summary>
        /// 查询分页总数(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        int QueryCountByDynamic(PageFilterDto pageFilterDto);

        #endregion

        #region 异步

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int?> InsertAsync(Table entity);

        /// <summary>
        /// 根据主键，删除唯一的一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        Task<int> DeleteAsync(Table entity);

        /// <summary>
        /// 根据主键，删除唯一的一条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns>受影响的行数</returns>
        Task<int> DeleteAsync(object id);

        /// <summary>
        /// 根据主键，更新唯一的一条记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>受影响的行数</returns>
        Task<int> UpdateAsync(Table entity);

        /// <summary>
        /// 依据条件查询数据
        ///  <![CDATA[ 例如：QueryListAsync<User>("age = @Age or Name like @Name", new {Age = 10, Name = likename})]]>
        /// </summary>
        /// <param name="parame"></param>
        /// <returns>受影响的行数</returns>
        Task<IList<Table>> QueryListAsync(string conditions, DynamicParameters parame);

        /// <summary>
        /// 查询分页列表(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        Task<IList<Table>> QueryListPagedByDynamicAsync(PageFilterDto pageFilterDto);

        /// <summary>
        /// 查询分页总数(动态传参)
        /// </summary>
        /// <param name="pageFilterDto"></param>
        /// <returns></returns>
        Task<int> QueryCountByDynamicAsync(PageFilterDto pageFilterDto);

        #endregion
    }
}
