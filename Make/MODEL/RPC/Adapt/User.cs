using Make.MODEL.TCP_Async_Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.MODEL.RPC.Adapt
{
    public class User
    {
        public static string hello(Token token, string msg)
        {
            //调用远程客户端服务 进行减血动画操作
            GeneralControl.Command.AddHp(token, 23);
            //返回远程客户端请求内容
            return msg + "你好，很高兴为您服务！";
        }
    }
}
