﻿using Material.Entity;
using Material.ExceptionModel;
using Material.MySQL.Dao.Interface;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Material.MySQL.Dao
{
    public class UserDao : IUserDao
    {
        string ConnectionString;
        public UserDao(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        public MySqlConnection GetConnection(out MySqlConnection connection)
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
        public async Task<long> Insert(string username, string nickname, string password)
        {
            GetConnection(out MySqlConnection connection);
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数
            try
            {
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, $"INSERT INTO user(username,nickname,password,register_date,attribute_update,state,card_groups) VALUES ('{username}','{nickname}','{password}','{timeStamp}','{timeStamp}','{User.UserState.Offline}','{""}')");
                if (result == 1)
                {
                    return await Query_LastInsertId(connection);
                }
                else return -1;
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<User> Query_UserByUsername(string username)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id,nickname,password,upgrade_num,create_num,money,personal_signature," +
                    $"battle_count,exp,lv,title,state,kills,deaths,register_date,attribute_update,card_repository_update,head_image_update,friend_update,card_groups_update,card_groups FROM user WHERE username='{username}'");
                User user = null;
                if (reader.Read())
                {   
                    user = new User();
                    user.Id = reader.GetInt64("id");
                    user.Username= username;
                    user.Nickname = reader.GetString("nickname");
                    user.Password = reader.GetString("password");
                    user.Upgrade_num = reader.GetInt32("upgrade_num");
                    user.Create_num = reader.GetInt32("create_num");
                    user.Money = reader.GetInt32("money");
                    user.PersonalSignature= reader.GetString("personal_signature");
                    user.BattleCount = reader.GetInt32("battle_count");
                    user.Exp = reader.GetInt64("exp");
                    user.Lv  = reader.GetInt32("lv");
                    user.Title = reader.GetString("title");
                    user.State = (User.UserState)Enum.Parse(typeof(User.UserState), reader.GetString("state"));
                    user.Kills = reader.GetInt32("kills");
                    user.Deaths = reader.GetInt32("deaths");
                    user.RegisterDate= reader.GetInt64("register_date");
                    user.Attribute_update= reader.GetInt64("attribute_update");
                    user.CardRepository_update = reader.GetInt64("card_repository_update");
                    user.HeadImage_update = reader.GetInt64("head_image_update");
                    user.Friend_update = reader.GetInt64("friend_update");
                    user.CardGroups = JsonConvert.DeserializeObject<List<CardGroup>>(reader.GetString("card_groups"));
                    if (user.CardGroups == null) user.CardGroups = new List<CardGroup>();
                    user.CardGroups_update = reader.GetInt64("card_groups_update");
                }
                return user;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<User> Query_UserByID(long id,bool has_password = false)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT username,nickname,password,upgrade_num,create_num,money,personal_signature," +
                    $"battle_count,exp,lv,title,state,kills,deaths,register_date,attribute_update,card_repository_update,head_image_update,friend_update,card_groups_update,card_groups FROM user WHERE id={id}");
                User user = null;
                if (reader.Read())
                {
                    user = new User();
                    user.Id = id;
                    user.Username = reader.GetString("username");
                    user.Nickname = reader.GetString("nickname");
                    user.Password = reader.GetString("password");
                    user.Upgrade_num = reader.GetInt32("upgrade_num");
                    user.Create_num = reader.GetInt32("create_num");
                    user.Money = reader.GetInt32("money");
                    user.PersonalSignature = reader.GetString("personal_signature");
                    user.BattleCount = reader.GetInt32("battle_count");
                    user.Exp = reader.GetInt64("exp");
                    user.Lv = reader.GetInt32("lv");
                    user.Title = reader.GetString("title");
                    user.State = (User.UserState)Enum.Parse(typeof(User.UserState), reader.GetString("state"));
                    user.Kills = reader.GetInt32("kills");
                    user.Deaths = reader.GetInt32("deaths");
                    user.RegisterDate = reader.GetInt64("register_date");
                    user.Attribute_update = reader.GetInt64("attribute_update");
                    user.CardRepository_update = reader.GetInt64("card_repository_update");
                    user.HeadImage_update = reader.GetInt64("head_image_update");
                    user.Friend_update = reader.GetInt64("friend_update");
                    user.CardGroups = JsonConvert.DeserializeObject<List<CardGroup>>(reader.GetString("card_groups"));
                    if (user.CardGroups == null) user.CardGroups = new List<CardGroup>();
                    user.CardGroups_update = reader.GetInt64("card_groups_update");
                }
                return user;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<bool> Update_NickName(long id,string nickname)
        {
            int result = await MySqlHelper.ExecuteNonQueryAsync(GetConnection(out MySqlConnection connection), $"UPDATE user SET nickname='{nickname}' WHERE id='{id}'");
            if (result == 1) return true;
            else return false;
        }   
        public async Task<bool> Update_CardGroups(long id, List<CardGroup> cardGroup,long timestamp)
        {
            int result = await MySqlHelper.ExecuteNonQueryAsync(GetConnection(out MySqlConnection connection), $"UPDATE user SET card_groups='{JsonConvert.SerializeObject(cardGroup)}',card_groups_update = '{timestamp}' WHERE id='{id}'");
            if (result == 1) return true;
            else return false;
        }
        public async Task<bool> Update_Password(long id, string password)
        {
            int result = await MySqlHelper.ExecuteNonQueryAsync(GetConnection(out MySqlConnection connection), $"UPDATE user SET password='{password}' WHERE id='{id}'");
            if (result == 1) return true;
            else return false;
        }


        public async Task<long> Query_IdByUsername(string username)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                MySqlDataReader result = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT id FROM user WHERE username='{username}'");
                if (result.Read())
                {
                    return result.GetInt64(0);
                }
                else return -1;
            }
            finally
            {
                connection.Close();
            }
        }
        private async Task<long> Query_LastInsertId(MySqlConnection connection)
        {
            try
            {
                MySqlDataReader reader = await MySqlHelper.ExecuteReaderAsync(connection, $"SELECT LAST_INSERT_ID()");
                if (reader.Read())
                {
                    return reader.GetInt64(0);
                }
                throw new UserException(UserException.ErrorCode.NotFoundLastIndex, "找不到自增主键的ID值");
            }
            finally
            {
                connection.Close();
            }
        }
        public async Task<bool> Update_State(long id, User.UserState state, long timestamp)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, $"UPDATE user SET state='{state.ToString()}',attribute_update = '{timestamp}' WHERE id='{id}'");
                if (result == 1) return true;
                else return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public async Task<bool> Update_CardRepositoryUpdate(long id, long timestamp)
        {
            GetConnection(out MySqlConnection connection);
            try
            {
                int result = await MySqlHelper.ExecuteNonQueryAsync(connection, $"UPDATE user SET card_repository_update='{timestamp}' WHERE id='{id}'");
                if (result == 1) return true;
                else return false;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
