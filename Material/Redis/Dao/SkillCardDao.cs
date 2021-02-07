using Material.Entity;
using Material.Redis.Dao.Interface;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Material.Redis.Dao
{
    public class SkillCardDao : ISkillCardDao
    {
        IDatabase db;
        public SkillCardDao(IDatabase db)
        {
            this.db = db;
        }

        public void Delete(long id)
        {
            db.HashDeleteAsync("SK" + id, new RedisValue[] { "id","name","description","mp","probability","auxiliary_hp",
                    "auxiliary_mp","enemy_hp","enemy_mp","max_enemy","max_auxiliary","author_id","register_date","update","auxiliary_buff","enemy_buff","category"});
        }

        public async Task<SkillCardBase> Query(long id)
        {
            RedisValue[] values = await db.HashGetAsync("SK" + id, new RedisValue[] { "id","name","description","mp","probability","auxiliary_hp",
                    "auxiliary_mp","enemy_hp","enemy_mp","max_enemy","max_auxiliary","author_id","register_date","update","auxiliary_buff","enemy_buff","category" });
            SkillCardBase skillCard = null;
            if (!values[0].IsNullOrEmpty)
            {
                skillCard = new SkillCardBase();
                skillCard.Id = (long)values[0];
                skillCard.Name = values[1];
                skillCard.Description = values[2];
                skillCard.Mp = (int)values[3];
                skillCard.Probability = (int)values[4];
                skillCard.AuxiliaryHp = (int)values[5];
                skillCard.AuxiliaryMp = (int)values[6];
                skillCard.EnemyHp = (int)values[7];
                skillCard.EnemyMp = (int)values[8];
                skillCard.MaxEnemy = (int)values[9];
                skillCard.MaxAuxiliary = (int)values[10];
                skillCard.AuthorId = (int)values[11];
                skillCard.RegisterDate = (int)values[12];
                skillCard.AttributeUpdate = (int)values[13];
                skillCard.AuxiliaryBuff = JsonConvert.DeserializeObject<List<BuffBase>>(values[14]);
                skillCard.EnemyBuff = JsonConvert.DeserializeObject<List<BuffBase>>(values[15]);
                skillCard.Category = JsonConvert.DeserializeObject<List<SkillCardBase.SkillCardCategory>>(values[16]);
            }
            return skillCard;
        }

        public void Set(SkillCardBase skillCard)
        {
            List<HashEntry> parameters = new List<HashEntry>();
            parameters.Add(new HashEntry("id", skillCard.Id));
            parameters.Add(new HashEntry("name", skillCard.Name));
            parameters.Add(new HashEntry("description", skillCard.Description));
            parameters.Add(new HashEntry("mp", skillCard.Mp));
            parameters.Add(new HashEntry("probability", skillCard.Probability));
            parameters.Add(new HashEntry("auxiliary_hp", skillCard.AuxiliaryHp));
            parameters.Add(new HashEntry("auxiliary_mp", skillCard.AuxiliaryMp));
            parameters.Add(new HashEntry("enemy_hp", skillCard.EnemyHp));
            parameters.Add(new HashEntry("enemy_mp", skillCard.EnemyMp));
            parameters.Add(new HashEntry("max_enemy", skillCard.MaxEnemy));
            parameters.Add(new HashEntry("max_auxiliary", skillCard.MaxAuxiliary));
            parameters.Add(new HashEntry("author_id", skillCard.AuthorId));
            parameters.Add(new HashEntry("register_date", skillCard.RegisterDate));
            parameters.Add(new HashEntry("update", skillCard.AttributeUpdate));
            parameters.Add(new HashEntry("auxiliary_buff", JsonConvert.SerializeObject(skillCard.AuxiliaryBuff)));
            parameters.Add(new HashEntry("enemy_buff", JsonConvert.SerializeObject(skillCard.EnemyBuff)));
            parameters.Add(new HashEntry("category", JsonConvert.SerializeObject(skillCard.Category)));
            db.HashSetAsync("SK" + skillCard.Id, parameters.ToArray());
        }
    }
}
