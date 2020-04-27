using NetPro.Core.Configuration;
using NetPro.Core.Infrastructure;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace NetPro.Dapper
{
	public class DbConnectionFactory
	{
		private readonly DbProviderFactory _provider;
		private readonly string _connectionString;

		public DbConnectionFactory(string connectionStringName, DataProvider dataProvider)
		{
			if (connectionStringName == null) throw new ArgumentNullException("connectionStringName");

			_provider = DbProviderFactories.GetFactory(dataProvider);
			_connectionString = connectionStringName;
		}

		/// <summary>
		/// Creates a new instance of <see cref="IDbConnection"/>.
		/// </summary>
		/// <exception cref="ConfigurationErrorsException">Thrown if the connectionstring entry in the app/web.config file is missing information, contains errors or is missing entirely.</exception>
		/// <returns></returns>
		public IDbConnection Create()
		{
			var connection = _provider.CreateConnection();

			connection.ConnectionString = _connectionString;
			bool? miniProfilerEnabled = EngineContext.Current.Resolve<NetProOption>()?.MiniProfilerEnabled;
			if (miniProfilerEnabled.GetValueOrDefault())
			{
				return new StackExchange.Profiling.Data.ProfiledDbConnection(connection, MiniProfiler.Current);
			}
			return connection;
		}
	}
}
