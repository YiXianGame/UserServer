using Make.BLL;
using Make.MODEL.TCP_Async_Event;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL
{
    [JsonObject(MemberSerialization.OptOut)]
    public class User
    {
        #region --字段--
        private string username="";
        private long id;
        private string nickname;
        private int upgrade_num=0;
        private int create_num=0;
        private int money = 0;
        private string information="";
        private string passwords = "";
        private int battle_Count;//战斗场次
        private int exp;//经验
        private int balances;//金钱
        private int lv = 1;//等级
        private string title = "炼气";//称号
        private Enums.User_Active active = Enums.User_Active.Leisure;//玩家当前游戏状态
        private int kills;//击杀数
        private int deaths;//死亡数
        private DateTime registration_date;//注册时间   
        #endregion

        #region --属性--
        
        public string UserName { get => username; set => username = value; }
        public string NickName { get => nickname; set => nickname = value; }
        public string Information { get => information; set => information = value; }
        public int Upgrade_num { get => upgrade_num; set => upgrade_num = value; }
        public int Create_num { get => create_num; set => create_num = value; }
        public int Money { get => money; set => money = value; }
        public string Passwords { get => passwords; set => passwords = value; }
        public int Battle_Count { get => battle_Count; set => battle_Count = value; }
        public int Exp
        {
            get => exp;
            set
            {
                exp = value;
                if (exp < 10)
                {
                    Title = "炼气";
                    Lv = 1;
                }
                else if (exp >= 10)
                {
                    Title = "筑基";
                    Lv = 2;
                }
                else if (exp >= 100)
                {
                    Title = "金丹";
                    Lv = 3;
                }
                else if (exp >= 500)
                {
                    Title = "元婴";
                    Lv = 4;
                }
                else if (exp >= 1000)
                {
                    Title = "分神";
                    Lv = 5;
                }
                else if (exp >= 1500)
                {
                    Title = "洞虚";
                    Lv = 6;
                }
                else if (exp >= 2000)
                {
                    Title = "大乘";
                    Lv = 7;
                }
                else if (exp >= 3000)
                {
                    Lv = 8;
                    Title = "羽化";
                }
            }
        }
        public int Balances { get => balances; set => balances = value; }
        public int Lv { get => lv; set => lv = value; }
        public string Title { get => title; set => title = value; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.User_Active Active { get => active; set => active = value; }
        public int Kills { get => kills; set => kills = value; }
        public int Deaths { get => deaths; set => deaths = value; }
        [JsonIgnore]
        public DateTime Registration_date { get => registration_date; set => registration_date = value; }
        public long ID { get => id; set => id = value; }
        #endregion

        #region --方法--
        public void SendMessages(String message, string bound = null)
        {
            //token.Send(Enums.Msg_Server_Type.Information,message, bound);
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            string filepath = GeneralControl.Directory + "\\用户\\" + UserName + ".json";
            File.WriteAllText(filepath, json);
        }
        public static User Load(long iD)
        {
            string filepath = Material.App.directory + "\\用户\\" + iD.ToString() + ".json";
            if (!File.Exists(filepath)) return null;
            string json = (File.ReadAllText(filepath));
            return JsonConvert.DeserializeObject<User>(json);
        }
        public void Delete()
        {
            string filepath = GeneralControl.Directory + "\\用户\\" + UserName + ".json";
            File.Delete(filepath);
        }
        /// <summary>
        /// 加经验
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool Add_Exp(int value)
        {
            Exp += value;
            Save();
            return true;
        }
        /// <summary>
        /// 加钱
        /// </summary>
        /// <param name="value">金额</param>
        /// <param name="reason">缘由</param>
        /// <param name="isSend">是否发送消息</param>
        /// <returns></returns>
        public bool Add_Balances(int value)
        {
            if (value < 0 && (-value) > Balances)
            {
                return false;
            }
            Balances += value;
            Save();
            return true;
        }


        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="Ex">经验</param>
        /// <param name="Bal">金钱</param>
        /// <param name="reason">理由</param>
        /// <returns></returns>
        public void Settle(int Ex, int Bal)
        {
            Add_Exp(Ex);
            Add_Balances(Bal);
            Battle_Count++;
        }
        /*
        public bool Battle_Skill_Add(SkillCard add_Skill_Card, bool is_save = true)
        {
            if (Battle_SkillCards.TryGetValue(add_Skill_Card.ID, out Simple_SkillCard simple_SkillCard))
            {
                if (add_Skill_Card.Amount < 0 && simple_SkillCard.Amount < -add_Skill_Card.Amount) return false;
                simple_SkillCard.Amount += add_Skill_Card.Amount;
                if (simple_SkillCard.Amount == 0)
                {
                    Battle_SkillCards.Remove(add_Skill_Card.ID);
                }
            }
            else
            {
                if (add_Skill_Card.Amount < 0) return false;
                Battle_SkillCards.Add(add_Skill_Card.ID, new Simple_SkillCard(add_Skill_Card.Name, add_Skill_Card.Level, add_Skill_Card.Amount));
            }
            if (is_save) Save();
            return true;
        }
        public bool Battle_Skill_Add(string add_Skill_id, int number, bool is_save = true)
        {
            if (Battle_SkillCards.TryGetValue(add_Skill_id, out Simple_SkillCard simple_SkillCard))
            {
                if (number < 0 && simple_SkillCard.Amount < -number) return false;
                simple_SkillCard.Amount += number;
                if (simple_SkillCard.Amount == 0)
                {
                    Battle_SkillCards.Remove(add_Skill_id);
                }
            }
            else
            {
                if (number < 0) return false;
                Battle_SkillCards.Add(add_Skill_id, new Simple_SkillCard(simple_SkillCard.Name, simple_SkillCard.Level, simple_SkillCard.Amount));
            }
            if (is_save) Save();
            return true;
        }

        public bool Repository_Skill_Add(SkillCard add_Skill_Card)
        {
            if (Repository_SkillCards.TryGetValue(add_Skill_Card.ID, out Simple_SkillCard simple_SkillCard))
            {
                if (add_Skill_Card.Amount < 0 && simple_SkillCard.Amount < -add_Skill_Card.Amount) return false;
                simple_SkillCard.Amount += add_Skill_Card.Amount;
                if (simple_SkillCard.Amount == 0)
                {
                    Repository_SkillCards.Remove(add_Skill_Card.ID);
                }
            }
            else
            {
                if (add_Skill_Card.Amount < 0) return false;
                Repository_SkillCards.Add(add_Skill_Card.ID, new Simple_SkillCard(add_Skill_Card.Name, add_Skill_Card.Level, add_Skill_Card.Amount));
            }
            return true;
        }
        public bool Repository_Skill_Add(string add_Skill_id, int number, bool is_save = true)
        {
            if (Repository_SkillCards.TryGetValue(add_Skill_id, out Simple_SkillCard simple_SkillCard))
            {
                if (number < 0 && simple_SkillCard.Amount < -number) return false;
                simple_SkillCard.Amount += number;
                if (simple_SkillCard.Amount == 0)
                {
                    Battle_SkillCards.Remove(add_Skill_id);
                }
            }
            else
            {
                if (number < 0) return false;
                Repository_SkillCards.Add(add_Skill_id, new Simple_SkillCard(simple_SkillCard.Name, simple_SkillCard.Level, simple_SkillCard.Amount));
            }
            if (is_save) Save();
            return true;
        }
        */
        /*
        /// <summary>
        /// 检查卡牌版本
        /// </summary>
        /// <returns></returns>
        public string Check_Skill()
        {
            string Messages = "";
            if (SkillCards_Date != GeneralControl.Skill_Card_Date)
            {
                ArrayList remove_List = new ArrayList();
                ArrayList add_List = new ArrayList();
                foreach (KeyValuePair<string, Simple_SkillCard> item in Repository_SkillCards)
                {
                    if (!GeneralControl.Skill_Card_ID_Skllcard.ContainsKey(item.Key))
                    {
                        remove_List.Add(item.Key);
                        Add_Balances(GeneralControl.Menu_GameControl_Class.Instance.Buy_Card_Coast * (int)Math.Pow(4, item.Value.Level - 1) * item.Value.Amount);
                    }
                }
                foreach (string card in remove_List) Repository_SkillCards.Remove(card);
                remove_List.Clear();
                foreach (KeyValuePair<string, Simple_SkillCard> item in Battle_SkillCards)
                {
                    if (!GeneralControl.Skill_Card_ID_Skllcard.ContainsKey(item.Key))
                    {
                        remove_List.Add(item.Key);
                        Add_Balances(GeneralControl.Menu_GameControl_Class.Instance.Buy_Card_Coast * (int)Math.Pow(4, item.Value.Level - 1) * item.Value.Amount);
                    }
                }
                foreach (string card in remove_List) Battle_SkillCards.Remove(card);
                SkillCards_Date = GeneralControl.Skill_Card_Date;
                Save();
            }
            return Messages;
        }
        */
        #endregion
    }
}
