using ArmadilloAssault.Configuration.Menus;
using ArmadilloAssault.Configuration.Textures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Menus.Assets
{
    public class Button(ButtonJson json, Point position, Point size)
    {
        public string Text { get; set; } = json.Text;
        public TextureName TextureName { get; set; } = json.TextureName;
        public List<MenuAction> Actions { get; set; } = json.Actions;
        public string Data { get; set; } = json.Data;

        public bool Unselectable { get; set; } = json.Unselectable;
        public bool Selected { get; set; }
        public List<MenuCondition> Conditions { get; set; } = json.Conditions;
        public MenuCondition EnabledCondition { get; set; } = json.EnabledCondition;
        public MenuKey TextKey { get; set; } = json.TextKey;
        public MenuKey TextureKey { get; set; } = json.TextureKey;
        public bool Visible { get; set; }
        public bool Enabled { get; set; }

        public Rectangle GetRectangle() => new(position, size);
    }
}
