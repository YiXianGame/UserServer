using Material.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Material.Entity
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Player
    {
        #region --字段--

        private long id;
        private string username;
        private byte[] headImage;
        private string nickname;
        private int lv = 1;//等级
        private string title = "炼气";//称号
        private CardGroup cardGroup;

        #endregion

        #region --属性--
        public long Id { get => id; set => id = value; }
        public CardGroup CardGroup { get => cardGroup; set => cardGroup = value; }
        public string Username { get => username; set => username = value; }
        public byte[] HeadImage { get => headImage; set => headImage = value; }
        public string Nickname { get => nickname; set => nickname = value; }
        public int Lv { get => lv; set => lv = value; }
        public string Title { get => title; set => title = value; }

        #endregion
        #region --方法--
        public void SetAttribute(User user)
        {
            this.id = user.Id;
            this.cardGroup = user.CardGroup;
            this.username = user.Username;
            this.headImage = user.HeadImage;
            this.nickname = user.Nickname;
            this.lv = user.Lv;
            this.title = user.Title;
        }
        #endregion
    }
}
