using System;
using System.Collections.Generic;
using System.Text;

namespace Material.RPCClient
{
    public class RPCNetConfig
    {
        #region --字段--
        private string host;
        private string port;
        private int bufferSize = 1024;
        private bool authorityCheck = false;
        #endregion

        #region --属性--
        public string Host { get => host; set => host = value; }
        public string Port { get => port; set => port = value; }
        public int BufferSize { get => bufferSize; set => bufferSize = value; }
        public bool AuthorityCheck { get => authorityCheck; set => authorityCheck = value; }
        #endregion

        #region --方法--
        public RPCNetConfig(string ip, string port)
        {
            this.host = ip;
            this.port = port;
        }
        #endregion
    }
}
