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
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using MotoTrialRacer.UI;

namespace MotoTrialRacer
{
    /// <summary>
    /// This class defines the view that is shown when the player completes a level.
    /// Derives from View.
    /// </summary>
    class WinView : View
    {
        private Button closeButton;
        private SpriteFont font;
        private TextInput textInput;
        private Vector2 textPos = new Vector2(150, 100);
        private RecordHandler recordHandler;
        private int placement = -1;

        /// <summary>
        /// Creates a new view for telling that the player has completed the level
        /// </summary>
        /// <param name="game">The Game instance that will show this view</param>
        public WinView(Game1 game) : base(game)
        {
            recordHandler = new RecordHandler(game.currentLevelName);
            recordHandler.LoadRecords();
            closeButton = new Button("ok", new Vector2(0, 0), game.Content);
			closeButton.ButtonPressed += new Action<Button>(closeButton_ButtonPressed);

            closeButton.Position = new Vector2(game.getWidth() * 0.5f - 
                                                  closeButton.Width * 0.5f, 
                                               game.getHeight() * 0.75f);
            textInput = new TextInput("Enter your name here", 
                                      new Vector2(game.getWidth() * 0.3f, game.relativeY(200)), 
                                      game.Content, (int)(game.getWidth() * 0.4f),
                                      (int)(game.getHeight() * 0.1f));
            font = game.Content.Load<SpriteFont>("SpriteFont2");
            placement = recordHandler.GetPlacement((int)(game.finishTime.TotalMilliseconds));
        }

		void closeButton_ButtonPressed(Button obj)
		{
			game.OpenMenu();
			if (placement != -1 && textInput.Text != textInput.HintText)
			{
				recordHandler.SetRecord(placement, textInput.Text,
										(int)(game.finishTime.TotalMilliseconds));
				recordHandler.SaveRecords();
			}
		}

        /// <summary>
        /// Updates the button and text input UI components
        /// </summary>
        public override void Update()
        {
            base.Update();
            if (touchChanged)
            {
                closeButton.Update(touchLocation);
                textInput.Update(touchLocation);
            }
        }

        /// <summary>
        /// Draws the congratulations text and the time of the level completion.
        /// Also draws the text input for the players name if his/her placement is in the top ten.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch )
        {
			base.Draw(spriteBatch);
            spriteBatch.DrawString(font, "Your time was " +  String.Format("{0:d2}:{1:d2}:{2:d3}", 
                                   game.finishTime.Minutes ,game.finishTime.Seconds, 
                                   game.finishTime.Milliseconds), textPos, Color.Yellow);
            if (placement != -1)
            {
				textInput.Draw(spriteBatch);
            }
			closeButton.Draw(spriteBatch);
        }
    }
}
