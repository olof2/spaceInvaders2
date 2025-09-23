using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spaceInvaders2
{
    internal class Enemy
    {
        Texture2D texture;
        public Vector2 position;
        public Vector2 velocity;
        int windowHeight;
        int windowWidth;
        public int lives;
        CooldownTimer moveTimer;
        CooldownTimer shootTimer;
        public Bullet bullet;
        List<Bullet> bulletList;
        public Microsoft.Xna.Framework.Rectangle rect;


        public Enemy(Texture2D texture, Vector2 position, Vector2 velocity, int windowHeight, int windowWidth, int lives)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            this.lives = lives;

            shootTimer = new CooldownTimer();
            shootTimer.ResetAndStart(1.0);
            bulletList = new List<Bullet>();

            moveTimer = new CooldownTimer();
            moveTimer.ResetAndStart(0.6);

            rect = new Microsoft.Xna.Framework.Rectangle((int)this.position.X, (int)this.position.Y, this.texture.Width, this.texture.Height);
        }

        public Vector2 Update(double gameTime)
        {

            moveTimer.Update(gameTime);
            shootTimer.Update(gameTime);

            if (shootTimer.IsDone() && lives > 0)
            {
                // Shoot

                shootTimer.ResetAndStart(2.0);
            }

            if (moveTimer.IsDone() && lives > 0)
            {
                position.Y += velocity.Y;
                rect.X = (int)position.X;
                rect.Y = (int)position.Y;
                moveTimer.ResetAndStart(0.6);
                return position;
            }
            else {return position;}
        }

        public void Draw(SpriteBatch sb)
        {
            if (lives > 0)
            {
                sb.Draw(texture, position, Microsoft.Xna.Framework.Color.White);
            }
        }
    }
}
