using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Entity
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Team
    {
        #region --字段--
        private string name;
        private Dictionary<long, Player> teammates = new Dictionary<long, Player>();
        #endregion

        #region --属性--
        public string Name { get => name; set => name = value; }
        public Dictionary<long, Player> Teammates { get => teammates; set => teammates = value; }
        #endregion
    }
}
