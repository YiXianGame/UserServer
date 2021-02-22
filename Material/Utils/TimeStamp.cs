using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Utils
{
    public class TimeStamp
    {
        public static long Now()
        {
            DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local); // 当地时区
            long timeStamp = (long)(DateTime.Now - startTime).TotalMilliseconds; // 相差毫秒数
            return timeStamp;
        }
    }
}
