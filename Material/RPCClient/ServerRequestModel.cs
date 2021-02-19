using Newtonsoft.Json;

namespace Material.RPCClient
{
    public class ServerRequestModel
    {
        public readonly string JsonRpc;
        public readonly string MethodId;
        public object[] Params;
        public readonly string Service;

        public ServerRequestModel(string jsonrpc,string service,string id,string methodid, object[] @params)
        {
            JsonRpc = jsonrpc;
            MethodId = methodid;
            Params = @params;
            Service = service;
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
