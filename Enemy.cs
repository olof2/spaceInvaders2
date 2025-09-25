using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace spaceInvaders2
{
    internal class Enemy
    {
        Texture2D texture;
        Texture2D bulletTexture;
        public Vector2 position;
        public Vector2 velocity;
        int windowHeight;
        int windowWidth;
        public int lives;
        CooldownTimer moveTimer;

        Vector2 bulletStartPosition;
        public Bullet bullet;
        Vector2 bulletVelocity;
        public Microsoft.Xna.Framework.Rectangle rect;


        public Enemy(Texture2D texture, Texture2D bulletTexture, Vector2 position, Vector2 velocity, int windowHeight, int windowWidth, int lives)
        {
            this.texture = texture;
            this.bulletTexture = bulletTexture;
            this.position = position;
            this.velocity = velocity;
            this.windowHeight = windowHeight;
            this.windowWidth = windowWidth;
            this.lives = lives;

            bulletVelocity = new Vector2(0, 6);
            bulletStartPosition = new Vector2(position.X + texture.Width / 2 - bulletTexture.Width / 2, position.Y + texture.Height);

            moveTimer = new CooldownTimer();
            moveTimer.ResetAndStart(0.6);

            rect = new Microsoft.Xna.Framework.Rectangle((int)this.position.X, (int)this.position.Y, this.texture.Width, this.texture.Height);
        }

        public void Shoot()
        {
            bullet = new Bullet(bulletTexture, 0, bulletStartPosition, bulletVelocity, windowHeight);
        }

        public Vector2 Update(double gameTime)
        {
            moveTimer.Update(gameTime);
            bulletStartPosition = new Vector2(position.X + texture.Width / 2 - bulletTexture.Width / 2, position.Y + texture.Height);

            if (moveTimer.IsDone() && lives > 0)
            {
                position.X += velocity.X;
                moveTimer.ResetAndStart(0.6);
                rect.X = (int)position.X;
                rect.Y = (int)position.Y;
                return position;
            }

            else { return position; }
        }

        public void JumpDown()
        {
            position.Y += velocity.Y;
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;
        }

        public void flipDirection()
        {
            velocity.X *= -1;    
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
