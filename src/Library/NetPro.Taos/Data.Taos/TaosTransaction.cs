// Copyright (c)  maikebing All rights reserved.
//// Licensed under the MIT License, See License.txt in the project root for license information.

using System;
using System.Data;
using System.Data.Common;

namespace Maikebing.Data.Taos
{
    /// <summary>
    ///     Represents a transaction made against a Taos database.
    /// </summary>
    public class TaosTransaction : DbTransaction
    {
        private TaosConnection _connection;
        private readonly IsolationLevel _isolationLevel;
        private bool _completed;
        private bool _externalRollback;

        internal TaosTransaction(TaosConnection connection, IsolationLevel isolationLevel)
        {
             

            _connection = connection;
            _isolationLevel = isolationLevel;

            

            //connection.ExecuteNonQuery(
            //    IsolationLevel == IsolationLevel.Serializable
            //        ? "BEGIN IMMEDIATE;"
            //        : "BEGIN;");
          
        }

        /// <summary>
        ///     Gets the connection associated with the transaction.
        /// </summary>
        /// <value>The connection associated with the transaction.</value>
        public new virtual TaosConnection Connection
            => _connection;

        /// <summary>
        ///     Gets the connection associated with the transaction.
        /// </summary>
        /// <value>The connection associated with the transaction.</value>
        protected override DbConnection DbConnection
            => Connection;

        internal bool ExternalRollback
            => _externalRollback;

        /// <summary>
        ///     Gets the isolation level for the transaction. This cannot be changed if the transaction is completed or
        ///     closed.
        /// </summary>
        /// <value>The isolation level for the transaction.</value>
        public override IsolationLevel IsolationLevel => IsolationLevel.Unspecified;
         

        /// <summary>
        ///     Applies the changes made in the transaction.
        /// </summary>
        public override void Commit()
        {
            //if (_externalRollback || _completed || _connection.State != ConnectionState.Open)
            //{
            //    throw new InvalidOperationException(Resources.TransactionCompleted);
            //}
 
            //_connection.ExecuteNonQuery("COMMIT;");
            Complete();
        }

        /// <summary>
        ///     Reverts the changes made in the transaction.
        /// </summary>
        public override void Rollback()
        {
            //if (_completed || _connection.State != ConnectionState.Open)
            //{
            //    throw new InvalidOperationException(Resources.TransactionCompleted);
            //}

            RollbackInternal();
        }

        /// <summary>
        ///     Releases any resources used by the transaction and rolls it back.
        /// </summary>
        /// <param name="disposing">
        ///     true to release managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing
                && !_completed
                && _connection.State == ConnectionState.Open)
            {
                RollbackInternal();
            }
        }

        private void Complete()
        {
            if (_connection!=null)_connection.Transaction = null;
            _connection = null;
            _completed = true;
        }

        private void RollbackInternal()
        {
            //if (!_externalRollback)
            //{
            //    raw.Taos3_rollback_hook(_connection.Handle, null, null);
            //    _connection.ExecuteNonQuery("ROLLBACK;");
            //}

            Complete();
        }

        private void RollbackExternal(object userData)
        {
            //raw.Taos3_rollback_hook(_connection.Handle, null, null);
            _externalRollback = true;
        }
    }
}
