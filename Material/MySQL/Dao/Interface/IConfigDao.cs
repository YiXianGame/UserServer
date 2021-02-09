using Material.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface IConfigDao
    {
        Task<bool> Insert(Config config);
        Task<bool> Delete(Config.ConfigCategory category);
        Task<bool> Update(Config config);
        Task<bool> Query(Config.ConfigCategory category);
    }
}
