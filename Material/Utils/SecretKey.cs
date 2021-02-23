using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Utils
{
    public class SecretKey
    {
        private static Random random = new Random(Guid.NewGuid().GetHashCode());
        public static string Generate(int length)
        {
            // 创建一个StringBuilder对象存储密码
            StringBuilder sb = new StringBuilder();
            //使用for循环把单个字符填充进StringBuilder对象里面变成14位密码字符串
            for (int i = 0; i < length; i++)
            {
                //随机选择里面其中的一种字符生成
                switch (random.Next(3))
                {
                    case 0:
                        //调用生成生成随机数字的方法
                        sb.Append(random.Next(10));
                        break;
                    case 1:
                        //调用生成生成随机小写字母的方法
                        sb.Append(Convert.ToChar(random.Next(65, 91)).ToString());
                        break;
                    case 2:
                        //调用生成生成随机大写字母的方法
                        sb.Append(Convert.ToChar(random.Next(97, 123)).ToString());
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
