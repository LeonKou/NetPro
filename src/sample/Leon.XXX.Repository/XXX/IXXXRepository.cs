using FreeSql;

namespace Leon.XXX.Repository
{
    public class XXXRepository : BaseRepository<XXXDo, int>, IXXXRepository
    {
        public XXXRepository(IFreeSql fsql) : base(fsql, null, null) { }
    }
    public interface IXXXRepository : IBaseRepository<XXXDo, int>
    {
    }
}
