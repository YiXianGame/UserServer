using Make.MODEL;
using System;
using System.Collections.Generic;
using System.Linq;

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
            SkillCard[] skillCards = Core.Skill_Card_ID_Skllcard.Values.ToArray();
            List<SkillCard> temp=new List<SkillCard>();
            if (skillCards.Length > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    SkillCard skillCard = skillCards[random.Next(0, skillCards.Length - 1)].Clone();
                    temp.Add(skillCard);
                }
                return temp;
            }
            return null;
        }
        public static List<SkillCard> Get_Random(int cnt,int num)
        {
            Random random = new Random();
            SkillCard[] skillCards = Core.Skill_Card_ID_Skllcard.Values.ToArray();
            List<SkillCard> temp = new List<SkillCard>();
            if (skillCards.Length > 0)
            {
                for (int i = 0; i < cnt; i++)
                {
                    SkillCard skillCard = skillCards[random.Next(0, skillCards.Length - 1)].Clone();
                    temp.Add(skillCard);
                }
                return temp;
            }
            return null;
        }

        /// <summary>
        /// 查找该技能卡并返回该卡的一个实体(已经克隆过的)
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>成功返回具体实例，失败返回 null</returns>
        public static SkillCard Query(string Name)
        {
            if (Core.Skill_Card_Name_Skllcard.TryGetValue(Name, out SkillCard skillCard))
            {
                return skillCard.Clone();
            }
            else return null;
        }
    }
}
