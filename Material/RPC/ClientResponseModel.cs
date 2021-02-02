using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Material.RPC
{
    public class ClientResponseModel
    {
        public string JsonRpc = null;
        public object Result = null;
        public Error Error = null;
        public readonly string Id = null;
        public ClientResponseModel(string jsonrpc,string id)
        {
            JsonRpc = jsonrpc;
            Id = id;
        }
        public ClientResponseModel(string jsonrpc, object result, Error error, string id)
        {
            JsonRpc = jsonrpc;
            Result = result;
            Error = error;
            Id = id;
        }
        public override string ToString()
        {

            return "Jsonrpc:" + JsonRpc + "\n"
                + "Id:" + Id + "\n"
                + "Result:" + Result + "\n"
                + "Error:" + Error.ToString();

        }
    }
}
