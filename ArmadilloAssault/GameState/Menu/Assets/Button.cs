using ArmadilloAssault.Configuration.Menu;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ArmadilloAssault.GameState.Menu.Assets
{
    public class Button(ButtonJson json, Point position)
    {
        public string Text { get; set; } = json.Text;
        public List<MenuAction> Actions { get; set; } = json.Actions;
        public string Data { get; set; } = json.Data;
        public Point Position { get; set; } = position;

        public bool Selected { get; set; }

        public Rectangle GetRectangle() => new(Position, MenuManager.ButtonSize);
    }
}
