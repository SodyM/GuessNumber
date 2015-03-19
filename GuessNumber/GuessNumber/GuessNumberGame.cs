using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Audio;

namespace GuessNumber
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GuessNumber : Microsoft.Xna.Framework.Game
    {
        // constants
        const int WINDOW_WIDTH      = 800;
        const int WINDOW_HEIGHT     = 600;
        const string SOUNDS         = @"Content\sounds.xgs";
        const string WAVE_BANK      = @"Content\Wave Bank.xwb";
        const string AUDION_BANK    = @"Content\Sound Bank.xsb";
        const string INFO_IMAGE     = "openingscreen";
        const string NEW_GAME_SOUND = "newGame";

        // variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // game state
        GameState gameState = GameState.Menu;

        // ui assets
        Texture2D openingScreen;
        Rectangle openingScreenRectangle;

        NumberBoard board;
        Random rnd = new Random();

        // audio
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;

        /// <summary>
        /// Constructor
        /// </summary>
        public GuessNumber()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;

            IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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

            // load audio content
            audioEngine = new AudioEngine(SOUNDS);
            waveBank = new WaveBank(audioEngine, WAVE_BANK);
            soundBank = new SoundBank(audioEngine, AUDION_BANK);

            openingScreen = Content.Load<Texture2D>(INFO_IMAGE);
            openingScreenRectangle = new Rectangle(0, 0, openingScreen.Width, openingScreen.Height);
          
            StartGame();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // change game state if game state is GameState.Menu and user presses Enter
            if (gameState == GameState.Menu && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                gameState = GameState.Play;
            }

            // if we're actually playing, update mouse state and update board
            if (gameState == GameState.Play)
            {
                bool correctNumberHit = board.Update(gameTime, Mouse.GetState());
                if (correctNumberHit)
                {
                    soundBank.PlayCue(NEW_GAME_SOUND);
                    StartGame();
                }
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

            spriteBatch.Begin();
            if (gameState == GameState.Menu)
            {
                spriteBatch.Draw(openingScreen, openingScreenRectangle, Color.White);
            }
            else if (gameState == GameState.Play)
            {
                board.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Starts a game
        /// </summary>
        void StartGame()
        {
            // randomly generate new number for game
            int number = rnd.Next(1, 10);

            // create the board object (this will be moved before you're done)
            board = new NumberBoard(Content, new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2),
                        (int)(graphics.PreferredBackBufferHeight * 0.8), number, soundBank);
        }
    }
}