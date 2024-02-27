using ArmadilloAssault.Configuration.Menu;
using ArmadilloAssault.Configuration.Textures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Menu.Assets
{
    public class Button(ButtonJson json, Point position, Point size)
    {
        public string Text { get; set; } = json.Text;
        public TextureName TextureName { get; set; } = json.TextureName;
        public List<MenuAction> Actions { get; set; } = json.Actions;
        public string Data { get; set; } = json.Data;

        public bool Selected { get; set; }
        public MenuCondition EnabledCondition { get; set; } = json.EnabledCondition;
        public bool Enabled { get; set; }

        public Rectangle GetRectangle() => new(position, size);
    }
}
