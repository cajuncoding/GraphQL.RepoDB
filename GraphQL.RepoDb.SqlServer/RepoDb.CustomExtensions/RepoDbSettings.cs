using Microsoft.Data.SqlClient;
using RepoDb.Interfaces;
using System;


namespace RepoDb.CustomExtensions
{ 
    public static class RepoDbSettings
    {
        public static readonly IDbSetting SqlServerSettings = DbSettingMapper.Get<SqlConnection>();
    }
}
