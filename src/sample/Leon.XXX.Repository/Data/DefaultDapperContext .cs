using NetPro.Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leon.XXX.Repository
{
    /// <summary>
    /// Admin数据库
    /// </summary>
    public class DefaultDapperContext : DapperContext
    {
        public DefaultDapperContext(string connectionStringName, DataProvider dataProvider) : base(connectionStringName, dataProvider)
        {

        }
    }
       //继续追加其他数据库上下文
}
