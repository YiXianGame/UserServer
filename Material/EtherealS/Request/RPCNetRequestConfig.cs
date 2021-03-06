using Material.EtherealS.Model;

namespace Material.EtherealS.Request
{
    public class RPCNetRequestConfig
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
        public RPCNetRequestConfig(RPCType type)
        {
            this.type = type;
        }
        #endregion
    }
}
