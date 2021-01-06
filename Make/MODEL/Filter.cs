using System.Collections.Generic;
using System.Linq;
namespace Make.MODEL
{
    static public class Filter
    {
        /// <summary>
        /// 以BluePrint技能卡为蓝本，查询所有类似的技能卡
        /// </summary>
        /// <param name="query">被筛选的技能卡</param>
        /// <param name="value">技能卡的等级</param>
        /// <param name="BluePrint">蓝本</param>
        /// <returns></returns>
        public static IEnumerable<SkillCard> SkillCardsModel(List<SkillCard> query, int value,SkillCard BluePrint)
        {
            IEnumerable<SkillCard> result = from SkillCard item in query select item;
            if (BluePrint.Is_Magic)
            {
                result = (from SkillCard item in result
                          where item.Is_Magic == true
                          select item);
            }
            if (BluePrint.Is_Physics && result!=null)
            {
                result = (from SkillCard item in result
                          where item.Is_Physics == true
                          select item);
            }
            if (BluePrint.Is_Cure && result != null)
            {
                result = (from SkillCard item in result
                          where item.Is_Cure == true
                          select item);
            }
            if (BluePrint.Is_Eternal && result != null)
            {
                result = (from SkillCard item in result
                          where item.Is_Eternal == true
                          select item);
            }
            if (BluePrint.Is_Attack && result != null)
            {
                result = (from SkillCard item in result
                          where item.Is_Attack == true
                          select item);
            }
            if (BluePrint.Name!="" && result != null)
            {
                result = (from SkillCard item in result
                          where item.Name.Contains(BluePrint.Name)
                          select item);
            }
            if (BluePrint.State != -1 && result != null)
            {
                result = (from SkillCard item in result
                          where item.State==BluePrint.State
                          select item);
            }
            return result;
        }
        /// <summary>
        /// 根据BluePrint进行奇遇筛选
        /// </summary>
        /// <param name="query">被筛选的奇遇</param>
        /// <param name="BluePrint">蓝本</param>
        /// <returns></returns>
        public static IEnumerable<Adventure> Adventure(List<Adventure> query, Adventure BluePrint)
        {
            IEnumerable<Adventure> result = from Adventure item in query select item;
            if (BluePrint.Name != "" && result != null)
            {
                result = (from Adventure item in result
                          where item.Name.Contains(BluePrint.Name)
                          select item);
            }
            if (BluePrint.State != -1 && result != null)
            {
                result = (from Adventure item in result
                          where item.State == BluePrint.State
                          select item);
            }
            return result;
        }
    }
}
