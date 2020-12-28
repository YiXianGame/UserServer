using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL
{
    public class Msg_Client
    {
        public string Head;
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.Msg_Client_Type Type;
        public string Bound;
        public MsgToken Token;
        public Msg_Client(MsgToken token,Enums.Msg_Client_Type type, string head, string bound = null)
        {
            this.Type = type;
            this.Head = head;
            this.Bound = bound;
            this.Token = token;
        }
        public override string ToString()
        {
            return $"类型:{Type}\n指令:{Head}\n数据:{Bound}";
        }
    }
    
}
