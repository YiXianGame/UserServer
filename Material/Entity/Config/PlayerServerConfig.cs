using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace Material.Entity.Config
{
    public class PlayerServerConfig
    {
        #region --Enum--
        [JsonConverter(typeof(StringEnumConverter))]
        public enum PlayerServerCategory { LowPlayerServer, StandardPlayerServer, HighPlayerServer }
        #endregion

        #region --字段--
        private PlayerServerCategory category = PlayerServerCategory.LowPlayerServer;
        string token;
        string ip;
        string port;
        #endregion

        #region --属性--
        public PlayerServerCategory Category { get => category; set => category = value; }
        public string Token { get => token; set => token = value; }
        public string Ip { get => ip; set => ip = value; }
        public string Port { get => port; set => port = value; }

        #endregion
    }
}
