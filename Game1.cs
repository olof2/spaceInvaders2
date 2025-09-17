using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Text;



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
        private List<Enemy> enemyList;
        private List<Enemy> enemyTrash;

        private List<Bullet> bulletList;
        private List<Bullet> bulletTrash;
        private Bullet bullet;
        private Bullet _bull;
        private Vector2 bulletVelocity;
        private Vector2 bulletPosition;
        private int j;

        private String _title = "";
        private int _score = 0;
        private int windowWidth;
        private int windowHeight;

        private CooldownTimer cooldownTimer;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1200;
            _graphics.ApplyChanges();
            Window.Title = _title;

            windowWidth = Window.ClientBounds.Width;
            windowHeight = Window.ClientBounds.Height;

            cooldownTimer = new CooldownTimer();
            cooldownTimer.ResetAndStart(0.6);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>(@"Ship_01-1");
            bulletTexture = Content.Load<Texture2D>(@"explosion");
            enemyTexture = Content.Load<Texture2D>(@"alien03_single");


            playerPosition = new Vector2(windowWidth/2, windowHeight - 20 - playerTexture.Height);
            playerVelocity = new Vector2(5, 5);
            lives = 3;
            player = new Player(playerTexture, playerPosition, playerVelocity, windowWidth, windowHeight, lives);

            enemyPosition = new Vector2(50, 50);
            enemyVelocity = new Vector2(2, 1);
            enemyList = new List<Enemy>();
            enemyTrash = new List<Enemy>();
            //spawning enemies
            for (int i = 0; i < 7; i++ )
            {
                enemyPosition.Y = 50;
                enemyPosition.X = 40 + i * 50 + i * enemyTexture.Width;
                enemy = new Enemy(enemyTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth);
                enemyList.Add(enemy);

                enemyPosition.Y = 140;
                enemy = new Enemy(enemyTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth);
                enemyList.Add(enemy);

                enemyPosition.Y = 230;
                enemy = new Enemy(enemyTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth);
                enemyList.Add(enemy);
            }

            bulletList = new List<Bullet>();
            bulletTrash = new List<Bullet>();
            bulletVelocity = new Vector2(0, -10);
            bulletPosition = new Vector2(0, 0);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Window.Title = _title;
            _title = "Spaceinvaders, score: " + _score + "window är" + windowHeight + " x " + windowWidth + "liv: " + lives;


            playerPosition = player.Update();
            cooldownTimer.Update(gameTime.ElapsedGameTime.TotalSeconds);

            foreach (Enemy _enemy in enemyList)
            {
                enemyPosition = _enemy.Update();
                if (enemyPosition.Y > windowHeight - 100) { lives -= 1; }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (cooldownTimer.IsDone())
                {
                    bullet = new Bullet(bulletTexture, collisionLayer, playerPosition, bulletVelocity, windowHeight - bulletTexture.Height - 100);
                    bulletList.Add(bullet);
                    cooldownTimer.ResetAndStart(0.6);
                }
            }

            foreach (Bullet _bullet in bulletList)
            {
                bulletPosition = _bullet.Update();


                foreach (Enemy _enemy in enemyList)
                {
                    if (_bullet.rect.Intersects(_enemy.rect))
                    {
                        enemyTrash.Add(_enemy);
                        bulletTrash.Add(_bullet);
                        _score += 100;
                    }
                }

                //for (int i = 0; i < enemyList.Count ; i++)
                //{
                //    //removes enemy hit by bullet
                //    if (_bullet.rect.Intersects(enemyList[i].rect))
                //        {
                //        enemyList.RemoveAt(i);
                //        break;
                //        }
                //}
            }

            foreach (Enemy _e in enemyTrash) {enemyList.Remove(_e);}
            foreach (Bullet _b in bulletTrash) {bulletList.Remove(_b);}

            //removing bullets that reached end length
            for (int i = 0; i < bulletList.Count; i++)
            {
                _bull = bulletList[i];
                if (_bull.position.Y < _bull.endPosition.Y)
                {
                    bulletList.RemoveAt(i);
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);
            _spriteBatch.Begin();

            player.Draw(_spriteBatch);

            foreach (Bullet _bullet in bulletList)
            {
                _bullet.Draw(_spriteBatch);
            }

            foreach (Enemy _enemy in enemyList)
            {
                _enemy.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
