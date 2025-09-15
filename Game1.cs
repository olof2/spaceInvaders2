using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace spaceInvaders1._1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D playerTexture;
        private Texture2D bulletTexture;
        private Texture2D enemyTexture;

        private Player player;
        private Vector2 playerPosition;
        private Vector2 playerVelocity;
        private int lives;
        private int collisionLayer = 1;

        private Enemy enemy;
        private Vector2 enemyPosition;
        private Vector2 enemyVelocity;

        private List<Bullet> bulletList;
        private Bullet bullet;
        private Vector2 bulletVelocity;

        private String _title = "";
        private int _score = 0;
        private int windowWidth;
        private int windowHeight;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();
            Window.Title = _title;

            windowWidth = Window.ClientBounds.Width;
            windowHeight = Window.ClientBounds.Height;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>(@"Ship_01-1");
            bulletTexture = Content.Load<Texture2D>(@"explosion");
            enemyTexture = Content.Load<Texture2D>(@"alien02_sprites");

            playerPosition = new Vector2(windowWidth/2, windowHeight - 20 - playerTexture.Height);
            playerVelocity = new Vector2(5, 5);
            lives = 3;
            player = new Player(playerTexture, playerPosition, playerVelocity, windowWidth, windowHeight, lives);

            enemyPosition = new Vector2(100, 50);
            enemyVelocity = new Vector2(2, 2);
            enemy = new Enemy(enemyTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth);

            bulletList = new List<Bullet>();
            bulletVelocity = new Vector2(0, -10);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Window.Title = _title;
            _title = "Spaceinvaders, score: " + _score + "window är" + windowHeight + " x " + windowWidth + "liv: " + lives;

            playerPosition = player.Update();
            enemyPosition = enemy.Update();
            if (enemyPosition.Y > windowHeight - 100) { lives -= 1; }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                bullet = new Bullet(bulletTexture, collisionLayer, playerPosition, bulletVelocity, windowHeight - bulletTexture.Height);
                bulletList.Add(bullet);
            }
            // lägg till timer för att kunna skjuta

            foreach (Bullet _bullet in bulletList)
            {
                _bullet.Update();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);
            _spriteBatch.Begin();

            player.Draw(_spriteBatch);
            enemy.Draw(_spriteBatch);

            foreach (Bullet _bullet in bulletList)
            {
                if (_bullet.direction < 0 && _bullet.position.Y > _bullet.endPosition.Y)
                {
                    _bullet.Draw(_spriteBatch);
                }
                //lägg till logik om bullet åker andra hållet
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
