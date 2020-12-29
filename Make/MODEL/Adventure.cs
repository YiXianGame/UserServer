using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Make.MODEL
{
    /// <summary>
    /// 奇遇
    /// </summary>
    public class Adventure
    {
        #region --字段--
        private string name = "";//奇遇名
        private int attack;//攻击力
        private int cure;//治疗量
        private int self_Mp;//自我能量
        private int direct_Mp;//指向能量
        private int state=1;//奇遇状态（是否可用）
        private ObservableCollection<State> effect_States=new ObservableCollection<State>();//奇遇所自带的状态效果
        private string messages="";//自带信息
        private int probability;//概率
        private string description="";//奇遇的描述（介绍）
        private string iD;
        private string cloud = "云端";
        #endregion

        #region --属性--
        public string ID { get => iD; set => iD = value; }
        public string Name
        { 
            get => name;
            set
            {
                if (!int.TryParse(value, out int result))
                {
                    if (GeneralControl.Adventures.Contains(this))
                    {
                        while ((from Adventure item in GeneralControl.Adventures where item.Name == value && item.Name != Name select item).Any()) value += "-副本";
                    }
                    name = value;
                }
                else return;
            }
        }
        private string username;
        public string UserName { get => username; set => username = value; }
        public string Messages { get => messages; set => messages = value; }
        public int Probability { get => probability; set => probability = value; }
        public ObservableCollection<State> Effect_States { get => effect_States; set => effect_States = value; }
        public int State { get => state; set => state = value; }
        public string Description { get => description; set => description = value; }
        public string Cloud { get => cloud; set => cloud = value; } 
        public int Attack { get => attack; set => attack = value; }
        public int Cure { get => cure; set => cure = value; }
        public int Self_Mp { get => self_Mp; set => self_Mp = value; }
        public int Direct_Mp { get => direct_Mp; set => direct_Mp = value; }
        #endregion

        #region --方法--
        public void SetName(string adventure_Name)
        {
            name = adventure_Name;
        }
        public Adventure()
        {
            string temp_id;
            do
            {
                temp_id = Guid.NewGuid().ToString();
            }
            while (File.Exists(GeneralControl.Directory + "\\技能卡\\" + temp_id + ".json"));
            ID = temp_id;
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this);
            string filepath = GeneralControl.Directory + "\\奇遇\\" + ID + ".json";
            File.WriteAllText(filepath, json);
        }

        public void Delete()
        {
            string filepath = GeneralControl.Directory + "\\奇遇\\" + ID + ".json";
            GeneralControl.Adventures.Remove(this);
            GeneralControl.Adventures_ID.Remove(ID);
            File.Delete(filepath);
        }

        public void Add_To_General()
        {
            while ((from Adventure item in GeneralControl.Adventures where item.Name == Name select item).Any()) Name += "-副本";
            while ((from Adventure item in GeneralControl.Adventures where item.ID == ID select item).Any()) ID = Guid.NewGuid().ToString();
            GeneralControl.Adventures.Add(this);
            GeneralControl.Adventures_ID.Add(ID, this);
        }
        #endregion
    }
}
