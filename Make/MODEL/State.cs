using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Make.MODEL
{
    [JsonObject(MemberSerialization.OptOut)]
    public class State
    {
        private string name;//状态名称
        private Player owner;//状态来源
        private Player direct;//状态对象
        private int duration_Round;//持续回合
        private int expire_Round;//到期回合
        private int duration_Immediate;//持续时长
        private DateTime expire_Immediate;//到期时间
        private string message;//状态反馈
        private bool is_Self;//是否自身
        private int effect_mp;//作用范围
        private string effect_Information;
        private string message_Information;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                if(name == "冻结")
                {
                    Effect_Information = "自身仙气低于X值将会被冻结";
                }
                else if(name == "霸体")
                {
                    Effect_Information = "自身仙气低于X值将会被霸体";
                }
                else if(name == "缴械")
                {
                    Effect_Information = "技能仙气低于X值将无法使用物理技能";
                }
                else if (name == "沉默")
                {
                    Effect_Information = "技能仙气低于X值将无法使用魔法技能";
                }
                else if(name == "破甲")
                {
                    Effect_Information = "技能仙气低于X值将受到破甲效果";
                }
                else if (name == "穿甲")
                {
                    Effect_Information = "技能仙气低于X值将受到穿甲效果";
                }
                else if (name == "格挡")
                {
                    Effect_Information = "技能仙气低于X值将受到格挡效果";
                }
                else if (name == "反制")
                {
                    Effect_Information = "技能仙气低于X值将受到反制效果";
                }
                else if (name == "反弹")
                {
                    Effect_Information = "技能仙气低于X值将受到反弹效果";
                }
                else if (name == "无敌")
                {
                    Effect_Information = "血量伤害在X值范围内变动(取绝对值)将受到无敌效果";
                }
                else if (name == "封穴")
                {
                    Effect_Information = "仙气伤害在X值范围内变动(取绝对值)将受到封穴效果";
                }
                else if (name == "恒血")
                {
                    Effect_Information = "每回合回复的血量值";
                }
                else if (name == "恒气")
                {
                    Effect_Information = "每回合回复的仙气值";
                }
            }
        }
        public string Message { get => message; set => message = value; }
        public bool Is_Self { get => is_Self; set => is_Self = value; }
        public int Duration_Round { get => duration_Round; set => duration_Round = value; }
        public int Expire_Round { get => expire_Round; set => expire_Round = value; }
        public int Duration_Immediate
        {
            get => duration_Immediate;
            set 
            { 
                duration_Immediate = value;
                int round = Duration_Immediate / GeneralControl.Menu_GameControl_Class.Instance.Immediate_To_Round;
                if (round > 0) Duration_Round = round;
                else Duration_Round = 1;
            }
        }
        [JsonIgnore]
        public DateTime Expire_Immediate { get => expire_Immediate; set => expire_Immediate = value; }
        public int Effect_mp { get => effect_mp; set => effect_mp = value; }
        [JsonIgnore]
        public Player Owner { get => owner; set => owner = value; }
        [JsonIgnore]
        public Player Direct { get => direct; set => direct = value; }
        public string Effect_Information { get => effect_Information; set => effect_Information = value; }
        public string Message_Information { get => message_Information; set => message_Information = value; }

        public State Clone()
        {
            return MemberwiseClone() as State;
        }
        public bool Can_Effect( int mp)
        {
            if ( Effect_mp >= mp) return true;
            return false;
        }

        /// <summary>
        /// 检测该状态是否到期
        /// </summary>
        /// <returns>True:到期 False:没到期</returns>
        public bool Is_Expire()
        {
            if(Direct.Active ==  Enums.Player_Active.Immediate)
            {
                if (Expire_Immediate >= DateTime.Now)
                {
                    return false;
                }
                else
                {
                    Direct.States.Remove(this);
                    return true;
                }
            }
            else if (Direct.Active == Enums.Player_Active.Round)
            {
                if (Expire_Round <= Direct.Room.Round)
                {
                    return false;
                }
                else
                {
                    Direct.States.Remove(this);
                    return true;
                }
            }
            else
            {
                if (Expire_Immediate >= DateTime.Now)
                {
                    return false;
                }
                else
                {
                    Direct.States.Remove(this);
                    return true;
                }
            }
        }
    }
}
