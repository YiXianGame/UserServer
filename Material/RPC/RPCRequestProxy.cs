﻿using Material.TCP_Async_Event;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Material.RPC
{
    public class RPCRequestProxy : DispatchProxy
    {
        private string servicename;
        private Tuple<string, string> key;
        RPCType type;


        public static T Register<T>(string servicename, Tuple<string, string> clientkey, RPCType type)
        {
            RPCRequestProxy proxy = (RPCRequestProxy)(Create<T, RPCRequestProxy>() as object);
            proxy.key = clientkey;
            proxy.servicename = servicename;
            proxy.type = type;
            return (T)(proxy as object);
        }


        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            //这里要连接字符串，发现StringBuilder效率高一些.
            StringBuilder methodid = new StringBuilder(targetMethod.Name);
            ParameterInfo[] parameters = targetMethod.GetParameters();
            object[] obj = null;
            if (parameters.Length > 1)
            {
                obj = new object[parameters.Length - 1];
                for (int i = 1; i < parameters.Length; i++)
                {
                    try
                    {
                        methodid.Append("-" + type.TypeToAbstract[parameters[i].ParameterType]);
                        obj[i - 1] = args[i];
                    }
                    catch (Exception)
                    {
                        throw new RPCException($"C#对应的{parameters[i].ParameterType}类型参数尚未注册");
                    }
                }
            }
            ServerRequestModel request = new ServerRequestModel("2.0", servicename, methodid.ToString(), obj);
            if (args[0].GetType() == typeof(Token))
            {
                (args[0] as Token).Send(request);
            }
            return null;
        }
    }
}