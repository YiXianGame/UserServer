using System;

namespace Material.ExceptionModel
{
    public class MatchSystemException : Exception
    {
        public enum ErrorCode { NotFoundSucessPool}
        public ErrorCode Code { get; set; }
        public string Msg { get; set; }
        public MatchSystemException(ErrorCode code,string msg)
        {
            this.Code = code;
            this.Msg = msg;
        }
    }
}
