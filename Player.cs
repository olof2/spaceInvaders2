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
    internal class Player
    {
        public Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;
        public int windowWidth;
        public int windowHeight;
        public int lives;
        public Microsoft.Xna.Framework.Rectangle rect;

        public struct KeyboardState;

        public Player(Texture2D texture, Vector2 position, Vector2 velocity, int windowWidth, int windowHeight, int lives)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            this.lives = lives;

            rect = new Microsoft.Xna.Framework.Rectangle((int)this.position.X, (int)this.position.Y, this.texture.Width, this.texture.Height);

        }

        public Vector2 Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && position.X + texture.Width < windowWidth)
            {
                position.X += velocity.X;
                rect.X = (int)position.X;
                rect.Y = (int)position.Y;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && position.X > 0)
            {
                position.X -= velocity.X;
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
