using ArmadilloAssault.Configuration.Menu;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Menu.Assets
{
    public class Menu(MenuJson menu)
    {
        //private static readonly int ButtonSpaceY = 504;
        private static readonly int ButtonSpaceStart = 448;
        private static readonly int ButtonsSpace = 40;

        public string Name { get; set; } = menu.Name;
        public List<Button> Buttons { get; set; } = GetButtons(menu.Buttons, menu.HasLoadingSpinner);
        public LoadingSpinner LoadingSpinner { get; set; } = menu.HasLoadingSpinner ? new(new Point ((1920 / 2) - (LoadingSpinner.Size / 2), ButtonSpaceStart)) : null;

        private static List<Button> GetButtons(List<ButtonJson> buttonJsons, bool hasSpinner)
        {
            var buttons = new List<Button>();

            //var spaceBetweenButtons = buttonJsons.Count > 1 ? (ButtonSpaceY - (buttonJsons.Count * MenuManager.ButtonSize.Y)) / (buttonJsons.Count - 1) : 0;

            var start = ButtonSpaceStart;

            if (hasSpinner)
            {
                start += LoadingSpinner.Size + ButtonsSpace;
            }

            var buttonLocation = new Point((1920 / 2) - (MenuManager.ButtonSize.X / 2), start);

            foreach (var buttonJson in buttonJsons)
            {
                buttons.Add(new Button(buttonJson, buttonLocation));
                buttonLocation = new Point(buttonLocation.X, buttonLocation.Y + ButtonsSpace + MenuManager.ButtonSize.Y);
            }

            return buttons;
        }

        public void Update()
        {
            if (LoadingSpinner != null)
            {
                LoadingSpinner.Update();
            }
        }
    }
}
