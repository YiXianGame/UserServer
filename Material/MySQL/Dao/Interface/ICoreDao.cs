using Material.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.MySQL.Dao.Interface
{
    public interface ICoreDao
    {
        Task<bool> Insert(CoreBase item);
        Task<bool> Delete(CoreBase.ConfigCategory category);
        Task<bool> Update(CoreBase core);
        Task<CoreBase> Query(CoreBase.ConfigCategory category);
    }
}
