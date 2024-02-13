using DilloAssault.Controls;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.GameState.Battle.Players
{
    public class Player
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public int PlayerIndex { get; set; }
        public int PlayerControllerIndex { get; set; }
        public Vector2 AimPosition { get; set; } = Vector2.Zero;
        public List<Control> AreControlsDown { get; set; } = [];
    }
}
