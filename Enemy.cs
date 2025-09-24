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
        CooldownTimer shootTimer;
        public Bullet bullet;
        Vector2 bulletVelocity;
        List<Bullet> bulletList;
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

            shootTimer = new CooldownTimer();
            shootTimer.ResetAndStart(1.0);
            bulletList = new List<Bullet>();
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
            //shootTimer.Update(gameTime);
            bulletStartPosition = new Vector2(position.X + texture.Width / 2 - bulletTexture.Width / 2, position.Y + texture.Height);

            //if (shootTimer.IsDone() && lives > 0)
            //{
            //    // Shoot
            //    Shoot();
            //    shootTimer.ResetAndStart(2.0);
            //}

            if (moveTimer.IsDone() && lives > 0)
            {
                position.Y += velocity.Y;
                rect.X = (int)position.X;
                rect.Y = (int)position.Y;
                moveTimer.ResetAndStart(0.6);
                return position;
            }

            else { return position; }
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
