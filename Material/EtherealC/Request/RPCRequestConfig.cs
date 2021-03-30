﻿using Material.EtherealC.Model;

namespace Material.EtherealC.Request
{
    public class RPCRequestConfig
    {
        #region --字段--
        private bool tokenEnable = true;
        private RPCType type;
        private int timeout = 0;
        #endregion

        #region --属性--
        public bool TokenEnable { get => tokenEnable; set => tokenEnable = value; }
        public RPCType Type { get => type; set => type = value; }
        public int Timeout { get => timeout; set => timeout = value; }


        #endregion

        #region --方法--
        public RPCRequestConfig(RPCType type)
        {
            this.type = type;
        }
        #endregion
    }
}
