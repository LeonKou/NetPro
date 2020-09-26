using Microsoft.EntityFrameworkCore;

namespace NetPro.EFCore
{
    public abstract class NetProDbContext : DbContext
    {
        public NetProDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
