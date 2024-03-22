using ArmadilloAssault.GameState.Battle.Mode;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class BattleStaticData
    {
        public string SceneName { get; set; }
        public ModeType ModeType { get; set; }
        public List<string> Names { get; set; } = [];
        public AvatarStaticData AvatarStaticData { get; set; } = new();

        public ItemFrame ItemFrame { get; set; }
    }
}
