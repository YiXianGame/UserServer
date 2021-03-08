using Material.Entity;
using Material.EtherealS.Annotation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Make.RPCServer.Request
{
    public interface EquipRequest
    {
        [RPCRequest]
        public void SwitchCardGroup(User user, long id, CardGroup cardGroup);
        [RPCRequest]
        public void ConnectPlayerServer(User user, string ip, string port);
    }
}
