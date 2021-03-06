using System;
using System.Collections.Generic;
using System.Text;

namespace Material.RPCServer
{
    public class RPCRequestConfig
    {
        #region --字段--
        private bool tokenEnable;
        private RPCType type;
        #endregion

        #region --属性--
        public bool TokenEnable { get => tokenEnable; set => tokenEnable = value; }
        public RPCType Type { get => type; set => type = value; }
        #endregion

        #region --方法--
        public RPCRequestConfig(RPCType type)
        {
            this.type = type;
        }
        #endregion
    }
}
