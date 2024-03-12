using ArmadilloAssault.Configuration.Menus;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Controls;
using ArmadilloAssault.GameState.Menus;
using ArmadilloAssault.GameState.Menus.Assets;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.Assets
{
    public class Menu(MenuJson menu, bool applyConditionsImmediately = false)
    {
        //private static readonly int ButtonSpaceY = 504;
        private static readonly int ButtonSpaceStart = 448;
        private static readonly int ButtonsSpace = 40;

        public string Name { get; set; } = menu.Name;
        public List<Button> Buttons { get; set; } = GetButtons(menu.Buttons, menu.HasLoadingSpinner, applyConditionsImmediately);
        public LoadingSpinner LoadingSpinner { get; set; } = menu.HasLoadingSpinner ? new(new Point(1920 / 2 - LoadingSpinner.Size / 2, ButtonSpaceStart)) : null;

        public IEnumerable<Button> VisibleButtons => Buttons.Where(button => button.Visible).ToList();

        private static List<Button> GetButtons(List<ButtonJson> buttonJsons, bool hasSpinner, bool applyConditionsImmediately = false)
        {
            var buttons = new List<Button>();

            var start = ButtonSpaceStart;

            if (hasSpinner)
            {
                start += LoadingSpinner.Size + ButtonsSpace;
            }

            var buttonLocationY = start;
            foreach (var buttonJson in buttonJsons.Where(buttonJson => !applyConditionsImmediately || MenuManager.ConditionsFulfilled(buttonJson.Conditions)))
            {
                var size = buttonJson.Size != null ? buttonJson.Size.ToPoint() : MenuManager.ButtonSize;

                if (buttonJson.Location == null)
                {
                    buttons.Add(new Button(buttonJson, new Point(1920 / 2 - (buttonJson.Size != null ? buttonJson.Size.ToPoint() : MenuManager.ButtonSize).X / 2, buttonLocationY), size));
                    buttonLocationY += ButtonsSpace + MenuManager.ButtonSize.Y;
                }
                else
                {
                    buttons.Add(new Button(buttonJson, buttonJson.Location.ToPoint(), size));
                }

            }

            return buttons;
        }

        public void Update()
        {
            LoadingSpinner?.Update();

            foreach (var button in Buttons)
            {
                var rectangle = button.GetRectangle();
                button.Visible = MenuManager.ConditionsFulfilled(button.Conditions);
                button.Enabled = MenuManager.ConditionFulfilled(button.EnabledCondition);

                if (button.Visible && button.TextKey != MenuKey.nothing)
                {
                    button.Text = MenuManager.GetValue(button.TextKey);
                }

                if (button.Visible && button.TextureKey != MenuKey.nothing)
                {
                    button.TextureName = Enum.Parse<TextureName>(MenuManager.GetValue(button.TextureKey));
                }

                button.Selected = !button.Unselectable && button.Visible && button.Enabled && rectangle.Contains(ControlsManager.GetMousePosition(0));
            }
        }
    }
}
