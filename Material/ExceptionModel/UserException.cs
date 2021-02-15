using System;
using System.Collections.Generic;
using System.Text;

namespace Material.ExceptionModel
{
    public class UserException : Exception
    {
        public enum ErrorCode { NotFoundLastIndex,DataNotFound}
        public ErrorCode Code { get; set; }
        public string Msg { get; set; }
        public UserException(ErrorCode code,string msg)
        {
            this.Code = code;
            this.Msg = msg;
        }
    }
}
