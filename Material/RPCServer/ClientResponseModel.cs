namespace Material.RPCServer
{
    public class ClientResponseModel
    {
        public string JsonRpc = null;
        public object Result = null;
        public Error Error = null;
        public readonly string Id = null;
        public string ResultType;
        public ClientResponseModel(string jsonrpc,string id)
        {
            JsonRpc = jsonrpc;
            Id = id;
        }
        public ClientResponseModel(string jsonrpc, string result,string resultType, Error error, string id)
        {
            JsonRpc = jsonrpc;
            Result = result;
            Error = error;
            Id = id;
            ResultType = resultType;
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
