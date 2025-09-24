using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing.Text;



namespace spaceInvaders2
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        Texture2D playerTexture;
        Texture2D bulletTexture;
        Texture2D enemyTexture;

        Player player;
        Vector2 playerPosition;
        Vector2 playerVelocity;
        int lives;
        int collisionLayer = 1;

        Enemy enemy;
        Vector2 enemyPosition;
        Vector2 enemyVelocity;
        Enemy[,] enemyArray;
        Random rnd;
        int enemyCol;
        int enemyRow;
        bool goRight;

        Bullet bullet;
        Bullet _bull;
        List<Bullet> bulletList;
        List<Bullet> bulletTrash;
        Vector2 bulletVelocity;
        Vector2 bulletStartPosition;
        Vector2 bulletPosition;

        String _title = "";
        int _score = 0;
        int windowWidth;
        int windowHeight;

        CooldownTimer shootingTimer;
        CooldownTimer enemyShootingTimer;

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

            shootingTimer = new CooldownTimer();
            shootingTimer.ResetAndStart(0.6);
            enemyShootingTimer = new CooldownTimer();
            enemyShootingTimer.ResetAndStart(1.0);

            base.Initialize();
        }

        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerTexture = Content.Load<Texture2D>(@"Ship_01-1");
            bulletTexture = Content.Load<Texture2D>(@"Bullet");
            enemyTexture = Content.Load<Texture2D>(@"alien03_single");


            playerPosition = new Vector2(windowWidth/2, windowHeight - 20 - playerTexture.Height);
            playerVelocity = new Vector2(5, 5);
            lives = 3;
            player = new Player(playerTexture, playerPosition, playerVelocity, windowWidth, windowHeight, lives);


            enemyPosition = new Vector2(50, 50);
            enemyVelocity = new Vector2(20, 20);
            enemyArray = new Enemy[3,5];
            rnd = new Random();
            enemyCol = 4;
            enemyRow = 2;
            goRight = true;

            //spawning enemies in ARRAY, 3 rows of 5
            for (int i = 0; i < 5; i++)
            {
                enemyPosition.Y = 20;
                enemyPosition.X = 20 + i * 30 + i * enemyTexture.Width;
                enemy = new Enemy(enemyTexture, bulletTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth, 2);
                enemyArray[0,i] = enemy;

                enemyPosition.Y = 110;
                enemy = new Enemy(enemyTexture, bulletTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth, 1);
                enemyArray[1,i] = enemy;

                enemyPosition.Y = 200;
                enemy = new Enemy(enemyTexture, bulletTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth, 1);
                enemyArray[2,i] = enemy;
            }


            bulletList = new List<Bullet>();
            bulletTrash = new List<Bullet>();
            bulletVelocity = new Vector2(0, -10);
            bulletPosition = new Vector2(0, 0);

        }


        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }


            //updating title with score and lives
            Window.Title = _title;
            _title = "Spaceinvaders, score: " + _score + ", liv: " + lives;


            playerPosition = player.Update();
            shootingTimer.Update(gameTime.ElapsedGameTime.TotalSeconds);
            enemyShootingTimer.Update(gameTime.ElapsedGameTime.TotalSeconds);

            //enemy update logic
            foreach (Enemy _enemy in enemyArray)
            {
                enemyPosition = _enemy.Update(gameTime.ElapsedGameTime.TotalSeconds, goRight);

                if (enemyPosition.X >= windowWidth - enemyTexture.Width)
                {
                    goRight = false;
                    //vad som går fel är att den kollar en fiende (rad 0) och rad 1 och 2 hoppar ner 1 frame före rad 0


                    //make all enemies jump down when one hits the edge _____fortsätt här
                    //foreach (Enemy _e in enemyArray) { _e.position.Y += 20; }
                    //denna funkar inte, något händer med foreach loopen i foreach, den loopar för alla gånger antalet fiender
                    //_enemy.JumpDown(); kan kallas på men funkar inte riktigt, samma som ovan
                }
                else if (enemyPosition.X <= 0) { goRight = true; }

                //checking if enemy reached bottom of screen, if so, player loses a life and enemy dies
                if (enemyPosition.Y > windowHeight - 100)
                {
                    lives -= 1;
                    _enemy.lives = 0;
                    _enemy.position.Y = 200;
                }

                //updating enemy bullets and checking collision with player
                if (_enemy.bullet != null)
                {
                    _enemy.bullet.Update();
                    if (_enemy.bullet.rect.Intersects(player.rect))
                    {
                        lives -= 1;
                        _enemy.bullet = null;
                    }
                }

            }

            //enemy shooting logic
            if (enemyShootingTimer.IsDone())
            {
                //slumpar fram en enemy som ska skjuta, kollar om den lever, annars kollar nästa enemy i arrayen
                enemyCol = rnd.Next(0, 5);
                enemyRow = rnd.Next(0, 3);
                while (enemyArray[enemyRow, enemyCol].lives == 0)
                {
                    Debug.WriteLine("enemy at " + enemyRow + " , " + enemyCol + " is dead, finding new enemy");
                    if (enemyCol < 4) { enemyCol++; }
                    else
                    {
                        enemyCol = 0;

                        if (enemyRow < 2)
                        {
                            enemyRow++;
                        }

                        else { enemyRow = 0; }
                    }
                }

                Debug.WriteLine("shooting at " + enemyRow + " , " + enemyCol);
                enemyArray[enemyRow, enemyCol].Shoot();
                enemyShootingTimer.ResetAndStart(1.5);

            }

            //player shooting logic
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if (shootingTimer.IsDone())
                {
                    bulletStartPosition = new Vector2(playerPosition.X + 38, playerPosition.Y);
                    bullet = new Bullet(bulletTexture, collisionLayer, bulletStartPosition, bulletVelocity, windowHeight - bulletTexture.Height - 100);
                    bulletList.Add(bullet);
                    shootingTimer.ResetAndStart(0.6);
                }
            }


            //updating player bullets and checking collision with enemies
            foreach (Bullet _bullet in bulletList)
            {
                bulletPosition = _bullet.Update();

                foreach (Enemy _enemy in enemyArray)
                {
                    if (_bullet.rect.Intersects(_enemy.rect) && _enemy.lives > 0)
                    {
                        bulletTrash.Add(_bullet);
                        _enemy.lives -= 1;
                        _score += 100;
                    }
                }
            }


            //removing bullets that hit enemy
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
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            player.Draw(_spriteBatch);

            foreach (Bullet _bullet in bulletList)
            {
                _bullet.Draw(_spriteBatch);
            }

            foreach (Enemy _enemy in enemyArray)
            {
                _enemy.Draw(_spriteBatch);
                if (_enemy.bullet != null)
                {
                    _enemy.bullet.Draw(_spriteBatch);
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
