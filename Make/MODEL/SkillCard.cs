using Material.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Make.MODEL
{
    public class SkillCard : SkillCardBase
    {
        #region --字段--
        List<User> enemy = new List<User>();

        List<User> auxiliary = new List<User>();
        #endregion

        #region --属性--
        [JsonIgnore]
        public List<User> Enemy { get => enemy; set => enemy = value; }
        [JsonIgnore]
        public List<User> Auxiliary { get => auxiliary; set => auxiliary = value; }
        #endregion

        #region --方法--

        #endregion
    }
}
