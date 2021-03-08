using Material.EtherealS.Annotation;
using Material.EtherealS.Model;
using Material.EtherealS.Service;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Material.EtherealS.Extension.Authority
{
    public class AuthorityCheck
    {
        public static bool ServiceCheck(RPCNetService service, MethodInfo method, BaseUserToken token)
        {
            RPCService annotation = method.GetCustomAttribute<RPCService>();
            if (annotation.Authority != null)
            {
                if ((token as IAuthorityCheck).Check(annotation))
                {
                    return true;
                }
                else return false;
            }
            else if (service.Config.Authoritable)
            {
                if ((token as IAuthorityCheck).Check((IAuthoritable)service.Instance))
                {
                    return true;
                }
                else return false;
            }
            else return true;
        }
    }
}
