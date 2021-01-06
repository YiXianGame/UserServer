using Make.MODEL.TCP_Async_Event;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Make.MODEL.RPC
{
    public class ClientResponseModel
    {
        public string Jsonrpc = null;
        public object Result = null;
        public Error Error = null;
        public readonly string Id = null;
        public ClientResponseModel(string jsonrpc,string id)
        {
            Jsonrpc = jsonrpc;
            Id = id;
        }
        public ClientResponseModel(string jsonrpc, object result, Error error, string id)
        {
            Jsonrpc = jsonrpc;
            Result = result;
            Error = error;
            Id = id;
        }
        public override string ToString()
        {

            return "Jsonrpc:" + Jsonrpc + "\n"
                + "Id:" + Id + "\n"
                + "Result:" + Result + "\n"
                + "Error:" + Error.ToString();

        }
    }
}
