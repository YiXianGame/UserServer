using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Make.MODEL.RPC
{
    public class ClientRequestModel
    {
        public readonly string Jsonrpc;
        public readonly string Methodid;
        public readonly object[] Params;
        public string ID;
        public readonly string Service;

        public ClientRequestModel(string jsonrpc,string service,string id,string methodid, object[] @params)
        {
            Jsonrpc = jsonrpc;
            ID = id;
            Methodid = methodid;
            Params = @params;
            Service = service;
        }
        public override string ToString()
        {
            return "Jsonrpc:" + Jsonrpc + "\n"
                + "Service:" + Service + "\n"
                + "Methodid:" + Methodid + "\n"
                + "Params:" + JsonConvert.SerializeObject(Params);
        }

    }
}
