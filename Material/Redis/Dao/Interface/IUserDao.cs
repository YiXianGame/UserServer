﻿using Material.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Material.Entity.User;

namespace Material.Redis.Dao.Interface
{
    public interface IUserDao
    {
        public void SetAccount(User user);
        public void SetAccount(string username, string password, long id,long attribute_update,long skill_card_update,long head_image_update);
        public void SetState(long id, UserState state, long timestamp);
        public void SetCardGroups(long id, List<CardGroup> cardGroups, long timestamp);
        public Task<long> ValidPerson(long id,string password);
        public Task<User> Query_Attribute(long id, bool has_password = false);

        public Task<long> Query_SkillCardUpdate(long id);

        public Task<long> Query_FriendUpdate(long id);

        public Task<long> Query_CardGroupsUpdate(long id);
        public Task<List<CardGroup>> Query_CardGroups(long id);
    }
}
