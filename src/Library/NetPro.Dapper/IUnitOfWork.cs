using System;
using System.Data;

namespace NetPro.Dapper
{
    /// <summary>
    /// 工作单元接口，用于数据库事务
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 提交dapper事务
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// 获取当前事务(用于自定义控制事务，不推荐！)
        /// </summary>
        IDbTransaction Transaction { get; }
    }
}
