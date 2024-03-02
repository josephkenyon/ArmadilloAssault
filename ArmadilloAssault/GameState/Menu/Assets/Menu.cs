﻿using ArmadilloAssault.Configuration.Menu;
using ArmadilloAssault.Configuration.Textures;
using ArmadilloAssault.Controls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArmadilloAssault.GameState.Menu.Assets
{
    public class Menu(MenuJson menu, bool applyConditionsImmediately = false)
    {
        //private static readonly int ButtonSpaceY = 504;
        private static readonly int ButtonSpaceStart = 448;
        private static readonly int ButtonsSpace = 40;

        public string Name { get; set; } = menu.Name;
        public List<Button> Buttons { get; set; } = GetButtons(menu.Buttons, menu.HasLoadingSpinner, applyConditionsImmediately);
        public LoadingSpinner LoadingSpinner { get; set; } = menu.HasLoadingSpinner ? new(new Point ((1920 / 2) - (LoadingSpinner.Size / 2), ButtonSpaceStart)) : null;

        private static List<Button> GetButtons(List<ButtonJson> buttonJsons, bool hasSpinner, bool applyConditionsImmediately = false)
        {
            var buttons = new List<Button>();

            //var spaceBetweenButtons = buttonJsons.Count > 1 ? (ButtonSpaceY - (buttonJsons.Count * MenuManager.ButtonSize.Y)) / (buttonJsons.Count - 1) : 0;

            var start = ButtonSpaceStart;

            if (hasSpinner)
            {
                start += LoadingSpinner.Size + ButtonsSpace;
            }

            var buttonLocation = new Point((1920 / 2) - (MenuManager.ButtonSize.X / 2), start);

            foreach (var buttonJson in buttonJsons.Where(buttonJson => !applyConditionsImmediately || MenuManager.ConditionsFulfilled(buttonJson.Conditions)))
            {
                var size = buttonJson.Size != null ? buttonJson.Size.ToPoint() : MenuManager.ButtonSize;

                if (buttonJson.Location == null)
                {
                    buttons.Add(new Button(buttonJson, buttonLocation, size));
                    buttonLocation = new Point(buttonLocation.X, buttonLocation.Y + ButtonsSpace + MenuManager.ButtonSize.Y);
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

                button.Selected = button.Visible && button.Enabled && rectangle.Contains(ControlsManager.GetAimPosition(0));
            }
        }
    }
}
