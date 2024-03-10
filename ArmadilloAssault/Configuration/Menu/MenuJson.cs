using System.Collections.Generic;

namespace ArmadilloAssault.Configuration.Menus
{
    public class MenuJson
    {
        public string Name { get; set; }
        public bool HasLoadingSpinner { get; set; }
        public List<ButtonJson> Buttons { get; set; }
    }
}
