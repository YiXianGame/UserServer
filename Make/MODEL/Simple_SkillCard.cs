using System;

namespace Make.MODEL
{
    [Serializable]
    public class Simple_SkillCard
    {
        #region --字段--
        private ulong userid;
        private ulong cardid;
        private string solution;
        #endregion

        #region --属性--
        public ulong UserID { get => userid; set => userid = value; }
        public ulong CardID { get => cardid; set => cardid = value; }
        public string Solution { get => solution; set => solution = value; }


        #endregion

        #region --方法--

        public Simple_SkillCard(ulong userid, ulong cardid, string solution)
        {
            this.userid = userid;
            this.cardid = cardid;
            this.solution = solution;
        }


        #endregion
    }
}
