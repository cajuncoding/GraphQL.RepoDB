using Microsoft.Data.SqlClient;
using RepoDb.Interfaces;

namespace RepoDb.SqlServer
{ 
    public static class RepoDbSettings
    {
        public static readonly IDbSetting SqlServerSettings = DbSettingMapper.Get<SqlConnection>();
    }
}
