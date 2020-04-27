using Dapper;
using NetPro.Core.Infrastructure;
using NetPro.Core.Infrastructure.PagedList;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetPro.Dapper.Repositories
{
	public interface IGeneralRepository<Table> where Table : class
	{
		void SetMySqlConnectioin(int serverId);

		#region 同步

		/// <summary>
		/// 插入
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		int? Insert(Table entity);

		/// <summary>
		/// 根据主键，删除唯一的一条记录
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns>受影响的行数</returns>
		int Delete(Table entity);

		/// <summary>
		/// 根据主键，删除唯一的一条记录
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns>受影响的行数</returns>
		int Delete(object id);

		/// <summary>
		/// 根据主键，更新唯一的一条记录
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns>受影响的行数</returns>
		int Update(Table entity);

		/// <summary>
		/// 依据条件查询数据
		/// 例如：GetList<User>("age = @Age or Name like @Name", new {Age = 10, Name = likename})
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns>受影响的行数</returns>
		IList<Table> GetList<Table>(string conditions, DynamicParameters parame);

		/// <summary>
		/// 查询分页列表(动态传参)
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="pageFilterDto"></param>
		/// <returns></returns>
		IList<Table> GetListPagedByDynamic(PageFilterDto pageFilterDto);

		/// <summary>
		/// 查询分页总数(动态传参)
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="pageFilterDto"></param>
		/// <returns></returns>
		int GetCountByDynamic(PageFilterDto pageFilterDto);

		#endregion

		#region 异步

		/// <summary>
		/// 插入
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns></returns>
		Task<int?> InsertAsync(Table entity);

		/// <summary>
		/// 根据主键，删除唯一的一条记录
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns>受影响的行数</returns>
		Task<int> DeleteAsync(Table entity);

		/// <summary>
		/// 根据主键，删除唯一的一条记录
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns>受影响的行数</returns>
		Task<int> DeleteAsync(object id);

		/// <summary>
		/// 根据主键，更新唯一的一条记录
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns>受影响的行数</returns>
		Task<int> UpdateAsync(Table entity);

		/// <summary>
		/// 依据条件查询数据
		/// 例如：GetList<User>("age = @Age or Name like @Name", new {Age = 10, Name = likename})
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="entity"></param>
		/// <returns>受影响的行数</returns>
		Task<IList<Table>> GetListAsync(string conditions, DynamicParameters parame);

		/// <summary>
		/// 查询分页列表(动态传参)
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="pageFilterDto"></param>
		/// <returns></returns>
		Task<IList<Table>> GetListPagedByDynamicAsync(PageFilterDto pageFilterDto);

		/// <summary>
		/// 查询分页总数(动态传参)
		/// </summary>
		/// <typeparam name="Table"></typeparam>
		/// <param name="pageFilterDto"></param>
		/// <returns></returns>
		Task<int> GetCountByDynamicAsync(PageFilterDto pageFilterDto);

		#endregion
	}
}
