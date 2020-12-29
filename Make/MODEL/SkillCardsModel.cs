using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Make.MODEL
{
    /// <summary>
    /// 内含一种技能卡的不同等级
    /// </summary>
    [JsonObject(MemberSerialization.OptOut)]
    public class SkillCardsModel
    {
        #region --字段--
        private SkillCard[] skillCards = new SkillCard[5] { new SkillCard(), new SkillCard(), new SkillCard(), new SkillCard(), new SkillCard() };
        private string iD;
        public SkillCard[] SkillCards { get => skillCards; set => skillCards = value; }
        private string userName;
        public string Cloud { get => cloud; set => cloud = value; }
        private string cloud = "云端";
        #endregion

        #region --属性--
        public string ID { get => iD; set => iD = value; }
        public string UserName { get => userName; set => userName = value; }
        public SkillCardsModel()
        {
            string temp_id;
            do
            {
                temp_id = Guid.NewGuid().ToString();
            }
            while (File.Exists(GeneralControl.Directory + "\\技能卡\\" + temp_id + ".json"));
            ID = temp_id;
        }
        public SkillCardsModel(SkillCard[] Bind)
        {
            skillCards = Bind;
            string temp_id;
            do
            {
                temp_id = Guid.NewGuid().ToString();
            }
            while (File.Exists(GeneralControl.Directory + "\\技能卡\\" + temp_id + ".json"));
            ID = temp_id;
            foreach (SkillCard item in Bind) item.Father_ID = ID;
        }
        public void Assign(SkillCardsModel item)
        {
            iD = item.ID;
            userName = item.userName;
            int cnt = 0;
            foreach(SkillCard skillCard in skillCards)
            {
                skillCard.Assign(item.SkillCards[cnt++]);
            }
        }
        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            string filepath = Material.App.directory + "\\技能卡\\" + ID + ".json";
            File.WriteAllText(filepath, json);
        }
        public void Delete()
        {
            string filepath = Material.App.directory + "\\技能卡\\" + ID + ".json";
            foreach(SkillCard item in skillCards)
            {
                GeneralControl.Skill_Card_Name_Skllcard.Remove(item.Name);
                GeneralControl.Skill_Card_ID_Skllcard.Remove(item.ID);
            }
            GeneralControl.Skill_Cards.Remove(this);
            GeneralControl.Skill_Cards_ID.Remove(ID);
            File.Delete(filepath);
        }
        public void Add_To_General()
        {
            foreach (SkillCard skill in skillCards)
            {
                while ((from string item in GeneralControl.Skill_Card_Name_Skllcard.Keys where item == skill.Name select item).Any()) skill.Name += "-副本";
                while ((from string item in GeneralControl.Skill_Card_ID_Skllcard.Keys where item == skill.ID select item).Any()) skill.ID = Guid.NewGuid().ToString();
                GeneralControl.Skill_Card_Name_Skllcard.Add(skill.Name, skill);
                GeneralControl.Skill_Card_ID_Skllcard.Add(skill.ID, skill);
            }
            GeneralControl.Skill_Cards.Add(this);
            GeneralControl.Skill_Cards_ID.Add(ID, this);
        }
        #endregion
    }
}
