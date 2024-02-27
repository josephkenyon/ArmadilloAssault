using ArmadilloAssault.Configuration.Generics;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication.Frame
{
    public class LobbyFrame
    {
        public AvatarFrame AvatarFrame { get; set; } = new();
        public List<RectangleJson> PlayerBackgrounds { get; set; } = [];
        public List<int> PlayerBackgroundIds { get; set; } = [];
        public List<string> PlayerNames { get; set; } = [];
    }
}
