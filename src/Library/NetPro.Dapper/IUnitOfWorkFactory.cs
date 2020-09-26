namespace NetPro.Dapper
{
    /// <summary>
    /// The interface of factory of unit of work.
    /// </summary>
    public interface IUnitOfWorkFactory<DapperDbContext> where DapperDbContext : DapperContext
    {
        /// <summary>
        /// 创建一个Dapper事务
        /// 业务代码执行完毕,务必执行SaveChanges()方法提交事务！！
        /// 注意：此方法不适用于EF事务，EF事务请使用TransactionScopeBuilder.CreateReadCommitted()创建一个事务
        /// </summary>
        /// <returns>The unit of work.</returns>
        IUnitOfWork Create();
    }
}
