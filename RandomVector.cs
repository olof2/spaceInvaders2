using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace spaceInvaders2
{
    internal class RandomVector
    {
        public Vector2 pos;
        int posX;
        int posY;
        Random rnd;
        public Vector2 Position(int maxX, int maxY)
        {
            rnd = new Random();
            posX = rnd.Next(maxX);
            posY = rnd.Next(maxY);
            pos = new Vector2(posX, posY);
            return pos;
        }
    }
}
