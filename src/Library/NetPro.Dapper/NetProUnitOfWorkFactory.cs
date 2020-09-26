namespace NetPro.Dapper
{
    public class NetProUnitOfWorkFactory<DapperDbContext> : IUnitOfWorkFactory<DapperDbContext> where DapperDbContext : DapperContext
    {
        private readonly DapperDbContext _context;

        /// <summary>
        /// Creates a new instance, with context object.
        /// </summary>
        /// <param name="context">The context object.</param>
        public NetProUnitOfWorkFactory(DapperDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates an unit of work.
        /// </summary>
        /// <returns>The unit of work.</returns>
        public IUnitOfWork Create()
        {
            return _context.CreateUnitOfWork();
        }
    }
}
