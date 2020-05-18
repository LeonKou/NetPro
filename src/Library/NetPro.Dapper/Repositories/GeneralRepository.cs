using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

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
        public GeneralRepository(IDapperRepository<Context> dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public virtual void SetMySqlConnectioin(int serverId)
        {
            //通过apollo读取具体换库逻辑  
            _dapperRepository.DbContext.SetTempConnection("172.16.25.58;Port=41022;Database=demobase;charset=utf8;user=myname;password=***;");
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

        public int GetCountByDynamic(PageFilterDto pageFilterDto)
        {
            return _dapperRepository.GetCountByDynamic<Table>(pageFilterDto);
        }

        public async Task<int> GetCountByDynamicAsync(PageFilterDto pageFilterDto)
        {
            return await _dapperRepository.GetCountByDynamicAsync<Table>(pageFilterDto);
        }

        public IList<Table> GetList<Table>(string conditions, DynamicParameters parame)
        {
            return _dapperRepository.GetList<Table>(conditions, parame);
        }

        public async Task<IList<Table>> GetListAsync(string conditions, DynamicParameters parame)
        {
            return await _dapperRepository.GetListAsync<Table>(conditions, parame);
        }

        public IList<Table> GetListPagedByDynamic(PageFilterDto pageFilterDto)
        {
            return _dapperRepository.GetListPagedByDynamic<Table>(pageFilterDto);
        }

        public async Task<IList<Table>> GetListPagedByDynamicAsync(PageFilterDto pageFilterDto)
        {
            return await _dapperRepository.GetListPagedByDynamicAsync<Table>(pageFilterDto);
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
