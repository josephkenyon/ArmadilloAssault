using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Menu
{
    public class ButtonJson
    {
        public string Text { get; set; }
        public List<MenuAction> Actions { get; set; }
        public string Data { get; set; }
    }
}
