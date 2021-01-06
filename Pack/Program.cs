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
            //服务端
            Initialization initialization = new Initialization();
            //适配远程客户端服务
            RPCAdaptFactory.Register<User>("User", Environment.MachineName, "28015");
            //注册远程服务
            GeneralControl.Command = RPCRequestProxyFactory.Register<ICommand>("Command", Environment.MachineName, "28015");
        }
    }
}
