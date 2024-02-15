using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DilloAssault.Web.Communication.Updates
{
    public abstract class BaseUpdate
    {
        public List<int> Xs { get; set; } = [];
        public List<int> Ys { get; set; } = [];

        public Vector2 GetPosition(int i) => new(Xs[i], Ys[i]);
    }
}
