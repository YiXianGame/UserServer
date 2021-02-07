using System;
using System.Collections.Generic;
using System.Text;

namespace Material.ExceptionModel
{
    public class SkillCardException : Exception
    {
        public enum ErrorCode { NotFoundLastIndex}
        public ErrorCode Code { get; set; }
        public string Msg { get; set; }
        public SkillCardException(ErrorCode code,string msg)
        {
            this.Code = code;
            this.Msg = msg;
        }
    }
}
