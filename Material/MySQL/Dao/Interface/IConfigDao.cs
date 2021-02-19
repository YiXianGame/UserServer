using Material.Entity;
using Material.Entity.Config;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface IConfigDao
    {
        Task<bool> Insert(UserServerConfig config);
        Task<bool> Delete(UserServerConfig.UserServerCategory category);
        Task<bool> Update(UserServerConfig config);
        Task<UserServerConfig> Query(UserServerConfig.UserServerCategory category);

        Task<PlayerServerConfig> Query(PlayerServerConfig.PlayerServerCategory category);
    }
}
