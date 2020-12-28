using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL
{
    [Serializable]
    public class Simple_SkillCard
    {
        private string name;
        private int level;
        private int amount;

        public string Name { get => name; set => name = value; }
        public int Level { get => level; set => level = value; }
        public int Amount { get => amount; set => amount = value; }
        public Simple_SkillCard(string name,int level,int amount)
        {
            this.name = name;
            this.level = level;
            this.amount = amount;
        }
    }
}
