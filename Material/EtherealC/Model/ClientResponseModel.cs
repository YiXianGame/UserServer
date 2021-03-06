namespace Material.EtherealC.Model
{
    public class ClientResponseModel
    {
        public readonly string JsonRpc = null;
        public readonly object Result = null;
        public readonly string Result_Type = null;
        public readonly Error Error = null;
        public readonly string Id = null;

        public override string ToString()
        {

            return  "Jsonrpc:" + JsonRpc + "\n"
                + "Id:" + Id + "\n"
                + "Result:" + Result + "\n"
                + "Error:" + Error.ToString();
        }
    }
}
