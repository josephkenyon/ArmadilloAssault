using ArmadilloAssault.Controls;
using System.Collections.Generic;

namespace ArmadilloAssault.Web.Communication
{
    public class ClientMessage
    {
        public ClientMessageType Type { get; set; }
        public string Id { get; set; } = null;
        public string Name { get; set; } = null;
        public float AimX { get; set; }
        public float AimY { get; set; }
        public List<Control> AreControlsDown { get; set; }
    }
}
