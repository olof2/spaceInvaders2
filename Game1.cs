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
        public enum GameState { Menu, Game, GameOver }
        GameState gameState = GameState.Menu;
        int frame;
        double frameTimer, frameInterval;

        SpriteFont spriteFont;
        Vector2 textPosition;
        Vector2 scorePosition;

        Texture2D playerTexture;
        Texture2D bulletTexture;
        Texture2D enemyTexture1;
        Texture2D enemyTexture2;
        Texture2D menuBackground;
        Texture2D gameOverBackground;
        Texture2D explosion;
        Rectangle explosionRect;

        Texture2D rndStar;
        Vector2 rndStarPos;
        List<Vector2> rndStarList;

        Texture2D startButton;
        Rectangle startButtonRect;
        Vector2 startButtonPos;
        System.Drawing.Point mousePos;
        Point mousePoint;

        Vector2 backgroundOrigin1;
        Vector2 backgroundOrigin2;
        Vector2 backgroundOrigin3;

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
        bool flipDirection;
        int edgeBoost;
        int enemyLivesSum;

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
            frameTimer = 100; 
            frameInterval = 100;

            spriteFont = Content.Load<SpriteFont>("spritefont1");
            textPosition = new Vector2(windowWidth / 2 - 100, windowHeight / 2);
            scorePosition = new Vector2(40, windowHeight - 45);

            playerTexture = Content.Load<Texture2D>(@"Ship_01-1");
            bulletTexture = Content.Load<Texture2D>(@"Bullet");
            enemyTexture1 = Content.Load<Texture2D>(@"alien03_single");
            enemyTexture2 = Content.Load<Texture2D>(@"alien02_single"); 
            menuBackground = Content.Load<Texture2D>("space_light");
            gameOverBackground = Content.Load<Texture2D>("Stars_panorama_sheet");

            explosion = Content.Load<Texture2D>("explotion01_sprites");
            explosionRect = new Rectangle(0, 0, 110, 100);
            startButton = Content.Load<Texture2D>("Startknapp");
            startButtonPos = new Vector2(windowWidth / 2 - startButton.Width / 2, 300);
            startButtonRect = new Rectangle((int)startButtonPos.X, (int)startButtonPos.Y, startButton.Width, startButton.Height);
            mousePos = new System.Drawing.Point(0,0);

            rndStar = Content.Load<Texture2D>("explosion");
            rndStarList = new List<Vector2>();
            for (int i = 0; i < 7; i++)
            {
                int rndX = new Random().Next(windowWidth);
                int rndY = new Random().Next(windowHeight);
                rndStarPos = new Vector2(rndX, rndY);
                rndStarList.Add(rndStarPos);
            }

            //Vectors to place multiple backgrounds in gameover screen instead of scaling
            backgroundOrigin1 = new Vector2(gameOverBackground.Width, 50);
            backgroundOrigin2 = new Vector2(gameOverBackground.Width*2, 10);
            backgroundOrigin3 = new Vector2(gameOverBackground.Width*3, 20);


            playerPosition = new Vector2(windowWidth/2, windowHeight - 20 - playerTexture.Height);
            playerVelocity = new Vector2(5, 5);
            lives = 3;
            player = new Player(playerTexture, playerPosition, playerVelocity, windowWidth, windowHeight, lives);


            enemyPosition = new Vector2(50, 50);
            enemyVelocity = new Vector2(20, 20);
            enemyArray = new Enemy[5,5];
            rnd = new Random();
            enemyCol = 4;
            enemyRow = 4;
            flipDirection = false;
            edgeBoost = 0;
            enemyLivesSum = 0;

            //spawning enemies in ARRAY, 5 rows of 5
            for (int i = 0; i < 5; i++)
            {
                //first 2 rows, enemies get 2 lives
                if (i <= 1)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        enemyPosition.Y = 20 + 100 * i;
                        enemyPosition.X = 20 + k * 10 + k * enemyTexture1.Width;
                        enemy = new Enemy(enemyTexture2, bulletTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth, 2);
                        enemyArray[i, k] = enemy;
                    }
                }
                else
                {
                    for (int k = 0; k < 5; k++)
                    {
                        enemyPosition.Y = 20 + 100 * i;
                        enemyPosition.X = 20 + k * 10 + k * enemyTexture1.Width;
                        enemy = new Enemy(enemyTexture1, bulletTexture, enemyPosition, enemyVelocity, windowHeight, windowWidth, 1);
                        enemyArray[i, k] = enemy;
                    }
                }
            }

            bulletList = new List<Bullet>();
            bulletTrash = new List<Bullet>();
            bulletVelocity = new Vector2(0, -10);
            bulletPosition = new Vector2(0, 0);
        }


        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit with ESC
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }


            //updating title with score and lives
            Window.Title = _title;
            _title = "Spaceinvaders, score: " + _score + ", liv: " + lives;

            //gamestate MENU
            if (gameState == GameState.Menu)
            {
                //space to start
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    gameState = GameState.Game;
                }

                //clicking button to start
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    mousePos = new System.Drawing.Point(Mouse.GetState().X, Mouse.GetState().Y);
                    mousePoint = new Point(mousePos.X, mousePos.Y);
                    if (startButtonRect.Contains(mousePoint))
                    {
                        gameState = GameState.Game;
                    }
                }

                //animating explosion sprite
                frameTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (frameTimer <= 0)
                {
                    frame++;
                    frameTimer = frameInterval;
                    if (frame >= 5) { frame = 0; }
                    explosionRect.X = frame * 110;
                }
            }

            //gamestate game
            if (gameState == GameState.Game)
            {

                playerPosition = player.Update();
                shootingTimer.Update(gameTime.ElapsedGameTime.TotalSeconds);
                enemyShootingTimer.Update(gameTime.ElapsedGameTime.TotalSeconds);
                enemyLivesSum = 0;


                //enemy update logic
                flipDirection = false;
                foreach (Enemy _enemy in enemyArray)
                {
                    enemyPosition = _enemy.Update(gameTime.ElapsedGameTime.TotalSeconds);

                    //if enemy reached edge, flip direction, setting edgeboost to safeguard from enemy getting stuck at edge
                    if (enemyPosition.X >= windowWidth - enemyTexture1.Width)
                    {
                        flipDirection = true;
                        edgeBoost = -20;
                    }
                    else if (enemyPosition.X <= 0)
                    {
                        flipDirection = true;
                        edgeBoost = 20;
                    }

                    //checking if enemy reached bottom of screen, if so, player loses a life and enemy dies
                    if (enemyPosition.Y > windowHeight - 100)
                    {
                        lives -= 1;
                        _enemy.lives = 0;
                        _enemy.position.Y = 200;
                        if (lives <= 0) { gameState = GameState.GameOver; }
                    }

                    //updating enemy bullets and checking collision with player
                    if (_enemy.bullet != null)
                    {
                        _enemy.bullet.Update();
                        if (_enemy.bullet.rect.Intersects(player.rect))
                        {
                            lives -= 1;
                            _enemy.bullet = null;
                            if (lives <= 0) { gameState = GameState.GameOver; }
                        }
                    }

                }

                //if flipdirection just happend, move all enemies down and flip direction
                if (flipDirection == true)
                {
                    foreach (Enemy _e in enemyArray)
                    {
                        _e.position.X += edgeBoost;
                        _e.JumpDown();
                        _e.flipDirection();
                    }
                    flipDirection = false;
                }

                //enemy shooting logic
                if (enemyShootingTimer.IsDone())
                {
                    //slumpar fram en enemy som ska skjuta, kollar om den lever, annars kollar nästa enemy i arrayen
                    enemyCol = rnd.Next(0, 5);
                    enemyRow = rnd.Next(0, 5);
                    while (enemyArray[enemyRow, enemyCol].lives == 0)
                    {
                        Debug.WriteLine("enemy at " + enemyRow + " , " + enemyCol + " is dead, finding new enemy");
                        if (enemyCol < 4) { enemyCol++; }
                        else
                        {
                            enemyCol = 0;

                            if (enemyRow < 4)
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
                            if (_enemy.lives == 0) { _enemy.position.X = 200; }
                        }
                    }
                }

                //check if any enemies are alive, if not, go to gameover (sum set to 0 at start of each frame)
                foreach (Enemy _enemy in enemyArray)
                {
                    enemyLivesSum += _enemy.lives;
                }
                if (enemyLivesSum <= 0) { gameState = GameState.GameOver; }


                //removing bullets that hit enemy
                foreach (Bullet _b in bulletTrash) { bulletList.Remove(_b); }


                //removing bullets that reached end 
                for (int i = 0; i < bulletList.Count; i++)
                {
                    _bull = bulletList[i];
                    if (_bull.position.Y < _bull.endPosition.Y)
                    {
                        bulletList.RemoveAt(i);
                    }
                }
            }

            //gamestate gameover
            if (gameState == GameState.GameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    //resetting game variables
                    lives = 3;
                    _score = 0;
                    shootingTimer.ResetAndStart(0.6);
                    enemyShootingTimer.ResetAndStart(1.0);
                    //resetting enemies
                    enemyPosition = new Vector2(50, 50);
                    for (int i = 0; i < 5; i++)
                    {
                        if (i <= 1)
                        {
                            for (int k = 0; k < 5; k++)
                            {
                                enemyPosition.Y = 20 + 100 * i;
                                enemyPosition.X = 20 + k * 10 + k * enemyTexture1.Width;
                                enemyArray[i, k].position = enemyPosition;
                                enemyArray[i, k].lives = 2;
                            }
                        }
                        else
                        {
                            for (int k = 0; k < 5; k++)
                            {
                                enemyPosition.Y = 20 + 100 * i;
                                enemyPosition.X = 20 + k * 10 + k * enemyTexture1.Width;
                                enemyArray[i, k].position = enemyPosition;
                                enemyArray[i, k].lives = 1;
                            }
                        }
                    }
                    foreach (Enemy _e in enemyArray)
                    {
                        _e.moveTimer.ResetAndStart(0.6);
                        _e.moveDelay = 0.6;
                    }
                    bulletList.Clear();
                    bulletTrash.Clear();
                    gameState = GameState.Game;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            if (gameState == GameState.Menu)
            {
                _spriteBatch.Draw(menuBackground, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                _spriteBatch.DrawString(spriteFont, "click button or press SPACE to start", textPosition, Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                _spriteBatch.Draw(explosion, new Vector2(100, 500), explosionRect, Color.White);
                _spriteBatch.Draw(startButton, startButtonPos, Color.White);
            }

            if (gameState == GameState.Game)
            {
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

                _spriteBatch.DrawString(spriteFont, "score: " + _score + ", liv: " + lives, scorePosition, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 1);
            }

            if (gameState == GameState.GameOver)
            {
                foreach (Vector2 pos in rndStarList)
                {
                    _spriteBatch.Draw(rndStar, pos, Color.White);
                }
                _spriteBatch.Draw(gameOverBackground, Vector2.Zero, null, Color.White);
                _spriteBatch.Draw(gameOverBackground, backgroundOrigin1, Color.Yellow);
                _spriteBatch.Draw(gameOverBackground, backgroundOrigin2, Color.DarkGray);
                _spriteBatch.Draw(gameOverBackground, backgroundOrigin3, Color.Turquoise);
                _spriteBatch.DrawString(spriteFont, "GAME OVER", textPosition, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 1);
                _spriteBatch.DrawString(spriteFont, "press space to restart", new Vector2(textPosition.X, textPosition.Y + 80), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

