using StackExchange.Profiling;
using System;
using System.Data;
using System.Data.Common;

namespace NetPro.Dapper
{
    public class DbConnectionFactory
    {
        private readonly DbProviderFactory _provider;
        private readonly string _connectionString;
        private readonly bool _miniProfilerEnabled;
        public DbConnectionFactory(string connectionStringName, DataProvider dataProvider, bool miniProfilerEnabled = false)
        {
            if (connectionStringName == null) throw new ArgumentNullException("connectionStringName");

            _provider = DbProviderFactories.GetFactory(dataProvider);
            _connectionString = connectionStringName;
            _miniProfilerEnabled = miniProfilerEnabled;
        }

        /// <summary>
        /// Creates a new instance of <see cref="IDbConnection"/>.
        /// </summary>
        ///Thrown if the connectionstring entry in the app/web.config file is missing information, contains errors or is missing entirely
        /// <returns></returns>
        public IDbConnection Create()
        {
            var connection = _provider.CreateConnection();

            connection.ConnectionString = _connectionString;
            if (_miniProfilerEnabled)
            {
                return new StackExchange.Profiling.Data.ProfiledDbConnection(connection, MiniProfiler.Current);
            }
            return connection;
        }
    }
}
