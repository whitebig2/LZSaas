using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace LZ.Database
{
   public static  class DbContext
    {
        public static ConnectionConfig msConfig = new ConnectionConfig()
        {
            ConnectionString = Common.Tool.GetCommAppsettings<Common.ConnectionString>("MsSQL").MsSQL,
            DbType = DbType.SqlServer,
            InitKeyType = InitKeyType.Attribute,
            IsAutoCloseConnection = true
        };


        public static SqlSugarClient DB
        {
            get => new SqlSugarClient(msConfig);
        }

    }
}
