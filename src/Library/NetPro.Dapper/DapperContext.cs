using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace NetPro.Dapper
{
    /// <summary>
    /// 数据库上下文信息，推荐继承后使用
    /// </summary>
    public class DapperContext : IDisposable
    {
        private IDbConnection _connection;
        private DbConnectionFactory _connectionFactory { get; set; }
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly LinkedList<UnitOfWork> _workItems = new LinkedList<UnitOfWork>();

        /// <summary>
        /// <para>Default constructor.</para>
        /// <para>Uses the <paramref name="connectionStringName"/> to instantiate a <see cref="DbConnectionFactory"/>. This factory will be used to create connections to a database.</para>
        /// </summary>
        /// <param name="connectionStringName">The name of the connectionstring as defined in a app/web.config file's connectionstrings section.</param>
        public DapperContext(string connectionStringName, DataProvider dataProvider, bool miniProfilerEnabled = false)
        {
            DataProvider = dataProvider;
            _connectionFactory = new DbConnectionFactory(connectionStringName, dataProvider, miniProfilerEnabled);
        }

        public void SetTempConnection(string sqlconnection)
        {
            _connectionFactory = new DbConnectionFactory(sqlconnection, DataProvider);
            _connection = _connectionFactory.Create();
        }
        /// <summary>
        /// Creates a new 
        /// </summary>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> used for the transaction inside this unit of work. Default value: <see cref="IsolationLevel.ReadCommitted"/></param>
        /// <returns></returns>
        public virtual UnitOfWork CreateUnitOfWork(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            CreateOrReuseConnection();

            bool wasClosed = _connection.State == ConnectionState.Closed;
            if (wasClosed) _connection.Open();

            try
            {
                UnitOfWork unit;
                IDbTransaction transaction = _connection.BeginTransaction(isolationLevel);

                if (wasClosed)
                    unit = new UnitOfWork(transaction, RemoveTransactionAndCloseConnection, RemoveTransactionAndCloseConnection);
                else
                    unit = new UnitOfWork(transaction, RemoveTransaction, RemoveTransaction);

                _rwLock.EnterWriteLock();
                _workItems.AddLast(unit);
                _rwLock.ExitWriteLock();

                return unit;
            }
            catch
            {
                //Close the connection if we're managing it, and if an exception is thrown when creating the transaction.
                if (wasClosed) _connection.Close();

                throw; //Rethrow the original transaction
            }
        }
        public virtual DataProvider DataProvider { get; set; }
        public virtual IDbConnection Connection
        {
            get
            {
                return CreateOrReuseConnection();
            }
            set
            {
                _connection = value;
            }
        }

        public virtual IDbTransaction ActiveTransaction
        {
            get
            {
                return GetCurrentTransaction();
            }
        }

        public virtual bool IsTransactionStarted => throw new NotImplementedException();

        /// <summary>
        /// Ensures that a connection is ready for querying or creating transactions
        /// </summary>
        /// <remarks></remarks>
        private IDbConnection CreateOrReuseConnection()
        {
            if (_connection != null)
            {
                //这里dapper都会验证一次，为了安全起见，这里也判断一下
                bool wasClosed = _connection.State == ConnectionState.Closed;
                // opens connection
                //如果连接报错，更新sqlclient到4.40版本
                if (wasClosed) _connection.Open();
                return _connection;
            }
            else
            {

                _connection = _connectionFactory.Create();
                return _connection;
            }
        }

        private IDbTransaction GetCurrentTransaction()
        {
            IDbTransaction currentTransaction = null;
            _rwLock.EnterReadLock();
            if (_workItems.Any()) currentTransaction = _workItems.First.Value.Transaction;
            _rwLock.ExitReadLock();

            return currentTransaction;
        }

        private void RemoveTransaction(UnitOfWork workItem)
        {
            _rwLock.EnterWriteLock();
            _workItems.Remove(workItem);
            _rwLock.ExitWriteLock();
        }

        private void RemoveTransactionAndCloseConnection(UnitOfWork workItem)
        {
            _rwLock.EnterWriteLock();
            _workItems.Remove(workItem);
            _rwLock.ExitWriteLock();

            _connection.Close();
        }

        /// <summary>
        /// Implements <see cref="IDisposable.Dispose"/>.
        /// </summary>
        public virtual void Dispose()
        {
            //Use an upgradeable lock, because when we dispose a unit of work,
            //one of the removal methods will be called (which enters a write lock)
            _rwLock.EnterUpgradeableReadLock();
            try
            {
                while (_workItems.Any())
                {
                    var workItem = _workItems.First;
                    workItem.Value.Dispose(); //rollback, will remove the item from the LinkedList because it calls either RemoveTransaction or RemoveTransactionAndCloseConnection
                }
            }
            finally
            {
                _rwLock.ExitUpgradeableReadLock();
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
