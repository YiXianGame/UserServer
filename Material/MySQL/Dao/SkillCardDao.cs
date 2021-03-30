using Material.Entity;
using Material.ExceptionModel;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Material.MySQL.Dao
{
    public class SkillCardDao : ISkillCardDao
    {
        string ConnectionString;
        public SkillCardDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection(out MySqlConnection connection)
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
        public async Task<long> Insert(SkillCard skillCard)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "INSERT INTO skillcard (name,description,mp,probability,auxiliary_hp,auxiliary_mp,enemy_hp,enemy_mp,max_enemy,max_auxiliary,author_id,register_date,attribute_update,auxiliary_buff,enemy_buff,category)VALUES(@name,@description,@mp,@probability,@auxiliary_hp,@auxiliary_mp,@enemy_hp,@enemy_mp,@max_enemy,@max_auxiliary,@author_id,@register_date,@attribute_update,@auxiliary_buff,@enemy_buff,@category)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@name", skillCard.Name));
                parameters.Add(new MySqlParameter("@description", skillCard.Description));
                parameters.Add(new MySqlParameter("@mp", skillCard.Mp));
                parameters.Add(new MySqlParameter("@probability", skillCard.Probability));
                parameters.Add(new MySqlParameter("@auxiliary_hp", skillCard.AuxiliaryHp));
                parameters.Add(new MySqlParameter("@auxiliary_mp", skillCard.AuxiliaryMp));
                parameters.Add(new MySqlParameter("@enemy_hp", skillCard.EnemyHp));
                parameters.Add(new MySqlParameter("@enemy_mp", skillCard.EnemyMp));
                parameters.Add(new MySqlParameter("@max_enemy", skillCard.MaxEnemy));
                parameters.Add(new MySqlParameter("@max_auxiliary", skillCard.MaxAuxiliary));
                parameters.Add(new MySqlParameter("@author_id", skillCard.AuthorId));
                parameters.Add(new MySqlParameter("@register_date", skillCard.RegisterDate));
                parameters.Add(new MySqlParameter("@attribute_update", skillCard.AttributeUpdate));
                parameters.Add(new MySqlParameter("@auxiliary_buff", JsonConvert.SerializeObject(skillCard.AuxiliaryBuff)));
                parameters.Add(new MySqlParameter("@enemy_buff", JsonConvert.SerializeObject(skillCard.EnemyBuff)));
                parameters.Add(new MySqlParameter("@category", skillCard.Category.ToString()));
                await MySqlHelper.ExecuteNonQueryAsync(connection,sqlcommand,parameters.ToArray());
                return await QueryLastInsertId(connection);
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<bool> Update(SkillCard skillCard)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                string sqlcommand = "UPDATE skillcard SET name=@name,description=@description,mp=@mp,probability=@probability,auxiliary_hp=@auxiliary_hp," +
                    "auxiliary_mp=@auxiliary_mp,enemy_hp=@enemy_hp,enemy_mp=@enemy_mp,max_enemy=@max_enemy,max_auxiliary=@max_auxiliary," +
                    "author_id=@author_id,register_date=@register_date,attribute_update=@attribute_update,auxiliary_buff=@auxiliary_buff,enemy_buff=@enemy_buff,category=@category WHERE id = @id)";
                List<MySqlParameter> parameters = new List<MySqlParameter>();
                parameters.Add(new MySqlParameter("@id", skillCard.Id));
                parameters.Add(new MySqlParameter("@name", skillCard.Name));
                parameters.Add(new MySqlParameter("@description", skillCard.Description));
                parameters.Add(new MySqlParameter("@mp", skillCard.Mp));
                parameters.Add(new MySqlParameter("@probability", skillCard.Probability));
                parameters.Add(new MySqlParameter("@auxiliary_hp", skillCard.AuxiliaryHp));
                parameters.Add(new MySqlParameter("@auxiliary_mp", skillCard.AuxiliaryMp));
                parameters.Add(new MySqlParameter("@enemy_hp", skillCard.EnemyHp));
                parameters.Add(new MySqlParameter("@enemy_mp", skillCard.EnemyMp));
                parameters.Add(new MySqlParameter("@max_enemy", skillCard.MaxEnemy));
                parameters.Add(new MySqlParameter("@max_auxiliary", skillCard.MaxAuxiliary));
                parameters.Add(new MySqlParameter("@author_id", skillCard.AuthorId));
                parameters.Add(new MySqlParameter("@register_date", skillCard.RegisterDate));
                parameters.Add(new MySqlParameter("@attribute_update", skillCard.AttributeUpdate));
                parameters.Add(new MySqlParameter("@auxiliary_buff", skillCard.AuxiliaryBuff));
                parameters.Add(new MySqlParameter("@enemy_buff", skillCard.EnemyBuff));
                parameters.Add(new MySqlParameter("@category", skillCard.Category.ToString()));
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, sqlcommand, parameters.ToArray());
                if (result == 1) return true;
                else return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<bool> Delete(long id)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, $"DELETE FROM skillcard WHERE id = '{id}'");
                if (result == 1) return true;
                else return false;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<SkillCard> Query(long id)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT * FROM skillcard WHERE id = {id}");
                if (reader.Read())
                {
                    SkillCard skillCard = new SkillCard();
                    skillCard.Id = reader.GetInt64("id");
                    skillCard.Name = reader.GetString("name");
                    skillCard.Description = reader.GetString("description");
                    skillCard.Mp = reader.GetInt32("mp");
                    skillCard.Probability = reader.GetInt32("probability");
                    skillCard.AuxiliaryHp = reader.GetInt32("auxiliary_hp");
                    skillCard.AuxiliaryMp = reader.GetInt32("auxiliary_mp");
                    skillCard.EnemyHp = reader.GetInt32("enemy_hp");
                    skillCard.EnemyMp = reader.GetInt32("enemy_mp");
                    skillCard.MaxEnemy = reader.GetInt32("max_enemy");
                    skillCard.MaxAuxiliary = reader.GetInt32("max_auxiliary");
                    skillCard.AuthorId = reader.GetInt64("author_id");
                    skillCard.RegisterDate = reader.GetInt64("register_date");
                    skillCard.AttributeUpdate = reader.GetInt64("attribute_update");
                    skillCard.AuxiliaryBuff = JsonConvert.DeserializeObject<List<Buff>>(reader.GetString("auxiliary_buff"));
                    skillCard.EnemyBuff = JsonConvert.DeserializeObject<List<Buff>>(reader.GetString("enemy_buff"));
                    skillCard.Category = (SkillCard.SkillCardCategory)Enum.Parse(typeof(SkillCard.SkillCardCategory),reader.GetString("category"));
                    return skillCard;
                }
                else return null;
            }
            finally
            {
                connection.Close();
            }
        }

        private async Task<long> QueryLastInsertId(MySqlConnection connection)
        {
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT LAST_INSERT_ID()");
                if (reader.Read())
                {
                    return reader.GetInt64(0);
                }
                throw new SkillCardException(SkillCardException.ErrorCode.NotFoundLastIndex, "找不到自增主键的ID值");
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<List<SkillCard>> Query_All()
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, "SELECT * FROM skillcard");
                List<SkillCard> skillCards = new List<SkillCard>();
                while (reader.Read())
                {
                    SkillCard skillCard = new SkillCard();
                    skillCard.Id = reader.GetInt64("id");
                    skillCard.Name = reader.GetString("name");
                    skillCard.Description = reader.GetString("description");
                    skillCard.Mp = reader.GetInt32("mp");
                    skillCard.Probability = reader.GetInt32("probability");
                    skillCard.AuxiliaryHp = reader.GetInt32("auxiliary_hp");
                    skillCard.AuxiliaryMp = reader.GetInt32("auxiliary_mp");
                    skillCard.EnemyHp = reader.GetInt32("enemy_hp");
                    skillCard.EnemyMp = reader.GetInt32("enemy_mp");
                    skillCard.MaxEnemy = reader.GetInt32("max_enemy");
                    skillCard.MaxAuxiliary = reader.GetInt32("max_auxiliary");
                    skillCard.AuthorId = reader.GetInt64("author_id");
                    skillCard.RegisterDate = reader.GetInt64("register_date");
                    skillCard.AttributeUpdate = reader.GetInt64("attribute_update");
                    skillCard.AuxiliaryBuff = JsonConvert.DeserializeObject<List<Buff>>(reader.GetString("auxiliary_buff"));
                    skillCard.EnemyBuff = JsonConvert.DeserializeObject<List<Buff>>(reader.GetString("enemy_buff"));
                    skillCard.Category = (SkillCard.SkillCardCategory)Enum.Parse(typeof(SkillCard.SkillCardCategory), reader.GetString("category"));
                    skillCards.Add(skillCard);
                }
                return skillCards;
            }
            finally
            {
                connection.Close();
            }
        }

        public SkillCard QuerySync(long id)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = MySqlHelper.ExecuteReader(connection, $"SELECT * FROM skillcard WHERE id = {id}");
                if (reader.Read())
                {
                    SkillCard skillCard = new SkillCard();
                    skillCard.Id = reader.GetInt64("id");
                    skillCard.Name = reader.GetString("name");
                    skillCard.Description = reader.GetString("description");
                    skillCard.Mp = reader.GetInt32("mp");
                    skillCard.Probability = reader.GetInt32("probability");
                    skillCard.AuxiliaryHp = reader.GetInt32("auxiliary_hp");
                    skillCard.AuxiliaryMp = reader.GetInt32("auxiliary_mp");
                    skillCard.EnemyHp = reader.GetInt32("enemy_hp");
                    skillCard.EnemyMp = reader.GetInt32("enemy_mp");
                    skillCard.MaxEnemy = reader.GetInt32("max_enemy");
                    skillCard.MaxAuxiliary = reader.GetInt32("max_auxiliary");
                    skillCard.AuthorId = reader.GetInt64("author_id");
                    skillCard.RegisterDate = reader.GetInt64("register_date");
                    skillCard.AttributeUpdate = reader.GetInt64("attribute_update");
                    skillCard.AuxiliaryBuff = JsonConvert.DeserializeObject<List<Buff>>(reader.GetString("auxiliary_buff"));
                    skillCard.EnemyBuff = JsonConvert.DeserializeObject<List<Buff>>(reader.GetString("enemy_buff"));
                    skillCard.Category = (SkillCard.SkillCardCategory)Enum.Parse(typeof(SkillCard.SkillCardCategory), reader.GetString("category"));
                    return skillCard;
                }
                else return null;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
