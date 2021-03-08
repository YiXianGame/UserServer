using Make.Model;
using Make.RPCClient.Request;
using Make.RPCServer.Request;
using Material.Entity;
using Material.Entity.Config;
using Material.Entity.Match;
using Material.Model.MatchSystem;
using System.Collections.Generic;

namespace Make
{
    public static class Core
    {
        #region --字段--
        private static UserServerConfig config;
        private static Dictionary<long, SkillCard> skillCards = new Dictionary<long, SkillCard>();
        private static Model.Repository repository;
        private static UserRequest userRequest;
        private static PlayerServerRequest playerServerRequest;
        private static SkillCardRequest skillCardClient;
        private static ReadyRequest readyRequest;
        private static EquipRequest equipRequest;
        private static MatchSystem<MatchSquad, MatchTeam> soloMatchSystem = new MatchSystem<MatchSquad, MatchTeam>(1, 1);
        private static MatchSystem<MatchTeam, MatchTeamGroup> soloGroupMatchSystem = new MatchSystem<MatchTeam, MatchTeamGroup>(2, 2);
        #endregion

        #region --属性--
        public static Dictionary<long, SkillCard> SkillCards { get => skillCards; set => skillCards = value; }
        public static Model.Repository Repository { get => repository; set => repository = value; }
        public static UserRequest UserRequest { get => userRequest; set => userRequest = value; }
        public static SkillCardRequest SkillCardRequest { get => skillCardClient; set => skillCardClient = value; }
        public static UserServerConfig Config { get => config; set => config = value; }
        public static PlayerServerRequest PlayerServerRequest { get => playerServerRequest; set => playerServerRequest = value; }
        public static MatchSystem<MatchSquad, MatchTeam> SoloMatchSystem { get => soloMatchSystem; set => soloMatchSystem = value; }
        public static MatchSystem<MatchTeam, MatchTeamGroup> SoloGroupMatchSystem { get => soloGroupMatchSystem; set => soloGroupMatchSystem = value; }
        public static ReadyRequest ReadyRequest { get => readyRequest; set => readyRequest = value; }
        public static EquipRequest EquipRequest { get => equipRequest; set => equipRequest = value; }
        #endregion

    }
}
