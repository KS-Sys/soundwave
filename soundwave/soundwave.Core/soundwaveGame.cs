using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using soundwave.Core.Localization;
using static System.Net.Mime.MediaTypeNames;

namespace soundwave.Core
{
    /// <summary>
    /// The main class for the game, responsible for managing game components, settings, 
    /// and platform-specific configurations.
    /// </summary>
    public class soundwaveGame : Game
    {
        //private Texture2D _texture;
        private Texture2D [] _rflag_array;
        private Texture2D [] _bflag_array;
        private Vector2 [] _rposition_array;
        private Vector2 [] _bposition_array;
        public int _rposition_index = 0;
        private int _bposition_index = 0;
        private int _rposition_offset = 200;
        private int _bposition_offset = 600;
        private Vector2 _position;
        private Vector2 _position2;
        // Resources for drawing.
        private GraphicsDeviceManager graphicsDeviceManager;
        SpriteBatch spriteBatch;

        public float Speed = 2f;
        public float lambda_wavespeed = 2f;
        public float waveamplitude = 100f;

        /// <summary>
        /// Indicates if the game is running on a mobile platform.
        /// </summary>
        public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();

        /// <summary>
        /// Indicates if the game is running on a desktop platform.
        /// </summary>
        public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

        /// <summary>
        /// Initializes a new instance of the game. Configures platform-specific settings, 
        /// initializes services like settings and leaderboard managers, and sets up the 
        /// screen manager for screen transitions.
        /// </summary>
        public soundwaveGame()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Share GraphicsDeviceManager as a service.
            Services.AddService(typeof(GraphicsDeviceManager), graphicsDeviceManager);

            Content.RootDirectory = "Content";

            // Configure screen orientations.
            graphicsDeviceManager.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Load supported languages and set the default language.
            List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
            var languages = new List<CultureInfo>();
            for (int i = 0; i < cultures.Count; i++)
            {
                languages.Add(cultures[i]);
            }

            // TODO You should load this from a settings file or similar,
            // based on what the user or operating system selected.
            var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
            LocalizationManager.SetCulture(selectedLanguage);
        }

        /// <summary>
        /// Loads game content, such as textures and particle systems.
        /// </summary>
        protected override void LoadContent()
        {
            
            base.LoadContent();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // I load the flag textures and set their initial positions here
            _rflag_array = new Texture2D[10];
            _bflag_array = new Texture2D[10];
            _rposition_array = new Vector2[10];
            _bposition_array = new Vector2[10];
            for (int i = 0; i < 9; i++)
            {
              
            _rflag_array[i] = Content.Load<Texture2D>("reflag");
            _bflag_array[i] = Content.Load<Texture2D>("blflag");


            _rposition_array[i] = new Vector2(_rposition_offset, _rposition_index);
            _rposition_index += 60;

            _bposition_array[i] = new Vector2(_bposition_offset, _bposition_index);
            _bposition_index += 60;

            }

        }

        /// <summary>
        /// Updates the game's logic, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for game updates.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // Get screen height for boundary check as flags move down and reset to top
            int screenheight = GraphicsDevice.Viewport.Height;
            

            base.Update(gameTime);

            // Update positions of flags with wave effect and speed increase over time.
            for (int i = 0; i < 9; i++)
            {
                _rposition_array[i].Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _bposition_array[i].Y += Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float time = (float)gameTime.TotalGameTime.TotalSeconds;

                // Calculate wave effect using cosine function for smooth oscillation.
                float currentwave = (float)Math.Cos((time * lambda_wavespeed) + i);

                _rposition_array[i].X += currentwave * waveamplitude * deltaTime;
                _bposition_array[i].X += currentwave * waveamplitude * deltaTime;

                // Reset flag position to top if it moves beyond the screen height.
                if (_rposition_array[i].Y >= screenheight)
                {
                    _rposition_array[i].Y = -60;
                    _bposition_array[i].Y = -60;
                }
            }

            // Gradually increase speed and wave amplitude over time.
            Speed += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            waveamplitude += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //lambda_wavespeed += 5f * (float)gameTime.ElapsedGameTime.TotalSeconds;


        }

        /// <summary>
        /// Draws the game's graphics, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for rendering.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the MonoGame orange color before drawing.
            GraphicsDevice.Clear(Color.LightBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            for(int i = 0; i < 9; i++)
            {
                spriteBatch.Draw(_rflag_array[i], _rposition_array[i], Color.White);
                spriteBatch.Draw(_bflag_array[i], _bposition_array[i], Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
            
        }
    }
}