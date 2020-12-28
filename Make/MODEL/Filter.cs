using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
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
        public static IEnumerable<SkillCardsModel> SkillCardsModel(List<SkillCardsModel> query, int value,SkillCard BluePrint)
        {
            IEnumerable<SkillCardsModel> result = from SkillCardsModel item in query select item;
            if (BluePrint.Is_Magic)
            {
                result = (from SkillCardsModel item in result
                          where item.SkillCards[value].Is_Magic == true
                          select item);
            }
            if (BluePrint.Is_Physics && result!=null)
            {
                result = (from SkillCardsModel item in result
                          where item.SkillCards[value].Is_Physics == true
                          select item);
            }
            if (BluePrint.Is_Cure && result != null)
            {
                result = (from SkillCardsModel item in result
                          where item.SkillCards[value].Is_Cure == true
                          select item);
            }
            if (BluePrint.Is_Eternal && result != null)
            {
                result = (from SkillCardsModel item in result
                          where item.SkillCards[value].Is_Eternal == true
                          select item);
            }
            if (BluePrint.Is_Attack && result != null)
            {
                result = (from SkillCardsModel item in result
                          where item.SkillCards[value].Is_Attack == true
                          select item);
            }
            if (BluePrint.Name!="" && result != null)
            {
                result = (from SkillCardsModel item in result
                          where item.SkillCards[value].Name.Contains(BluePrint.Name)
                          select item);
            }
            if (BluePrint.State != -1 && result != null)
            {
                result = (from SkillCardsModel item in result
                          where item.SkillCards[value].State==BluePrint.State
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
        /// <summary>
        /// 查询出生卡组
        /// </summary>
        /// <param name="query">被筛选的卡</param>
        /// <returns></returns>
        public static IEnumerable<SkillCardsModel> Basic_SkillCardsModel(List<SkillCardsModel> query)
        {
            List<SkillCardsModel> result= new List<SkillCardsModel>();
            foreach(SkillCardsModel item in from SkillCardsModel item in query select item)
            {
                if((from SkillCard skillcard in item.SkillCards where skillcard.Is_Basic select skillcard).Any())
                {
                    result.Add(item);
                }
            }
            return (from SkillCardsModel item in result select item);
        }
    }
}
