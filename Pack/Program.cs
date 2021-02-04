using Make;
using Make.BLL;
using Make.BLL.Server;
using Make.MODEL;
using Material.MySQL;
using Material.Redis;
using Material.Repository;
using Material.RPC;
using Newtonsoft.Json.Linq;

namespace Pack
{
    class Program
    {
        static void Main(string[] args)
        {
            RPCType type = new RPCType();
            type.Add<int>("int");
            type.Add<string>("string");
            type.Add<bool>("bool");
            type.Add<long>("long");
            type.Add<User>("user", obj => ((JObject)obj).ToObject<User>());
            type.Add<SkillCard>("skillCard", obj => ((JObject)obj).ToObject<User>());
            //服务端
            Initialization initialization = new Initialization();
            //适配远程客户端服务
            RPCAdaptFactory.Register<UserServer>("UserServer", "192.168.0.105", "28015",type);
            //注册远程服务
            Core.UserRequest = RPCRequestProxyFactory.Register<UserClient>("UserClient", "192.168.0.105", "28015",type);
            Redis redis = new Redis("127.0.0.1:6379");
            MySQL mySQL = new MySQL("127.0.0.1","3306","yixian","root","root");
            Repository repository = new Repository(redis, mySQL);
            Core.Repository = repository;
        }
    }
}
