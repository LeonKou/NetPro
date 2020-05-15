using NetPro.Core.Infrastructure;
using NetPro.Dapper;
using NetPro.Dapper.Repositories;
namespace Leon.XXX.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public class XXXRepository : GeneralRepository<DefaultDapperContext, XXXDo>, IXXXRepository
    {
        private readonly IDapperRepository<DefaultDapperContext> _dapperRepository;
        private readonly IUnitOfWorkFactory<DefaultDapperContext> _unitOfWorkFactoryNew;
        public XXXRepository(IDapperRepository<DefaultDapperContext> dapperRepository,
            IUnitOfWorkFactory<DefaultDapperContext> _unitOfWorkFactoryNew) : base(dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        /// <summary>
        /// 插入(带事务)
        /// </summary>
        public void Insert()
        {
            SetMySqlConnectioin(2);
            var unit = _unitOfWorkFactoryNew.Create();

            Insert(new XXXDo());
            unit.SaveChanges();
        }

        /// <summary>
        ///  重写切换数据库逻辑
        /// </summary>
        /// <param name="serverId"></param>
        public override void SetMySqlConnectioin(int serverId)
        {
            //数据库从Apollo读取
            var context = EngineContext.Current.Resolve<DefaultDapperContext>();
            if (serverId == 1)
            {
                context.SetTempConnection("Server=");
                return;
            }
            if (serverId == 2)
            {
                context.SetTempConnection("Server=");
                return;
            }
        }
    }
}
