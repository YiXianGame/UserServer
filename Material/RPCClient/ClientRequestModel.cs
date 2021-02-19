using System.Threading;
using Newtonsoft.Json;

namespace Material.RPCClient
{
    public class ClientRequestModel
    {
        [JsonIgnore]
        private AutoResetEvent sign = new AutoResetEvent(false);
        [JsonIgnore]
        public ClientResponseModel Result;
        public readonly string JsonRpc;
        public readonly string MethodId;
        public readonly object[] Params;
        public string Id;
        public readonly string Service;

        public ClientRequestModel(string jsonRpc,string service,string methodId, object[] @params)
        {
            JsonRpc = jsonRpc;
            MethodId = methodId;
            Params = @params;
            Service = service;
        }

        public void set(ClientResponseModel result)
        {
            Result = result;
            sign.Set();
        }
        public ClientResponseModel get()
        {
            //暂停当前进程，等待返回.
            while (Result == null)
            {
                sign.WaitOne();
            }
            return Result;
        }
        public override string ToString()
        {
            return "Jsonrpc:" + JsonRpc + "\n"
                + "Service:" + Service + "\n"
                + "Methodid:" + MethodId + "\n"
                + "Params:" + JsonConvert.SerializeObject(Params);
        }
    }
}
