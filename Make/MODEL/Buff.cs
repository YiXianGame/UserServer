using Material.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Make.MODEL
{
    public class Buff : BuffBase
    {
        #region --字段--
        private User owner;//Buff拥有者
        private long expire;//到期时间
        private bool isExpire;//到期时间
        #endregion

        #region --属性--
        [JsonIgnore]
        public User Owner { get => owner; set => owner = value; }
        [JsonIgnore]
        public long Expire { get => expire; set => expire = value; }
        [JsonIgnore]
        public bool IsExpire { get => isExpire; set => isExpire = value; }
        #endregion

        #region --方法--
        public Buff Clone()
        {
            return MemberwiseClone() as Buff;
        }
        public bool Can_Effect( int mp)
        {
            if ( Power >= mp) return true;
            return false;
        }
        
        /// <summary>
        /// 检测该状态是否到期
        /// </summary>
        /// <returns>True:到期 False:没到期</returns>
        //public bool Is_Expire()
        //{
        //    if(owner.Active ==  Enums.Player_Active.Immediate)
        //    {
        //        if (Expire_Immediate >= DateTime.Now)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            Direct.States.Remove(this);
        //            return true;
        //        }
        //    }
        //    else if (Direct.Active == Enums.Player_Active.Round)
        //    {
        //        if (Expire_Round <= Direct.Room.Round)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            Direct.States.Remove(this);
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        if (Expire_Immediate >= DateTime.Now)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            Direct.States.Remove(this);
        //            return true;
        //        }
        //    }
        //}
        #endregion
    }
}
