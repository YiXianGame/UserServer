using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Entity
{
    public class UserBase
    {
        #region --Enum--
        public enum State { Leisure, Ready, Queue, Gaming, Offline };
        #endregion

        #region --字段--

        protected internal long id;

        protected internal string username;

        protected internal byte[] headImage;

        protected internal string nickname;

        protected internal int upgrade_num = 0;

        protected internal int create_num = 0;

        protected internal long money = 0;

        protected internal string personalSignature;

        protected internal int battleCount;//战斗场次

        protected internal long exp;//经验

        protected internal int lv = 1;//等级

        protected internal string title = "炼气";//称号

        protected internal State active = State.Offline;//玩家当前游戏状态

        protected internal int kills;//击杀数

        protected internal int deaths;//死亡数

        protected internal long registerDate;//注册日期

        protected internal long attribute_update;//个人信息更新日期

        protected internal long skillCard_update;//卡牌更新日期

        protected internal long headImage_update;//头像更新日期

        #endregion
    }
}
