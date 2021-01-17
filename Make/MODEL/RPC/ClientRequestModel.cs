﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Make.MODEL.RPC
{
    public class ClientRequestModel
    {
        public readonly string JsonRpc;
        public readonly string MethodId;
        public readonly object[] Params;
        public string Id;
        public readonly string Service;

        public ClientRequestModel(string jsonRpc, string service, string methodId, object[] @params)
        {
            JsonRpc = jsonRpc;
            MethodId = methodId;
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
