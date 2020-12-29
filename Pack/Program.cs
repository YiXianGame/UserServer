using Make.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Make.MODEL.TCP_Async_Event;
namespace Pack
{
    class Program
    {
        static void Main(string[] args)
        {
            Initialization initialization = new Initialization();
            TCP_Server tcp_server = new TCP_Server();
            TCP_Server socket_Server = new TCP_Server();//TCP初始化
            TCP_Event.Receive += XY.TCP_Event_Receive;
            Thread thread = new Thread(() => { socket_Server.Init(new string[] {Environment.MachineName, "28015", "1000", "1024"}); });
            thread.Start();
        }
    }
}
