using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ArmadilloAssault.Controls
{
    public static class ControlsHelper
    {
        public static readonly double ThumbStickConstant = 0.1;

        public readonly static List<KeyValuePair<Keys, Control>> KeyControlMappings = [
            new(Keys.A, Control.Left),
            new(Keys.D, Control.Right),
            new(Keys.W, Control.Up),
            new(Keys.S, Control.Down),
            new(Keys.LeftControl, Control.Crouch),
            new(Keys.C, Control.Crouch),
            new(Keys.Tab, Control.Cycle_Weapon),
            new(Keys.D1, Control.Pistol),
            new(Keys.D2, Control.Assault),
            new(Keys.D3, Control.Shotgun),
            new(Keys.D4, Control.Sniper),
            new(Keys.R, Control.Reload),
            new(Keys.Space, Control.Jump),
            new(Keys.Escape, Control.Start),
        ];

        public readonly static List<KeyValuePair<Buttons, Control>> ButtonControlMappings = [
            new(Buttons.A, Control.Jump),
            new(Buttons.X, Control.Reload),
            new(Buttons.Y, Control.Cycle_Weapon),
            new(Buttons.LeftTrigger, Control.Fire_Primary),
            new(Buttons.RightTrigger, Control.Fire_Secondary),
            new(Buttons.Start, Control.Start),
        ];
    }
}
