using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Entity
{
    [JsonObject(MemberSerialization.OptOut)]
    public class CardGroup
    {
        #region --字段--
        string name;
        List<Tuple<long, string>> cards = new List<Tuple<long, string>>();
        #endregion

        #region --属性--
        public string Name { get => name; set => name = value; }
        public List<Tuple<long, string>> Cards { get => cards; set => cards = value; }
        #endregion
    }
}
