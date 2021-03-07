namespace Material.EtherealC.Net
{
    public class RPCNetConfig
    {
        #region --字段--
        private int bufferSize = 1024;
        #endregion

        #region --属性--
        public int BufferSize { get => bufferSize; set => bufferSize = value; }
        #endregion

        #region --方法--
        public RPCNetConfig(int bufferSize)
        {
            this.bufferSize = bufferSize;
        }

        #endregion
    }
}
