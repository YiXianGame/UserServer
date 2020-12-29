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
        private long qQ = -1;
        private string nickname;
        private int upgrade_num=0;
        private int create_num=0;
        private int money = 0;
        private string information="";
        private string passwords = "";
        private Dictionary<string, Simple_SkillCard> repository_SkillCards = new Dictionary<string, Simple_SkillCard>();//技能卡仓库
        private Dictionary<string, Simple_SkillCard> battle_SkillCards = new Dictionary<string, Simple_SkillCard>();//备战的技能卡
        private int battle_Count;//战斗场次
        private int exp;//经验
        private int balances;//金钱
        private int lv = 1;//等级
        private string title = "炼气";//称号
        private Enums.User_Active active = Enums.User_Active.Leisure;//玩家当前游戏状态
        private int kills;//击杀数
        private int deaths;//死亡数
        private DateTime skillCards_Date;//技能卡版本
        private DateTime registration_date;//注册时间
        private Token token;
        #endregion

        #region --属性--
        [JsonIgnore]
        public Dictionary<string, Simple_SkillCard> Repository_SkillCards { get => repository_SkillCards; set => repository_SkillCards = value; }
        [JsonIgnore]
        public Dictionary<string, Simple_SkillCard> Battle_SkillCards { get => battle_SkillCards; set => battle_SkillCards = value; }
        public string UserName { get => username; set => username = value; }
        public string NickName { get => nickname; set => nickname = value; }
        public string Information { get => information; set => information = value; }
        public long QQ { get => qQ; set => qQ = value; }
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
        [JsonConverter(typeof(EnumJsonConvert<Enums.User_Active>))]
        public Enums.User_Active Active { get => active; set => active = value; }
        public int Kills { get => kills; set => kills = value; }
        public int Deaths { get => deaths; set => deaths = value; }
        [JsonIgnore]
        public DateTime SkillCards_Date { get => skillCards_Date; set => skillCards_Date = value; }
        [JsonIgnore]
        public DateTime Registration_date { get => registration_date; set => registration_date = value; }
        [JsonIgnore]
        public Token Token { get => token; set => token = value; }
        #endregion

        #region --方法--
        public void SendMessages(String message, string bound = null)
        {
            token.Send(Enums.Msg_Server_Type.Information, message, bound);
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            string filepath = GeneralControl.Directory + "\\用户\\" + UserName + ".json";
            File.WriteAllText(filepath, json);
        }
        public static User Load(string iD)
        {
            string filepath = Material.App.directory + "\\用户\\" + iD + ".json";
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
        /// <summary>
        /// 升级卡牌
        /// </summary>
        /// <param name="name">技能卡名</param>
        /// <param name="number">升级的技能卡数量</param>
        /// <returns></returns>
        public bool Upgrate_Skill(string name, int number)
        {
            GeneralControl.Skill_Card_Name_Skllcard.TryGetValue(name, out SkillCard skillCard);
            if (number <= 0) SendMessages("升级技能卡#失败#数量不足");
            if (skillCard.Level < GeneralControl.MaxLevel)
            {
                if (Add_Balances(-(number / 4) * 4))
                {
                    if (Repository_Skill_Add(skillCard.ID, -(number / 4) * GeneralControl.Menu_GameControl_Class.Instance.Upgrade_Card_Coast))
                    {
                        SkillCardsModel father_Skill_Card = SkillCard_Helper.Get_SkillCardsModel_ID(skillCard.Father_ID);
                        SkillCard new_Skill_Card = father_Skill_Card.SkillCards[skillCard.Level + 1].Clone(UserName);
                        new_Skill_Card.Amount = number / 4;
                        Repository_Skill_Add(new_Skill_Card);
                        Save();
                        SendMessages("升级技能卡#成功#升级成功！" + ((number / 4) * 4).ToString() + "张" + name + "升级成为" + number / 4 + "张" + new_Skill_Card.Name);
                        return true;
                    }
                    else SendMessages("升级技能卡#失败#您技能卡数量不足4张");
                    return false;
                }
                else
                {
                    SendMessages($"升级技能卡#失败#您的金额不足以升级卡牌,还需:{(number / 4) * GeneralControl.Menu_GameControl_Class.Instance.Upgrade_Card_Coast - Balances}枚仙域币");
                }
                return false;
            }
            else SendMessages("升级技能卡#失败#该技能卡已为最高等级");
            return false;
        }
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

        #endregion
    }
}
