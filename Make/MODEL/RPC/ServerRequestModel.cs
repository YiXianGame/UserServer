﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Make.MODEL.RPC
{
    public class ServerRequestModel
    {
        public readonly string JsonRpc;
        public readonly string MethodId;
        public readonly object[] Params;
        public readonly string Service;

        public ServerRequestModel(string jsonrpc,string service,string methodid, object[] @params)
        {
            JsonRpc = jsonrpc;
            MethodId = methodid;
            Params = @params;
            Service = service;
        }
        public override string ToString()
        {
            return "Jsonrpc:" + JsonRpc + "\n"
                + "Service:" + Service + "\n"
                + "Methodid:" + MethodId + "\n"
                + "Params:" + JsonConvert.SerializeObject(Params);
        }
    }
}
