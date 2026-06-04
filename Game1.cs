using Cocos2D;
using CocosDenshion;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FlappyBird
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        const double GRAVITY = 0.5165;
        const double FLAP_SPEED = -7;
        const int TOP_PIPE_MAX_Y = -219;
        const int TOP_PIPE_MIN_Y = -500;
        const int TOP_PIPE_START_X = 700;
        const int PIPE_HORIZONTAL_SPACING = 220;
        const int PIPE_VERTICAL_SPACING = 220;
        const int PIPE_HEIGHT = 600;
        const int PIPE_WIDTH = 80;
        const int PIPE_SPEED = 5;
        const int GROUND_WIDTH = 1200;
        const int GROUND_HEIGHT = 100;
        const int BACKGROUND_WIDTH = 1200;
        const int BACKGROUND_HEIGHT = 700;

        double vertSpeed;

        Texture2D restart;
        Rectangle restartRect;

        Texture2D topPipe;
        Rectangle[] topPipeRects;

        Texture2D bottomPipe;
        Rectangle[] bottomPipeRects;

        Texture2D background;
        Rectangle[] backgroundRects;

        Texture2D ground;
        Rectangle[] groundRects;

        Texture2D character;
        Rectangle characterRect;
        double characterActualY;

        bool atGameStart;
        bool atPlaying;
        bool atLose;
        bool atMenu;
        bool lastMouseState;

        Texture2D toMenu;
        Rectangle toMenuRect;

        Texture2D menuScreen;
        Rectangle menuScreenRect;
        Texture2D toGame;
        Rectangle toGameRect;
        Texture2D flappyBird;
        Rectangle flappyBirdRect;
        Texture2D flappy2;
        Rectangle flappy2Rect;

        Random rand = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 800;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            characterRect = new Rectangle(20, 240, 76, 54);
            characterActualY = 240;
            vertSpeed = 0;

            atGameStart = true;
            atPlaying = false;
            atLose = false;
            atMenu = false;
            lastMouseState = false;

            topPipeRects = new Rectangle[]
            {
                new Rectangle(TOP_PIPE_START_X, rand.Next(TOP_PIPE_MIN_Y, TOP_PIPE_MAX_Y), PIPE_WIDTH, PIPE_HEIGHT),
                new Rectangle(TOP_PIPE_START_X + PIPE_WIDTH + PIPE_HORIZONTAL_SPACING, rand.Next(TOP_PIPE_MIN_Y, TOP_PIPE_MAX_Y), PIPE_WIDTH, PIPE_HEIGHT),
                new Rectangle(TOP_PIPE_START_X + 2 * (PIPE_WIDTH + PIPE_HORIZONTAL_SPACING), rand.Next(TOP_PIPE_MIN_Y, TOP_PIPE_MAX_Y), PIPE_WIDTH, PIPE_HEIGHT)
            };

            bottomPipeRects = new Rectangle[]
            {
                new Rectangle(topPipeRects[0].X, topPipeRects[0].Y + PIPE_HEIGHT + PIPE_VERTICAL_SPACING, PIPE_WIDTH, PIPE_HEIGHT),
                new Rectangle(topPipeRects[1].X, topPipeRects[1].Y + PIPE_HEIGHT + PIPE_VERTICAL_SPACING, PIPE_WIDTH, PIPE_HEIGHT),
                new Rectangle(topPipeRects[2].X, topPipeRects[2].Y + PIPE_HEIGHT + PIPE_VERTICAL_SPACING, PIPE_WIDTH, PIPE_HEIGHT)
            };

            groundRects = new Rectangle[]
            {
                new Rectangle(0, 700, GROUND_WIDTH, GROUND_HEIGHT),
                new Rectangle(GROUND_WIDTH, 700, GROUND_WIDTH, GROUND_HEIGHT)
            };

            backgroundRects = new Rectangle[]
            {
                new Rectangle(0, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT),
                new Rectangle(BACKGROUND_WIDTH, 0, BACKGROUND_WIDTH, BACKGROUND_HEIGHT)
            };

            restartRect = new Rectangle(50, 250, 500, 300);
            toMenuRect = new Rectangle(50, 500, 500, 200);
            toGameRect = new Rectangle(50, 500, 500, 200);
            flappyBirdRect = new Rectangle(50, 50, 76, 54);
            flappy2Rect = new Rectangle(50, 154, 76, 54);
            menuScreenRect = new Rectangle(0, 0, 600, 800);


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            flappyBird = this.Content.Load<Texture2D>("flappyBird");
            flappy2 = this.Content.Load<Texture2D>("flappy2");
            topPipe = this.Content.Load<Texture2D>("topPipe");
            bottomPipe = this.Content.Load<Texture2D>("bottomPipe");
            ground = this.Content.Load<Texture2D>("ground");
            background = this.Content.Load<Texture2D>("background");
            restart = this.Content.Load<Texture2D>("restart");
            menuScreen = this.Content.Load<Texture2D>("menuScreen");
            toGame = this.Content.Load<Texture2D>("toGame");
            toMenu = this.Content.Load<Texture2D>("toMenu");

            character = flappyBird;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                ExitGame();
            }

            // TODO: Add your update logic here
            KeyboardState keys = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (atGameStart)
            {
                AtGameStartUpdate(keys, mouse);
            }
            else if (atPlaying)
            {
                AtPlayingUpdate(keys);
            }
            else if (atLose)
            {
                AtLoseUpdate(mouse);
            }
            else if (atMenu)
            {
                AtMenuUpdate(mouse);
            }

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                lastMouseState = true;
            }
            else
            {
                lastMouseState = false;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (atGameStart)
            {
                AtGameStartDraw(spriteBatch);
            }
            if (atPlaying)
            {
                AtPlayingDraw(spriteBatch);
            }
            if (atLose)
            {
                AtLoseDraw(spriteBatch);
            }
            if (atMenu)
            {
                AtMenuDraw(spriteBatch);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void ExitGame()
        {
            // TODO: add your exit code here to restore the device to its per-game environment.
            CCSimpleAudioEngine.SharedEngine.RestoreMediaState();
            Exit();
        }

        protected void AtGameStartUpdate(KeyboardState keys, MouseState mouse)
        {
            if (keys.IsKeyDown(Keys.Space))
            {
                atGameStart = false;
                atPlaying = true;
                vertSpeed = FLAP_SPEED;
            }

            if (mouse.LeftButton == ButtonState.Pressed
                && lastMouseState == false
                && mouse.X >= toMenuRect.X
                && mouse.X <= toMenuRect.X + toMenuRect.Width
                && mouse.Y >= toMenuRect.Y
                && mouse.Y <= toMenuRect.Y + toMenuRect.Height)
            {
                atGameStart = false;
                atMenu = true;
            }
        }
        protected void AtGameStartDraw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < backgroundRects.Length; i++)
            {
                spriteBatch.Draw(background, backgroundRects[i], Color.White);
            }
            for (int i = 0; i < groundRects.Length; i++)
            {
                spriteBatch.Draw(ground, groundRects[i], Color.White);
            }
            spriteBatch.Draw(character, characterRect, Color.White);
            spriteBatch.Draw(toMenu, toMenuRect, Color.White);
        }

        protected void AtPlayingUpdate(KeyboardState keys)
        {
            vertSpeed += GRAVITY;
            characterActualY += vertSpeed;
            characterRect.Y = (int)characterActualY;

            if (keys.IsKeyDown(Keys.Space))
            {
                vertSpeed = FLAP_SPEED;
            }

            MovePipes();
            MoveTextures();

            // flappy bird "faints" when we hit the top of the screen
            // when we hit any pipe
            // when we hit the ground
            if(characterRect.Y < 0)
            {
                atLose = true;
                atPlaying = false;
            }
            for(int i = 0; i < topPipeRects.Length; i++)
            {
                if (characterRect.Intersects(topPipeRects[i]))
                {
                    atLose = true;
                    atPlaying = false;
                }
                if (characterRect.Intersects(bottomPipeRects[i]))
                {
                    atLose = true;
                    atPlaying = false;
                }
            }
            for (int i = 0; i < groundRects.Length; i++)
            {
                if (characterRect.Intersects(groundRects[i]))
                {
                    atLose = true;
                    atPlaying = false;
                }
            }
        }

        protected void AtPlayingDraw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < backgroundRects.Length; i++)
            {
                spriteBatch.Draw(background, backgroundRects[i], Color.White);
            }
            for (int i = 0; i < topPipeRects.Length; i++)
            {
                spriteBatch.Draw(topPipe, topPipeRects[i], Color.White);
                spriteBatch.Draw(bottomPipe, bottomPipeRects[i], Color.White);
            }
            for (int i = 0; i < groundRects.Length; i++)
            {
                spriteBatch.Draw(ground, groundRects[i], Color.White);
            }
            spriteBatch.Draw(character, characterRect, Color.White);
        }

        protected void AtLoseUpdate(MouseState mouse)
        {
            if(mouse.LeftButton == ButtonState.Pressed
                && lastMouseState == false
                && mouse.X >= restartRect.X
                && mouse.X <= restartRect.X + restartRect.Width
                && mouse.Y >= restartRect.Y
                && mouse.Y <= restartRect.Y + restartRect.Height)
            {
                atLose = false;
                atGameStart = true;

                characterRect = new Rectangle(20, 240, 76, 54);
                characterActualY = 240;
                vertSpeed = 0;

                topPipeRects = new Rectangle[]
                {
                    new Rectangle(TOP_PIPE_START_X, rand.Next(TOP_PIPE_MIN_Y, TOP_PIPE_MAX_Y), PIPE_WIDTH, PIPE_HEIGHT),
                    new Rectangle(TOP_PIPE_START_X + PIPE_WIDTH + PIPE_HORIZONTAL_SPACING, rand.Next(TOP_PIPE_MIN_Y, TOP_PIPE_MAX_Y), PIPE_WIDTH, PIPE_HEIGHT),
                    new Rectangle(TOP_PIPE_START_X + 2 * (PIPE_WIDTH + PIPE_HORIZONTAL_SPACING), rand.Next(TOP_PIPE_MIN_Y, TOP_PIPE_MAX_Y), PIPE_WIDTH, PIPE_HEIGHT)
                };

                bottomPipeRects = new Rectangle[]
                {
                    new Rectangle(topPipeRects[0].X, topPipeRects[0].Y + PIPE_HEIGHT + PIPE_VERTICAL_SPACING, PIPE_WIDTH, PIPE_HEIGHT),
                    new Rectangle(topPipeRects[1].X, topPipeRects[1].Y + PIPE_HEIGHT + PIPE_VERTICAL_SPACING, PIPE_WIDTH, PIPE_HEIGHT),
                    new Rectangle(topPipeRects[2].X, topPipeRects[2].Y + PIPE_HEIGHT + PIPE_VERTICAL_SPACING, PIPE_WIDTH, PIPE_HEIGHT)
                };
            }
        }
        
        protected void AtLoseDraw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < backgroundRects.Length; i++)
            {
                spriteBatch.Draw(background, backgroundRects[i], Color.White);
            }
            for (int i = 0; i < topPipeRects.Length; i++)
            {
                spriteBatch.Draw(topPipe, topPipeRects[i], Color.White);
                spriteBatch.Draw(bottomPipe, bottomPipeRects[i], Color.White);
            }
            for (int i = 0; i < groundRects.Length; i++)
            {
                spriteBatch.Draw(ground, groundRects[i], Color.White);
            }
            spriteBatch.Draw(character, characterRect, Color.White);
            spriteBatch.Draw(restart, restartRect, Color.White);
        }

        protected void AtMenuUpdate(MouseState mouse)
        {
            if (mouse.LeftButton == ButtonState.Pressed
                && lastMouseState == false
                && mouse.X >= toGameRect.X
                && mouse.X <= toGameRect.X + toGameRect.Width
                && mouse.Y >= toGameRect.Y
                && mouse.Y <= toGameRect.Y + toGameRect.Height)
            {
                atGameStart = true;
                atMenu = false;
            }
            if (mouse.LeftButton == ButtonState.Pressed
                && lastMouseState == false
                && mouse.X >= flappyBirdRect.X
                && mouse.X <= flappyBirdRect.X + flappyBirdRect.Width
                && mouse.Y >= flappyBirdRect.Y
                && mouse.Y <= flappyBirdRect.Y + flappyBirdRect.Height)
            {
                character = flappyBird;
            }
            if (mouse.LeftButton == ButtonState.Pressed
                && lastMouseState == false
                && mouse.X >= flappy2Rect.X
                && mouse.X <= flappy2Rect.X + flappy2Rect.Width
                && mouse.Y >= flappy2Rect.Y
                && mouse.Y <= flappy2Rect.Y + flappy2Rect.Height)
            {
                character = flappy2;
            }
        }
        protected void AtMenuDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(menuScreen, menuScreenRect, Color.White);
            if(character == flappyBird)
            {
                spriteBatch.Draw(flappyBird, flappyBirdRect, Color.White);
                spriteBatch.Draw(flappy2, flappy2Rect, Color.White * 0.5f);
            }
            else
            {
                spriteBatch.Draw(flappyBird, flappyBirdRect, Color.White * 0.5f);
                spriteBatch.Draw(flappy2, flappy2Rect, Color.White);
            }
            spriteBatch.Draw(toGame, toGameRect, Color.White);
        }

        protected void MovePipes()
        {
            for (int i = 0; i < topPipeRects.Length; i++)
            {
                topPipeRects[i].X -= PIPE_SPEED;
                bottomPipeRects[i].X -= PIPE_SPEED;

                if (topPipeRects[i].X < -1 * PIPE_WIDTH)
                {
                    topPipeRects[i].X = topPipeRects[(i - 1 + topPipeRects.Length) % topPipeRects.Length].X + PIPE_WIDTH + PIPE_HORIZONTAL_SPACING;
                    topPipeRects[i].Y = rand.Next(TOP_PIPE_MIN_Y, TOP_PIPE_MAX_Y);
                    bottomPipeRects[i].X = topPipeRects[i].X;
                    bottomPipeRects[i].Y = topPipeRects[i].Y + PIPE_HEIGHT + PIPE_VERTICAL_SPACING;
                }
            }
        }

        protected void MoveTextures()
        {
            for (int i = 0; i < groundRects.Length; i++)
            {
                groundRects[i].X -= PIPE_SPEED;
            }
            for (int i = 0; i < groundRects.Length; i++)
            {
                if (groundRects[i].X < -1 * GROUND_WIDTH)
                {
                    groundRects[i].X = groundRects[(i - 1 + groundRects.Length) % groundRects.Length].X + GROUND_WIDTH;
                }
            }
            for (int i = 0; i < backgroundRects.Length; i++)
            {
                backgroundRects[i].X -= PIPE_SPEED;
            }
            for (int i = 0; i < backgroundRects.Length; i++)
            {
                if (backgroundRects[i].X < -1 * BACKGROUND_WIDTH)
                {
                    backgroundRects[i].X = backgroundRects[(i - 1 + backgroundRects.Length) % backgroundRects.Length].X + BACKGROUND_WIDTH;
                }
            }
        }

    }
}
