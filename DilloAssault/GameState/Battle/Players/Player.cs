using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DilloAssault.GameState.Battle.Players
{
    public class Player
    {
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public int PlayerIndex { get; set; }
        public int PlayerControllerIndex { get; set; }
    }
}
