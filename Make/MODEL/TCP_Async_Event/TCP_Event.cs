using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL.TCP_Async_Event
{
    public static class TCP_Event
    {
        public delegate void ReceiveDelegate(Token token, Msg_Client msg_Client);
        public static event ReceiveDelegate Receive;
        public static void OnReceive(Token token,Msg_Client receiveStr)
        {
            Receive(token, receiveStr);
        }
    }
}
