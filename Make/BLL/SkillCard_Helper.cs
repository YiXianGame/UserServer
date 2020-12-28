using Make.MODEL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.BLL
{
    static public class SkillCard_Helper
    {
        
        /// <summary>
        /// 随机返回一张技能卡
        /// </summary>
        /// <returns></returns>
        public static List<SkillCard> Get_Random(int cnt)
        {
            Random random = new Random();
            SkillCard[] skillCards = GeneralControl.Skill_Card_ID_Skllcard.Values.ToArray();
            List<SkillCard> temp=new List<SkillCard>();
            if (skillCards.Length > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    SkillCard skillCard = skillCards[random.Next(0, skillCards.Length - 1)].Clone();
                    skillCard.Amount = 1;
                    temp.Add(skillCard);
                }
                return temp;
            }
            return null;
        }
        public static List<SkillCard> Get_Random(int cnt,int num)
        {
            Random random = new Random();
            SkillCard[] skillCards = GeneralControl.Skill_Card_ID_Skllcard.Values.ToArray();
            List<SkillCard> temp = new List<SkillCard>();
            if (skillCards.Length > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    SkillCard skillCard = skillCards[random.Next(0, skillCards.Length - 1)].Clone();
                    skillCard.Amount = num;
                    temp.Add(skillCard);
                }
                return temp;
            }
            return null;
        }
        public static SkillCardsModel Get_SkillCardsModel_ID(string ID)
        {
            string filepath = Material.App.directory + "\\技能卡\\" + ID + ".json";
            if (!File.Exists(filepath)) return null;
            string json = (File.ReadAllText(filepath));
            return JsonConvert.DeserializeObject<SkillCardsModel>(json);
        }
        /// <summary>
        /// 查找该技能卡并返回该卡的一个实体(已经克隆过的)
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>成功返回具体实例，失败返回 null</returns>
        public static SkillCard Query(string Name)
        {
            if (GeneralControl.Skill_Card_Name_Skllcard.TryGetValue(Name, out SkillCard skillCard))
            {
                return skillCard.Clone();
            }
            else return null;
        }
    }
}
