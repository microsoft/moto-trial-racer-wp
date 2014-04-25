/**
 * Copyright (c) 2011-2014 Microsoft Mobile and/or its subsidiary(-ies).
 * All rights reserved.
 *
 * For the applicable distribution terms see the license text file included in
 * the distribution.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;
using Box2D.XNA;
using MotoTrialRacer.UI;
using MotoTrialRacer.Components;
using System.IO;

namespace MotoTrialRacer
{
    /// <summary>
    /// The class that defines the level editor.
    /// Derives from Level.
    /// Implement the ButtonListener interface.
    /// </summary>
    class LevelEditor : Level
    {
		public event Action<bool> Ready;

        private Button grassButton;
        private Vector2 grassButtonPos = new Vector2(190, 10);
        private Button jumpButton;
        private Vector2 jumpButtonPos = new Vector2(350, 10);
        private Button nailButton;
        private Vector2 nailButtonPos = new Vector2(530, 10);
        private Button finishButton;
        private Vector2 finishButtonPos = new Vector2(650, 10);
        private Button testButton;
        private Vector2 testButtonPos = new Vector2(350, 430);
        private Button saveButton;
        private Vector2 saveButtonPos = new Vector2(685, 430);
        private Button closeButton;
        private Vector2 closeButtonPos = new Vector2(310, 400);
        private Button backButton;
        private Vector2 backButtonPos = new Vector2(10, 430);
        private Button zoomButton;
        private Vector2 zoomButtonPos = new Vector2(10, 220);
        private Button unZoomButton;
        private Button activeZoomButton;
        private Vector2 lastPos = Vector2.Zero;
        private Vector2 zoomStartPos;
        private Texture2D bikePosTexture;
        private Rectangle bikePosDestination;
        private Rectangle bikePosActiveDestination = new Rectangle(58, 23, 70, 70);
        private Rectangle bikePosUnActiveDestination = new Rectangle(58, 23, 64, 64);
        private bool testing = false;
        private bool isReady = false;

        /// <summary>
        /// Creates a new level editor.
        /// </summary>
        /// <param name="audioPlayer">The AudioPlayer intance 
        ///                           that playes all the sound effects</param>
        /// <param name="content">Used ContentManager</param>
        /// <param name="pSpriteBatch">Used SpriteBatch</param>
        public LevelEditor(AudioPlayer audioPlayer, ContentManager content)
            : base(audioPlayer, content)
        {
            bikePosTexture = content.Load<Texture2D>("Images/bikePos");
            bikePosDestination = bikePosUnActiveDestination;
            bike.SetInitPos(bikePosDestination.X, bikePosDestination.Y);

			CreateUI(content);

			activeZoomButton = unZoomButton;
            TireFail += new Action<Level>(CloseTesting);
			HeadFail += new Action<Level>(CloseTesting);
			Win += new Action<Level>(CloseTesting);
        }

		private void CreateUI(ContentManager content)
		{
			grassButton = new Button("grass", grassButtonPos, content, true);
			grassButton.Width = 100;
			grassButton.Height = 80;
			grassButton.ButtonPressed += new Action<Button>(grassButton_ButtonPressed);

			jumpButton = new Button("jump", jumpButtonPos, content, true);
			jumpButton.Width = 100;
			jumpButton.Height = 80;
			jumpButton.ButtonPressed += new Action<Button>(jumpButton_ButtonPressed);

			nailButton = new Button("nail", nailButtonPos, content, true);
			nailButton.Width = 50;
			nailButton.Height = 80;
			nailButton.ButtonPressed += new Action<Button>(nailButton_ButtonPressed);

			finishButton = new Button("finish", finishButtonPos, content, true);
			finishButton.Width = 100;
			finishButton.Height = 80;
			finishButton.ButtonPressed += new Action<Button>(finishButton_ButtonPressed);

			saveButton = new Button("save", saveButtonPos, content, true);
			saveButton.Width = 100;
			saveButton.Height = 43;
			saveButton.ButtonPressed += new Action<Button>(saveButton_ButtonPressed);

			backButton = new Button("back", backButtonPos, content, true);
			backButton.Width = 70;
			backButton.Height = 43;
			backButton.ButtonPressed += new Action<Button>(backButton_ButtonPressed);

			closeButton = new Button("ok", closeButtonPos, content);
			closeButton.Width = 200;
			closeButton.Height = 100;
			closeButton.ButtonPressed += new Action<Button>(closeButton_ButtonPressed);

			testButton = new Button("test", testButtonPos, content, true);
			testButton.Width = 88;
			testButton.Height = 43;
			testButton.ButtonPressed += new Action<Button>(testButton_ButtonPressed);

			zoomButton = new Button("zoom", zoomButtonPos, content, true);
			zoomButton.ButtonPressed += new Action<Button>(zoomButton_ButtonPressed);

			unZoomButton = new Button("unZoom", zoomButtonPos, content, true);
			unZoomButton.ButtonPressed += new Action<Button>(unZoomButton_ButtonPressed);
		}

		void unZoomButton_ButtonPressed(Button obj)
		{
			zoomStartPos.X = camPos[0];
			zoomStartPos.Y = camPos[1];
			zoom = 0.5f;
			camPos[0] -= zoom * 840;
			camPos[1] -= zoom * 400;
			activeZoomButton = zoomButton;
		}

		void zoomButton_ButtonPressed(Button obj)
		{
			activeZoomButton = unZoomButton;
			zoom = 1.0f;
			camPos[0] = zoomStartPos.X;
			camPos[1] = zoomStartPos.Y;
		}

		void testButton_ButtonPressed(Button obj)
		{
			bike.Reset();
			zoomStartPos.X = camPos[0];
			zoomStartPos.Y = camPos[1];
			testing = true;
            EnableControl();
			audioPlayer.PlayMotor();
		}

		void closeButton_ButtonPressed(Button obj)
		{
			camPos[0] = zoomStartPos.X;
			camPos[1] = zoomStartPos.Y;
			zoom = 1;
			testing = false;
			audioPlayer.StopMotor();
		}

		void backButton_ButtonPressed(Button obj)
		{
			if (Ready != null)
				Ready(false);
		}

		void saveButton_ButtonPressed(Button obj)
		{
			if (Ready != null)
				Ready(true);

			bike.Reset();
			testing = true;
			isReady = true;
		}

		void finishButton_ButtonPressed(Button obj)
		{
			float x = finishButtonPos.X + finishButton.Width * 0.5f + camPos[0];
			float y = finishButtonPos.Y + finishButton.Height * 1.5f + camPos[1];

			Add(LevelComponentType.finish, x, y, 0).SetCamPos(camPos);
		}

		void nailButton_ButtonPressed(Button obj)
		{
			float x = nailButtonPos.X + nailButton.Width * 0.5f + camPos[0];
			float y = nailButtonPos.Y + nailButton.Height * 1.5f + camPos[1];
			
			Add(LevelComponentType.nail, x, y, 0).SetCamPos(camPos);
		}

		void jumpButton_ButtonPressed(Button obj)
		{
			float x = jumpButtonPos.X + camPos[0];
			float y = jumpButtonPos.Y + jumpButton.Height * 1.0f + camPos[1];

			Add(LevelComponentType.jump, x, y, 0).SetCamPos(camPos);
		}

		void grassButton_ButtonPressed(Button obj)
		{
			float x = grassButtonPos.X + grassButton.Width * 0.5f + camPos[0];
			float y = grassButtonPos.Y + grassButton.Height * 1.5f + camPos[1];
			
			Add(LevelComponentType.grass, x, y, 0).SetCamPos(camPos);
		}

        /// <summary>
        /// Updates all the buttons and level components in the level editor.
        /// If a button or a level component reacts somehow to the touch this
        /// function returns immediately and rest of the buttons/level components
        /// doesn't get updated.
        /// </summary>
        public override void Update()
        {
            transform.M11 = zoom;
            transform.M22 = zoom;
            transform.M41 = -camPos[0] * zoom;
            transform.M42 = -camPos[1] * zoom;
            if (testing)
            {
                base.Update();
                if (!testing)
					closeButton_ButtonPressed(null);

                if (!isReady)
                {
                    TouchCollection touchCollection = TouchPanel.GetState();
                    if (touchCollection.Count > 0)
                    {
                        closeButton.Update(touchCollection[0]);
                    }
                }
            }
            else
            {
                TouchCollection touchCollection = TouchPanel.GetState();
                if (touchCollection.Count > 0)
                {
                    if (backButton.Update(touchCollection[0]))
                        return;
                    if (saveButton.Update(touchCollection[0]))
                        return;
                    if (testButton.Update(touchCollection[0]))
                        return;
                    if (activeZoomButton.Update(touchCollection[0]))
                        return;
                    if (zoom == 1)
                    {
                        if (grassButton.Update(touchCollection[0]))
                            return;
                        if (jumpButton.Update(touchCollection[0]))
                            return;
                        if (nailButton.Update(touchCollection[0]))
                            return;
                        if (finishButton.Update(touchCollection[0]))
                            return;
                        for (int i = components.Count - 1; i >= 0; i--)
                            if (components[i].Update(touchCollection[0]))
                            {
                                if (touchCollection.Count > 1)
                                {
                                    float dY = touchCollection[1].Position.Y - 
                                               touchCollection[0].Position.Y;
                                    float dX = touchCollection[1].Position.X - 
                                               touchCollection[0].Position.X;
                                    float angle = (float)Math.Atan2(dY, dX);
                                    float width = (float)Math.Sqrt(dX * dX + dY * dY);
                                    components[i].SetAngle(angle);
                                }
                                if (!bike.RotationData.device)
                                {
                                    KeyboardState keyboardState = Keyboard.GetState();
                                    if (keyboardState.IsKeyDown(Keys.A))
                                        components[i].SetAngle(components[i].Angle - 
                                                               1*Level.DEG_TO_RAD);
                                    if (keyboardState.IsKeyDown(Keys.D))
                                        components[i].SetAngle(components[i].Angle + 
                                                               1*Level.DEG_TO_RAD);
                                }
                                return;
                            }
                        if (touchCollection[0].State == TouchLocationState.Pressed && 
                            bikePosDestination.Contains(new Point((int)(
                                                        touchCollection[0].Position.X + camPos[0]), 
                            (int)(touchCollection[0].Position.Y + camPos[1]))))
                        {
                            bikePosDestination = bikePosActiveDestination;
                        }
                        else if (touchCollection[0].State == TouchLocationState.Released)
                        {
                            bikePosDestination = bikePosUnActiveDestination;
                            bike.SetInitPos(bikePosDestination.X, bikePosDestination.Y);
                        }
                        if (bikePosDestination == bikePosActiveDestination)
                        {
                            int x = (int)(touchCollection[0].Position.X - 
                                          0.5*bikePosTexture.Width + camPos[0]);
                            int y = (int)(touchCollection[0].Position.Y - 
                                          0.5*bikePosTexture.Height + camPos[1]);
                            bikePosDestination.X = x;
                            bikePosDestination.Y = y;
                            bikePosActiveDestination.X = x;
                            bikePosActiveDestination.Y = y;
                            bikePosUnActiveDestination.X = x;
                            bikePosUnActiveDestination.Y = y;
                            return;
                        }
                    }
                    if (lastPos.Equals(Vector2.Zero))
                        lastPos = touchCollection[0].Position;
                    camPos[0] -= touchCollection[0].Position.X - lastPos.X;
                    camPos[1] -= touchCollection[0].Position.Y - lastPos.Y;
                    lastPos = touchCollection[0].Position;
                }
                else
                    lastPos = Vector2.Zero;
            }
        }

        /// <summary>
        /// A callback funtion to stop the testing of the level and return to the modifying view
        /// </summary>
		/// <param name="level">The caller of this function</param>
        public void CloseTesting(Level level)
        {
            if (!isReady) 
			{
                testing = false;
                audioPlayer.StopMotor();
            }
        }

        /// <summary>
        /// Saves the definitions of the created level to a file
        /// </summary>
        /// <param name="name">The name of the level</param>
        /// <returns>Return true if the saving succeeded, false otherwise</returns>
        public bool SaveToFile(String name)
        {
            String fileName = name.Replace(' ', '_') + ".lvl";
            IsolatedStorageFile recordsStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (recordsStorage.FileExists(fileName) || name == "level1" ||
                                          name == "level2" || name == "level3")
                return false;

            using (IsolatedStorageFileStream fileStream = recordsStorage.OpenFile(fileName, System.IO.FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(fileStream))
				{
					writer.WriteLine(string.Format("start:{0}:{1}", bikePosDestination.X, bikePosDestination.Y));

					foreach (var c in components)
					{
						string s = string.Format("{0}:{1}:{2}:{3}", c.Name, c.Pos.X, c.Pos.Y, c.Angle);
						writer.WriteLine(s);
					}
				}
            }
            return true;
        }

        /// <summary>
        /// An empty implementation because the motorcycle is wanted 
        /// to behave differently in level editor than in other levels
        /// </summary>
        public override void StopBikeMotor()
        {
        }

        /// <summary>
        /// An empty implementation because the motorcycle is wanted to 
        /// behave differently in level editor than in other levels
        /// </summary>
        public override void ResetBike()
        {
        }

        /// <summary>
        /// Draws all the buttons and level components in the level editor
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (testing)
            {
				base.Draw(spriteBatch);
                if (!isReady)
					closeButton.Draw(spriteBatch);
            }
            else
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap,
                                  null, null, null, transform);
                for (int i = 0; i < components.Count; i++)
                    components[i].Draw(spriteBatch);
                spriteBatch.Draw(bikePosTexture, bikePosDestination, Color.White); 
                spriteBatch.End();
                spriteBatch.Begin();
				grassButton.Draw(spriteBatch);
				jumpButton.Draw(spriteBatch);
				nailButton.Draw(spriteBatch);
				finishButton.Draw(spriteBatch);
				saveButton.Draw(spriteBatch);
				testButton.Draw(spriteBatch);
				backButton.Draw(spriteBatch);
				activeZoomButton.Draw(spriteBatch);
            }
        }
    }
}
