using System;
using System.Data;

namespace NetPro.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbTransaction _transaction;
        private readonly Action<UnitOfWork> _onCommit;
        private readonly Action<UnitOfWork> _onRollback;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="transaction">The underlying <see cref="IDbTransaction"/> object used to either commit or roll back the statements that are being performed inside this unit of work.</param>
        /// <param name="onCommitOrRollback">An <see cref="Action{NewUnitOfWork}"/> that will be executed when the unit of work is being committed or rolled back.</param>
        public UnitOfWork(IDbTransaction transaction, Action<UnitOfWork> onCommitOrRollback) : this(transaction, onCommitOrRollback, onCommitOrRollback)
        {
        }

        /// <summary>
        /// Creates a new  instance.
        /// </summary>
        /// <param name="transaction">The underlying <see cref="IDbTransaction"/> object used to either commit or roll back the statements that are being performed inside this unit of work.</param>
        /// <param name="onCommit">An <see cref="Action{NewUnitOfWork}"/> that will be executed when the unit of work is being committed.</param>
        /// <param name="onRollback">An <see cref="Action{NewUnitOfWork}"/> that will be executed when the unit of work is being rolled back.</param>
        public UnitOfWork(IDbTransaction transaction, Action<UnitOfWork> onCommit, Action<UnitOfWork> onRollback)
        {
            _transaction = transaction;
            _onCommit = onCommit;
            _onRollback = onRollback;
        }

        /// <summary>
        /// Retrieves the underlying <see cref="IDbTransaction"/> instance.
        /// </summary>
        public IDbTransaction Transaction
        {
            get { return _transaction; }
        }

        /// <summary>
        /// SaveChanges will try and commit all statements that have been executed against the database inside this unit of work.
        /// </summary>
        /// <remarks>
        /// If committing fails, the underlying <see cref="IDbTransaction"/> will be rolled back instead.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if this unit of work has already been committed or rolled back.</exception>
        public void SaveChanges()
        {
            if (_transaction == null)
                throw new InvalidOperationException("This unit of work has already been saved or undone.");

            try
            {
                _transaction.Commit();
                _onCommit(this);
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        /// <summary>
        /// Implements <see cref="IDisposable.Dispose"/>, and rolls back the statements executed inside this unit of work.
        /// This makes it easier to use a unit of work instance inside a <c>using</c> statement (<c>Using</c> in VB.Net).
        /// </summary>
        public void Dispose()
        {
            if (_transaction == null || (_transaction?.Connection == null)) return;

            try
            {
                _transaction.Rollback();
                _onRollback(this);
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
