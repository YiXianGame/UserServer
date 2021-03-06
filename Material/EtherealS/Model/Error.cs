namespace Material.EtherealS.Model
{
    public class Error
    {
        public enum ErrorCode { Intercepted }
        ErrorCode Code { get; set; }
        string Message { get; set; }
        string Data { get; set; }

        public Error(ErrorCode code, string message, string data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public override string ToString()
        {
            return "Code:" + Code 
                + " Message:" + Message 
                + " Data:" + Data + "\n";
        }
    }
}
