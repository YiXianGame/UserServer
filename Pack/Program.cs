using Make;
using Make.BLL;
using Make.MODEL.Server;
using Material.MySQL;
using Material.Redis;
using Material.RPC;
using Material.Repository;
using Make.MODEL;

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
            type.Add<User>("user");
            type.Add<SkillCard>("skillCard");
            //服务端
            Initialization initialization = new Initialization();
            //适配远程客户端服务
            RPCAdaptFactory.Register<UserServer>("User", "192.168.0.105", "28015",type);
            //注册远程服务
            Core.UserRequest = RPCRequestProxyFactory.Register<UserRequest>("UserCommand", "192.168.0.105", "28015",type);
            Redis redis = new Redis("127.0.0.1:6379");
            MySQL mySQL = new MySQL("127.0.0.1:3306","yixian","root","root");
            Repository repository = new Repository(redis, mySQL);
            Core.Repository = repository;
        }
    }
}
