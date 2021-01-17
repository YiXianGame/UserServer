using Make.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Make.MODEL.TCP_Async_Event;
using Make.MODEL.RPC;
using Make.MODEL.RPC.Adapt;
using Make.MODEL.RPC.Request;
using Make;

namespace Pack
{
    class Program
    {
        static void Main(string[] args)
        {
            RPCType type = new RPCType();
            type.Add<int>("int");
            type.Add<string>("string");
            type.Add<bool>("bool");
            //服务端
            Initialization initialization = new Initialization();
            //适配远程客户端服务
            RPCAdaptFactory.Register<User>("User", "192.168.0.105", "28015",type);
            //注册远程服务
            GeneralControl.Command = RPCRequestProxyFactory.Register<ICommand>("Command", "192.168.0.105", "28015",type);
        }
    }
}
