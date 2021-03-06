using Material.EtherealC.Model;

namespace Material.EtherealC.Service
{
    public class RPCServiceConfig
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
        public RPCServiceConfig(RPCType type)
        {
            this.type = type;
        }

        public RPCServiceConfig(RPCType type, bool tokenEnable)
        {
            this.type = type;
            this.tokenEnable = tokenEnable;
        }

        #endregion
    }
}
