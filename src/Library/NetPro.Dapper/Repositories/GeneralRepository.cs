using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetPro.Dapper.Repositories
{
    /// <summary>
    /// 通用仓储
    /// </summary>
    /// <typeparam name="Context">数据库上下文</typeparam>
    /// <typeparam name="Table">表结构对应的Do模型</typeparam>
    public class GeneralRepository<Context, Table> : IGeneralRepository<Table> where Context : DapperContext where Table : class
    {
        private readonly IDapperRepository<Context> _dapperRepository;
        public GeneralRepository(IDapperRepository<Context> dapperRepository
        )
        {
            _dapperRepository = dapperRepository;
        }

        public virtual void SetMySqlConnectioin(int serverId)
        {
            //_dapperRepository.DbContext.SetTempConnection(_configuration.GetValue<string>($"ConnectionStrings:ServerIdConnection:{serverId}"));
        }

        public int Delete(Table entity)
        {
            return _dapperRepository.Delete(entity);
        }

        public int Delete(object id)
        {
            return _dapperRepository.Delete(id: id);
        }

        public async Task<int> DeleteAsync(Table entity)
        {
            return await _dapperRepository.DeleteAsync(entity);
        }

        public async Task<int> DeleteAsync(object id)
        {
            return await _dapperRepository.DeleteAsync(id);
        }

        public int QueryCountByDynamic(PageFilterDto pageFilterDto)
        {
            return _dapperRepository.QueryCountByDynamic<Table>(pageFilterDto);
        }

        public async Task<int> QueryCountByDynamicAsync(PageFilterDto pageFilterDto)
        {
            return await _dapperRepository.QueryCountByDynamicAsync<Table>(pageFilterDto);
        }


        //public Task<Table> QueryByIdAsync<Table>(string premeter, DynamicParameters parame)
        //{
        //    return await _dapperRepository.QueryFirstOrDefaultAsync<Table>("where ", parame);
        //}

        public IList<Table> QueryList(string conditions, DynamicParameters parame)
        {
            return _dapperRepository.QueryList<Table>(conditions, parame);
        }

        public async Task<IList<Table>> QueryListAsync(string conditions, DynamicParameters parame)
        {
            return await _dapperRepository.QueryListAsync<Table>(conditions, parame);
        }

        public IList<Table> QueryListPagedByDynamic(PageFilterDto pageFilterDto)
        {
            return _dapperRepository.QueryListPagedByDynamic<Table>(pageFilterDto);
        }

        public async Task<IList<Table>> QueryListPagedByDynamicAsync(PageFilterDto pageFilterDto)
        {
            return await _dapperRepository.QueryListPagedByDynamicAsync<Table>(pageFilterDto);
        }

        public int? Insert(Table entity)
        {
            return _dapperRepository.Insert(entity);
        }

        public async Task<int?> InsertAsync(Table entity)
        {
            return await _dapperRepository.InsertAsync(entity);
        }

        public int Update(Table entity)
        {
            return _dapperRepository.Update(entity);
        }

        public async Task<int> UpdateAsync(Table entity)
        {
            return await _dapperRepository.UpdateAsync(entity);
        }
    }
}
