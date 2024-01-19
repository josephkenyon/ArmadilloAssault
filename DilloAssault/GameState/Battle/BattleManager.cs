using DilloAssault.Assets;
using DilloAssault.GameState.Battle.Players;
using DilloAssault.Graphics.Drawing;
using System.Collections.Generic;

namespace DilloAssault.GameState.Battle
{
    public static class BattleManager
    {
        public static Scene Scene { get; set; }
        public static List<Player> Players { get; set; }

        public static void Initialize()
        {
            Players = [
                new Player { Name = "Player1", ConnectionId = null, PlayerControllerIndex = -1, PlayerIndex = 1 }
            ];
        }

        public static void Update()
        {
        }

        public static void Draw()
        {
            Scene.TileLists.ForEach(list => DrawingManager.DrawCollection([.. list.Tiles]));
        }
    }
}
