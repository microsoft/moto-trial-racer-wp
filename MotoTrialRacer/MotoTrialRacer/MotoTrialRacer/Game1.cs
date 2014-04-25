/**
 * Copyright (c) 2011-2014 Microsoft Mobile and/or its subsidiary(-ies).
 * All rights reserved.
 *
 * For the applicable distribution terms see the license text file included in
 * the distribution.
 */

//#define MEASURE_FPS //Uncomment this line if you want to measure the performance

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using MotoTrialRacer.UI;

namespace MotoTrialRacer
{
    /// <summary>
    /// The main class for the game. Handles the game mechanics, like timing,
    /// statring the game, failing and winning. Updates and draws all of the
     /// other components.
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //public static Rectangle screen = new Rectangle(0, 0, 800, 480);

		public static GraphicsDeviceManager Graphics { get; private set; }
        private SpriteBatch spriteBatch;
        private Level level;
        private Texture2D background;
        private View view;
        private Button optionsButton;
        private Button exitButton;
        private Button leftButton;
        private Button rightButton;
        private Matrix transform = Matrix.Identity;
        private AudioPlayer audioPlayer;
        public TimeSpan finishTime;
        private Stopwatch stopwatch = new Stopwatch();
        private String clockTime = "00:00";
        private Vector2 clockPos;
        private SpriteFont font;
        public float scale = 1;
        public bool paused = false;
        private bool failed = false;
        public bool won = false;
        private bool editorOpen = false;
        public String currentLevelName = "";

#if MEASURE_FPS
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int frameRate = 0;
        private int frameCounter = 0;
#endif
            
        public int getWidth()
        {
            return GraphicsDevice.Viewport.Width;
        }

        public int getHeight()
        {
            return GraphicsDevice.Viewport.Height;
        }

        public float relativeX( float xin )
        {
            return xin / 800.0f * (float)getWidth();
        }

        public float relativeY(float yin)
        {
            return yin / 480.0f * (float)getHeight();
        }

        /// <summary>
        /// The constuctor for the game. Called by the XNA framework
        /// </summary>
        public Game1()
        {
			Graphics = new GraphicsDeviceManager(this);
			Graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft;
			Graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone but that is 
            // too slow for Box2D so the target frame rate is set to 60 fps.
            TargetElapsedTime = TimeSpan.FromTicks(166666);
        }

        /// <summary>
        /// Closes views on top the game world
        /// </summary>
        public void CloseView()
        {
            view = null;
        }

        /// <summary>
        /// Resumes to the game if it has been paused
        /// </summary>
        public void Resume()
        {
            paused = false;
            audioPlayer.StopMusic();
            audioPlayer.PlayMotor();
            view = null;
            stopwatch.Start();
        }

        /// <summary>
        /// Starts a new game using a predefined level
        /// </summary>
        /// <param name="levelNumber">the number of the predefiner level (1, 2 or 3)</param>
        public void NewGame(int levelNumber)
        {
            level = new Level(levelNumber, audioPlayer, Content);
            currentLevelName = "level" + levelNumber;
            NewGame();
        }

        /// <summary>
        /// Starts a new game using a user defined custom level
        /// </summary>
        /// <param name="levelName">the name of the level </param>
        public void NewGame(String levelName)
        {
            level = new Level(levelName.Replace(' ', '_') + ".lvl", audioPlayer, Content);
            currentLevelName = levelName;
            NewGame();
        }

        /// <summary>
        /// Starts a new game after the level has been loaded.
        /// For internl use.
        /// </summary>
        private void NewGame()
        {
            level.HeadFail += new Action<Level>(HeadFail);
			level.TireFail += new Action<Level>(TireFail);
			level.Win += new Action<Level>(Win);
            level.EnableControl();
            level.StopBikeMotor();
            failed = false;
            level.ResetBike();
            paused = false;
            won = false;
            audioPlayer.StopMusic();
            audioPlayer.PlayMotor();
            view = null;
            stopwatch.Reset();
            stopwatch.Start();
        }

        /// <summary>
        /// A callback function for LevelEditor to notify that the editor should be closed
        /// </summary>
        /// <param name="sender">the caller of this function</param>
        /// <param name="args">for telling if the created level is wanted to be saved or 
        ///                    not</param>
        public void LevelCreated(bool save)
        {
            editorOpen = false;
            editorOpen = false;
            if (save)
                view = new LevelSaver(this);
            else
            {
                level = new Level(1, audioPlayer, this.Content);
                view = new Menu(this);
            }
        }

        /// <summary>
        /// Saves the user defined custom level
        /// </summary>
        /// <param name="levelName">the name of the level</param>
        /// <returns>true if the saving succeeded, false otherwise</returns>
        public bool SaveLevel(String levelName)
        {
            if (level is LevelEditor)
                if (((LevelEditor)level).SaveToFile(levelName))
                    return true;
            return false;
        }

        /// <summary>
        /// Opens the level editor
        /// </summary>
        public void StartLevelEditor()
        {
            audioPlayer.StopMusic();
            view = null;
            stopwatch.Reset();
            paused = false;
            editorOpen = true;
            level = new LevelEditor(audioPlayer, this.Content);
            ((LevelEditor)level).Ready += LevelCreated;
        }

        /// <summary>
        /// Shows LevelSelector on users custom level page
        /// </summary>
        /// <param name="newGame">for telling if we want to select the level for
        /// starting a new game or showing the high scores</param>
        public void ShowMyLevelsSelector(bool newGame)
        {
            view = new LevelSelector(this, newGame, true);
        }

        /// <summary>
        /// Shows LevelSelector on predefined level page
        /// </summary>
        /// <param name="newGame">for telling if we want to select the level bacause of
        /// starting a new game or showing the high scores</param>
        public void ShowLevelSelector(bool newGame)
        {
            view = new LevelSelector(this, newGame, false);
        }

		/// <summary>
		/// A callback function for Level to notify that the driver has hit his head
		/// </summary>
		/// <param name="level">the caller of this function</param>
		public void HeadFail(Level level)
		{
			Failed(false);
		}

        /// <summary>
        /// A callback function for Level to notify that the bike has contacted with a spike mat
        /// </summary>
		/// <param name="level">the caller of this function</param>
		public void TireFail(Level level)
        {
			Failed(true);
        }

		private void Failed(bool tireFailed)
		{
			if (view == null)
			{
				if (!failed)
				{
					failed = true;

					if (tireFailed)
						audioPlayer.PlayBlast();
					else
						audioPlayer.PlayOuch(); // head failed

					audioPlayer.StopMotor();
					view = new Menu(this);
					audioPlayer.PlayMusic();
				}
			}
		}

        /// <summary>
        /// A callback function for Level to notify that the player has got to the finish
        /// </summary>
		/// <param name="level">the caller of this function</param>
        public void Win(Level level)
        {
            if (view == null)
            {
                paused = true;
                stopwatch.Stop();
                finishTime = stopwatch.Elapsed;
                audioPlayer.PlayFanfare();
                audioPlayer.StopMotor();
                view = new WinView(this);
                won = true;
            }
        }

        /// <summary>
        /// Open the main menu
        /// </summary>
        public void OpenMenu()
        {
            view = new Menu(this);
            audioPlayer.StopMotor();
            audioPlayer.StopFanfare();
            audioPlayer.PlayMusic();
        }

        /// <summary>
        /// Pauses the game
        /// </summary>
        public void Pause()
        {
            stopwatch.Stop();
            paused = true;
			OpenMenu();
        }

        /// <summary>
        /// Shows the high scores for the selected level
        /// </summary>
        /// <param name="levelName">the name of the level which high scores we want to show</param>
        public void ShowHighScores(String levelName)
        {
            view = new HighScores(this, levelName);
        }

        /// <summary>
        /// Shows InfoView
        /// </summary>
        public void ShowInfo()
        {
            view = new InfoView(this);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            audioPlayer = new AudioPlayer(Content);
            level = new Level(1, audioPlayer, this.Content);
			level.HeadFail += new Action<Level>(HeadFail);
			level.TireFail += new Action<Level>(TireFail);
			level.Win += new Action<Level>(Win);
            background = this.Content.Load<Texture2D>("Images/sky");
            font = this.Content.Load<SpriteFont>("SpriteFont3");
            view = new SplashScreen(this);
            optionsButton = new Button("options", 
                                        new Vector2(getWidth() * 0.015f, getHeight() * 0.89f),
                                                    this.Content);
			optionsButton.ButtonPressed += (sender => Pause());

            float aspect = (float)optionsButton.Width / (float)optionsButton.Height;
            optionsButton.Height = (int)(0.1f * getHeight());
            optionsButton.Width = (int)(aspect * optionsButton.Height);
            exitButton = new Button("exit", 
                                    new Vector2(getWidth() * 0.89f, getHeight() * 0.88f),
                                    this.Content);
			exitButton.ButtonPressed += (sender => Exit());

            aspect = (float)exitButton.Width / (float)exitButton.Height;
            exitButton.Height = (int)(0.1f * (float)getHeight());
            exitButton.Width = (int)(aspect * (float)exitButton.Height);
            leftButton = new Button("left", new Vector2(10, 20), 
                                    this.Content, true);
			leftButton.ButtonPressed += (sender => level.leanBikeBackwards());
            leftButton.Height = 70;
            
			rightButton = new Button("right", new Vector2(relativeX(735), relativeY(20)), 
                                     this.Content, true);
            rightButton.Height = 70;
			rightButton.ButtonPressed += (sender => level.leanBikeForwards());

            clockPos = new Vector2(0.45f * getWidth(), 0.89f * getHeight());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            this.Content.Unload();
        }

        /// <summary>
        /// The main update method for the game loop. Calls all the lower level update methods
        /// </summary>
        /// <param name="gameTime">snapshot of the game timing state</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

#if MEASURE_FPS
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
#endif
            if (view != null)
            {
                level.disableBikeControls();
                view.Update();
            }
            else if (!editorOpen)
            {
                TouchCollection touchCollection = TouchPanel.GetState();
                if (touchCollection.Count() > 0)
                {
                    optionsButton.Update(touchCollection[0]);
                    exitButton.Update(touchCollection[0]);
                    leftButton.Update(touchCollection[0]);
                    rightButton.Update(touchCollection[0]);
                }
            }
            if (!paused)
            {
                level.Update();
            }

			clockTime = String.Format("{0:d2}:{1:d2}", stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds);
        }

        /// <summary>
        /// The main draw method for the game loop. Calls all the lower level draw methods
        /// </summary>
        /// <param name="gameTime">snapshot of the game timing state</param>
        protected override void Draw(GameTime gameTime)
        {
#if MEASURE_FPS
            frameCounter++;
#endif
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            Rectangle screen = new Rectangle(0, 0, getWidth(), getHeight());
            spriteBatch.Draw(background, screen, Color.White);

            level.Draw(spriteBatch);
            
			if (view != null)
				view.Draw(spriteBatch);
            else if (!editorOpen)
            {
                spriteBatch.DrawString(font, clockTime, clockPos, Color.Yellow);
				optionsButton.Draw(spriteBatch);
				exitButton.Draw(spriteBatch);
				leftButton.Draw(spriteBatch);
				rightButton.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
