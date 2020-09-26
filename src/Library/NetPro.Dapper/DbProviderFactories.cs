using Microsoft.Data.SqlClient;
using NetPro.Dapper.Expressions;
using System;
using System.Data.Common;

namespace NetPro.Dapper
{
    public static class DbProviderFactories
    {
        public static DbProviderFactory GetFactory(DataProvider dataProvider)
        {
            switch (dataProvider)
            {
                case DataProvider.SqlServer:
                    DapperSmart.SetDialect(DapperSmart.Dialect.SQLServer);
                    return SqlClientFactory.Instance;

                case DataProvider.Mysql:
                    DapperSmart.SetDialect(DapperSmart.Dialect.MySQL);
                    return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
                default:
                    throw new Exception("Database Version NO MACH");
            }
            throw new Exception("Database Version NO MACH");
        }
    }
}
