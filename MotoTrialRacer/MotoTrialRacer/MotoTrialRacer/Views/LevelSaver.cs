/**
 * Copyright (c) 2011 Nokia Corporation and/or its subsidiary(-ies).
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
using MotoTrialRacer.UI;
using Microsoft.Xna.Framework.Graphics;

namespace MotoTrialRacer
{
    /// <summary>
    /// This class defines the view which is used for the saving of the user created custom levels
    /// Derives from View.
    /// </summary>
    class LevelSaver : View
    {
        private TextInput nameInput;
        private Button closeButton;

        /// <summary>
        /// Creates a new level saving view
        /// </summary>
        /// <param name="game">The Game instance that will show this view</param>
        public LevelSaver(Game1 game)
            : base(game)
        {
            nameInput = new TextInput("Enter the name of you level", new Vector2(200, 200), 
                                      game.Content, 400, 50);
            closeButton = new Button("ok", new Vector2(300, 350), game.Content);
			closeButton.ButtonPressed += new Action<Button>(closeButton_ButtonPressed);
        }

		void closeButton_ButtonPressed(Button sender)
		{
			if (game.SaveLevel(nameInput.Text))
				game.OpenMenu();
			else
				nameInput.Text += " allready exists.";
		}

        /// <summary>
        /// Updates the name input and button UI components
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (touchChanged)
            {
                nameInput.Update(touchLocation);
                closeButton.Update(touchLocation);
            }
        }

        /// <summary>
        /// Draws the name input and button UI components
        /// </summary>
		public override void Draw(SpriteBatch spriteBatch)
        {
			base.Draw(spriteBatch);
			nameInput.Draw(spriteBatch);
			closeButton.Draw(spriteBatch);
        }
    }
}
