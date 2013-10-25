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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MotoTrialRacer.UI;

namespace MotoTrialRacer
{
    /// <summary>
    /// This class defines the main menu view
    /// Derives from View.
    /// </summary>
    class Menu : View
    {
        private List<Button> buttons = new List<Button>();
        private Button infoButton;
        private const int spacing = 20;

        /// <summary>
        /// Creates a new main menu view
        /// </summary>
        /// <param name="game">The Game instance that will show this view</param>
		public Menu(Game1 game)
			: base(game)
		{
			Vector2 pos = new Vector2(game.getWidth() * 0.88f, 0);
			infoButton = new Button("info", pos, game.Content);
			infoButton.ButtonPressed += (sender => game.ShowInfo());

			if (game.paused && !game.won)
			{
				Button tmp = new Button("resume", pos, game.Content);
				tmp.ButtonPressed += (sender => game.Resume());
				buttons.Add(tmp);
			}
			
			Button button = new Button("newGame", pos, game.Content);
			button.ButtonPressed += (sender => game.ShowLevelSelector(true));
			buttons.Add(button);

			button = new Button("levelEditor", pos, game.Content);
			button.ButtonPressed += (sender => game.StartLevelEditor());
			buttons.Add(button);

			button = new Button("highScores", pos, game.Content);
			button.ButtonPressed += (sender => game.ShowLevelSelector(false));
			buttons.Add(button);
			
			button = new Button("exit", pos, game.Content);
			button.ButtonPressed += (sender => game.Exit());
			buttons.Add(button);

			int menuHeight = (buttons.Count - 1) * spacing;
            foreach (Button but in buttons) 
            {
			    menuHeight += but.Height;
			}

			pos.Y = game.getHeight() * 0.5f - menuHeight * 0.5f;

			for (int i = 0; i < buttons.Count; i++)
			{
				pos.X = game.getWidth() * 0.5f - buttons[i].Width * 0.5f;
				buttons[i].Position = pos;
				pos.Y += (buttons[i].Height + spacing);
			}
		}

        /// <summary>
        /// Updates all the buttons in this view
        /// </summary>
        public override void Update()
        {
            base.Update();
            infoButton.Update(touchLocation);
            foreach (Button but in buttons)
            {
                but.Update(touchLocation);
            }
        }

        /// <summary>
        /// Draws all the buttons in this view
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
			base.Draw(spriteBatch);
			infoButton.Draw(spriteBatch);
            foreach (Button but in buttons) 
            {
				but.Draw(spriteBatch);
            }
        }
    }
}
