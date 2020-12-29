using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Make.MODEL.TCP_Async_Event
{
    public class Msg_Server
    {
        public string Head;
        [JsonConverter(typeof(StringEnumConverter))]
        public Enums.Msg_Server_Type Type;
        public string Bound;
        public MsgToken Token;
        public void Assign(MsgToken token,Enums.Msg_Server_Type type,string head, string bound = null)
        {
            this.Token = token;
            this.Type = type;
            this.Head = head;
            this.Bound = bound;
        }
        public override string ToString()
        {
            return $"类型:{Type}\n指令:{Head}\n数据:{Bound}";
        }
    }
}
