using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Make.BLL
{
    public static class  BinarySerialize
    {
        /// <summary>
        /// 将对象序列化为byte[]
        /// 使用IFormatter的Serialize序列化
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns>序列化获取的二进制流</returns>
        public static byte[] FormatterObjectBytes(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            byte[] buff;
            try
            {
                using (var ms = new MemoryStream())
                {
                    IFormatter iFormatter = new BinaryFormatter();
                    iFormatter.Serialize(ms, obj);
                    buff = ms.GetBuffer();
                }
            }
            catch (Exception er)
            {
                throw new Exception(er.Message);
            }
            return buff;
        }
        /// <summary>
        /// 将byte[]反序列化为对象
        /// 使用IFormatter的Deserialize发序列化
        /// </summary>
        /// <param name="buff">传入的byte[]</param>
        /// <returns></returns>
        public static object FormatterByteObject(byte[] buff)
        {
            if (buff == null)
                throw new ArgumentNullException("buff");
            object obj;
            try
            {
                using (var ms = new MemoryStream())
                {
                    IFormatter iFormatter = new BinaryFormatter();
                    obj = iFormatter.Deserialize(ms);
                }
            }
            catch (Exception er)
            {
                throw new Exception(er.Message);
            }
            return obj;
        }
    }
}
