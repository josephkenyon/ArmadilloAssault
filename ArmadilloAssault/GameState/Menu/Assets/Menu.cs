using ArmadilloAssault.Configuration.Menu;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Menu.Assets
{
    public class Menu(MenuJson menu)
    {
        private static readonly int ButtonSpaceY = 504;
        private static readonly int ButtonSpaceStart = 448;

        public string Name { get; set; } = menu.Name;
        public List<Button> Buttons { get; set; } = GetButtons(menu.Buttons);

        private static List<Button> GetButtons(List<ButtonJson> buttonJsons)
        {
            var buttons = new List<Button>();

            var spaceBetweenButtons = buttonJsons.Count > 1 ? (ButtonSpaceY - (buttonJsons.Count * MenuManager.ButtonSize.Y)) / (buttonJsons.Count - 1) : 0;

            var buttonLocation = new Point((1920 / 2) - (MenuManager.ButtonSize.X / 2), ButtonSpaceStart);

            foreach (var buttonJson in buttonJsons)
            {
                buttons.Add(new Button(buttonJson, buttonLocation));
                buttonLocation = new Point(buttonLocation.X, buttonLocation.Y + spaceBetweenButtons + MenuManager.ButtonSize.Y);
            }

            return buttons;
        }
    }
}
