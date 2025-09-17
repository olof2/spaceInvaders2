using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spaceInvaders1._1
{
    internal class Bullet
    {
        public Texture2D texture;
        public int collisionLayer;
        public Vector2 position;
        public Vector2 endPosition;
        public Vector2 velocity;
        public int direction;
        public int distance;
        public Microsoft.Xna.Framework.Rectangle rect;

        public Bullet(Texture2D texture, int collisionLayer, Vector2 position, Vector2 velocity, int windowHeight)
        {
            this.texture = texture;
            this.collisionLayer = collisionLayer;
            this.position = position;
            this.velocity = velocity;
            distance = windowHeight;

            rect = new Microsoft.Xna.Framework.Rectangle((int)this.position.X, (int)this.position.Y, texture.Width, texture.Height);

            if (velocity.Y < 0) { direction = -1; }
            else { direction = 1; }

            endPosition = new Vector2(position.X, position.Y + distance * direction);
        }

        public Vector2 Update()
        {
            if (position.Y * direction < endPosition.Y)
            {
                position.Y += velocity.Y;
                rect.X = (int)position.X;
                rect.Y = (int)position.Y;
            }
            return position;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, position, Microsoft.Xna.Framework.Color.HotPink);
        }
    }
}

//testa lägga till classen i main och spawna från spelaren, kolla så bullet går i rätt rikning och distans