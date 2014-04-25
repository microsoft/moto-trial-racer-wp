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
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MotoTrialRacer.UI;

namespace MotoTrialRacer
{
    /// <summary>
    /// This class defines the view which is used for selecting a level.
    /// Derives from View.
    /// </summary>
    class LevelSelector : View
    {
        private List<Button> buttons = new List<Button>();
        private List<Button> preDefinedButtons = new List<Button>();
        private List<Button> myButtons = new List<Button>();
        private String deleteQuestion = "";
        private String deleteCandidate = "";
        private SpriteFont font;
        private Button backButton;
        private Button yesButton;
        private Button noButton;
        private bool newGame = true;
        private bool dragging = false;
        private float lastY = 0;
        private float speed = 0;
        private int minY = 0;
        private int maxY = 0; 
        private float listPos = 0;

        /// <summary>
        /// Creates a new level selecting view
        /// </summary>
        /// <param name="game">The Game instance that will show this view</param>
        /// <param name="pNewGame">true if the level is going to be 
        ///                        selected for the new game, false otherwise</param>
        /// <param name="myLevelsPage">true if the view is wanted to
        ///                            be opened on the custom level page</param>
        public LevelSelector(Game1 game, bool pNewGame, bool myLevelsPage)
            : base(game)
        {
            maxY = (int)(game.getHeight() * 0.1f);
            newGame = pNewGame;
            float screenXMiddle = game.getWidth() / 2;
            int spacing = 90;
            int top = 70;
            Button button = new Button("level1", Vector2.Zero, game.Content);
			button.ButtonPressed += new Action<Button>(tmpButton_ButtonPressed); 

            preDefinedButtons.Add(button);
            button.Position = new Vector2(screenXMiddle - button.Width * 0.5f, 
                                          spacing * (preDefinedButtons.Count-1) + top);
            button = new Button("level2", Vector2.Zero, game.Content);
			button.ButtonPressed += new Action<Button>(tmpButton_ButtonPressed);

            preDefinedButtons.Add(button);
            button.Position = new Vector2(screenXMiddle - button.Width * 0.5f, 
                                          spacing * (preDefinedButtons.Count-1) + top);
            button = new Button("level3", Vector2.Zero, game.Content);
			button.ButtonPressed += new Action<Button>(tmpButton_ButtonPressed); 

            preDefinedButtons.Add(button);
            button.Position = new Vector2(screenXMiddle - button.Width * 0.5f, 
                                          spacing * (preDefinedButtons.Count-1) + top);
            button = new Button("myLevels", Vector2.Zero, 
                                game.Content);
			button.ButtonPressed += new Action<Button>(button_ButtonPressed);

            preDefinedButtons.Add(button);
            button.Position = new Vector2(screenXMiddle - button.Width * 0.5f, 
                                          spacing * (preDefinedButtons.Count-1) + top);

            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
            String[] fileNames = savegameStorage.GetFileNames();
            for (int i = 0; i < fileNames.Length; i++)
            {
                String[] pieces = fileNames[i].Split('.');
                if (pieces.Length > 1 && pieces[1] == "lvl")
                {
                    var tmpButton = new Button(pieces[0].Replace('_', ' '), Vector2.Zero, 
                                        game.Content, false);
					tmpButton.ButtonPressed += new Action<Button>(tmpButton_ButtonPressed);
					tmpButton.ButtonLongPressed += new Action<Button>(tmpButton_ButtonLongPressed);
					myButtons.Add(tmpButton);
					tmpButton.Position = new Vector2(screenXMiddle - tmpButton.Width * 0.5f, 
                                                  60 * (myButtons.Count));
                }
            }
            minY = -myButtons.Count * 60 + (int)(0.5f * game.getHeight());
            yesButton = new Button("Yes", new Vector2(game.relativeX(290), game.relativeY(280)), game.Content, false);
			yesButton.ButtonPressed += new Action<Button>(yesButton_ButtonPressed);

            noButton = new Button("No", new Vector2(game.relativeX(490), game.relativeY(280)), game.Content, false);
			noButton.ButtonPressed += new Action<Button>(noButton_ButtonPressed);

            backButton = new Button("back", new Vector2(game.relativeX(10), game.relativeY(430)), game.Content, true);
			backButton.ButtonPressed += new Action<Button>(backButton_ButtonPressed);
            backButton.Width = 70; 
			backButton.Height = 43;
            
			if (myLevelsPage)
                buttons = myButtons;
            else
                buttons = preDefinedButtons;

            font = game.Content.Load<SpriteFont>("SpriteFont1");
        }

		void tmpButton_ButtonLongPressed(Button sender)
		{
			deleteCandidate = sender.Name.Replace(' ', '_');
			if (buttons == myButtons)
				deleteQuestion = "Do you want to delete " + sender.Name + "?";	
		}

		void tmpButton_ButtonPressed(Button sender)
		{
			if (newGame)
			{
				if (buttons == preDefinedButtons)
					game.NewGame(Convert.ToInt32(sender.Name.Substring(sender.Name.Length - 1)));
				else if (Math.Abs(speed) < 1)
					game.NewGame(sender.Name);
			}
			else
			{
				if (Math.Abs(speed) < 1)
					game.ShowHighScores(sender.Name);
			}
		}

		void button_ButtonPressed(Button sender)
		{
			buttons = myButtons;
		}

		void yesButton_ButtonPressed(Button sender)
		{
			DeleteLevel(sender.Name);
		}

		void noButton_ButtonPressed(Button sender)
		{
			deleteCandidate = "";
			deleteQuestion = "";
		}

		void backButton_ButtonPressed(Button sender)
		{
			if (buttons == preDefinedButtons)
				game.OpenMenu();
			else
			{
				speed = 0;
				buttons = preDefinedButtons;
			}
		}

        /// <summary>
        /// Deletes a custom level and all its high scores
        /// </summary>
        /// <param name="name">The name of the level</param>
        private void DeleteLevel(String name)
        {
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();
            savegameStorage.DeleteFile(deleteCandidate + ".lvl");
            savegameStorage.DeleteFile(deleteCandidate + ".scr");
            game.ShowMyLevelsSelector(false);
        }

		/// <summary>
        /// Updates the position of the info text corresponding to the touch inputs of the user.
        /// This implements the "flickable" feature. Also updates the buttons in this view.
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (touchChanged)
            {
                if (deleteQuestion != "")
                {
                    yesButton.Update(touchLocation);
                    noButton.Update(touchLocation);
                }
                else
                {
                    if (buttons == myButtons)
                    {
                        if (touchLocation.State == TouchLocationState.Pressed)
                        {
                            dragging = true;
                            lastY = touchLocation.Position.Y;
                        }
                        else if (touchLocation.State == TouchLocationState.Released)
                        {
                            dragging = false;
                        }
                    }
                    List<Button> curButs = buttons;
                    for (int i = 0; i < curButs.Count; i++)
                    {
                        curButs[i].Update(touchLocation);
                    }
                    backButton.Update(touchLocation);
                }
            }
            if (buttons == myButtons)
            {
                if (dragging)
                {
                    speed = lastY - touchLocation.Position.Y;
                    lastY = touchLocation.Position.Y;
                }
                else
                {
                    if (listPos < minY)
                        if (speed > 0)
                            speed -= (minY - listPos) * 0.02f;
                        else
                            speed = (listPos - minY) * 0.1f;
                    else if (maxY < listPos)
                        if (speed < 0)
                            speed -= (maxY - listPos) * 0.02f;
                        else
                            speed = (listPos - maxY) * 0.1f;
                }
                listPos -= speed;
                for (int i = 0; i < buttons.Count; i++)
                    buttons[i].Y = (int)(listPos + 60 * (i + 1));
                speed *= 0.98f;
            }
        }

        /// <summary>
        /// Draws all the buttons in this view
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
			base.Draw(spriteBatch);
            if (deleteQuestion == "")
            {
                for (int i = 0; i < buttons.Count; i++)
					buttons[i].Draw(spriteBatch);
				backButton.Draw(spriteBatch);
            }
            else
            {
                spriteBatch.DrawString(font, deleteQuestion, new Vector2(240, 200), Color.Yellow);
				yesButton.Draw(spriteBatch);
				noButton.Draw(spriteBatch);
            }
        }
    }
}
