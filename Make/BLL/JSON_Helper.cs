using Make.MODEL;
using System;

namespace Make.BLL
{
    public static class JSON_Helper
    {
        public static string Personal_Information(User user)
        {
            string msg = $"{{ \"app\": \"com.tencent.miniapp\", \"desc\":\"\", \"view\":\"notification\", \"prompt\": \"【来自仙域的诏令】\", " +
                $" \"meta\": {{ \"notification\":{{ \"appInfo\":{{ \"appName\": \"{"仙战-仙域篇"}\", \"appType\": 4, \"appid\": 1109659848, " +
                $"\"iconUrl\": \"http://dl.4177.com/hgame_album/180131/082d2384810d60a5d4fc9e5a1cd4ede8aeff0950.082d2384810d60a5d4fc9e5a1cd4ede8aeff0950.png\" }}," +
                $"\"data\": [ " +
                $"{{ \"title\": \"玩家名\",  \"value\": \"{user.NickName}\" }}," +
                $"{{ \"title\": \"境界\",\"value\": \"{user.Title}\" }}," +
                $"{{ \"title\": \"经验\", \"value\": \"{user.Exp}\" }}," +
                $"{{ \"title\": \"战斗场次\", \"value\": \"{user.BattleCount}\" }}," +
                $"], \"emphasis_keyword\": \"\",\"title\": \"个人信息\"}}  }} }}";
            return msg;
        }
        public static string Hand_Skill_Show(Player player)
        {
            string msg = $"{{\"app\": \"com.tencent.miniapp\",\"desc\": \"\",\"view\": \"notification\",\"ver\": \"0.0.0.1\",\"prompt\": \"【来自仙域的一道诏令】\",\"appID\": \"\",\"sourceName\": \"\",\"actionData\": \"\",\"actionData_A\": \"\",\"sourceUrl\": \"\",\"meta\": {{\"notification\": {{\"appInfo\": {{\"appName\": \"【仙技】\",\"appType\": 4,\"ext\": \"\",\"img\": \"\",\"img_s\": \"\",\"appid\": 1108249016,\"iconUrl\": \"http://dl.4177.com/hgame_album/180131/082d2384810d60a5d4fc9e5a1cd4ede8aeff0950.082d2384810d60a5d4fc9e5a1cd4ede8aeff0950.png\"}}, \"button\": [";
            foreach (SkillCard skillCard in player.Hand_SkillCards.Values)
            {
                try
                {
                    string state_temp = "";
                    if (skillCard.Buff.Count > 0) state_temp = " 状态:";
                    foreach (State state in skillCard.Buff)
                    {
                        state_temp += "（" + state.Name + ")能力:" + state.Effect_mp + " 回合:" + state.Duration_Round;
                    }
                    msg += $"{{\"action\": \"https:\\/\\/www.baidu.com\\/\", \"name\": \"{"" + skillCard.Name + "-" + skillCard.Need_Mp + "[血]伤:" + skillCard.Attack + "疗:" + skillCard.Cure + " [气]损:" + skillCard.Direct_Mp + "恢:" + skillCard.Self_Mp }\"}},";
                }
                catch (Exception e)
                {
                    
                }         
            }
            msg += $"{{\"action\": \"\",\"name\": \"您当前仙气:{player.Mp}\"}}],\"emphasis_keyword\": \"\"}}}},\"text\": \"\",\"sourceAd\": \"\"}}";
            return msg;         
        }
    }
}
