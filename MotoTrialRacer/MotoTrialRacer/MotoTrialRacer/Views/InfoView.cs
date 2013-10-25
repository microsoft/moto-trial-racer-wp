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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MotoTrialRacer.UI;

namespace MotoTrialRacer
{
    /// <summary>
    /// A view that shows the information about this application.
    /// Derives from View.
    /// </summary>
    class InfoView : View
    {
        private String infoText;
        private float textHeight;
        private Button closeButton;
        private Vector2 buttonPos = new Vector2(0, 0);
        private SpriteFont font;
        private Vector2 textPos;
        private float lastY = 0;
        private float speed = 0;
        private int minY = -700;
        private int maxY = 0; 
        private bool dragging = false;

        /// <summary>
        /// Creates a new info view
        /// </summary>
        /// <param name="game">The Game instance that will show this view</param>
        public InfoView(Game1 game) : base(game)
        {
            maxY = (int)(game.getHeight() * 0.1f);
            infoText = game.Content.Load<String>(@"infotexts");
            font = game.Content.Load<SpriteFont>("SpriteFont1");
            textHeight = font.MeasureString(infoText).Y;
            textPos = new Vector2(game.getWidth() * 0.25f, game.getHeight() * 0.1f);
            closeButton = new Button("ok", buttonPos, game.Content);
			closeButton.ButtonPressed += new Action<Button>(closeButton_ButtonPressed);

            buttonPos = new Vector2(game.getWidth() * 0.5f - closeButton.Width * 0.5f,
                                    textPos.Y + textHeight);
            closeButton.Position = buttonPos;
        }

		void closeButton_ButtonPressed(Button sender)
		{
			game.OpenMenu();
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
                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    dragging = true;
                    lastY = touchLocation.Position.Y;
                }
                else if (touchLocation.State == TouchLocationState.Released)
                {
                    dragging = false;
                }
                closeButton.Update(touchLocation);
            }
            if (dragging)
            {
                speed = lastY - touchLocation.Position.Y;
                lastY = touchLocation.Position.Y;
            }
            else
            {
                if (textPos.Y < minY)
                    if (speed > 0)
                        speed -= (minY - textPos.Y) * 0.02f;
                    else
                        speed = (textPos.Y - minY) * 0.1f;
                else if (maxY < textPos.Y)
                    if (speed < 0)               
                        speed -= (maxY - textPos.Y) * 0.02f;
                    else
                        speed = (textPos.Y - maxY) * 0.1f;
            }
            textPos.Y -= speed;
            buttonPos.Y = textPos.Y + textHeight;
            closeButton.Y = (int)buttonPos.Y;
            speed *= 0.98f;
        }

        /// <summary>
        /// Draws all the components in this view
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
			base.Draw(spriteBatch);
            spriteBatch.DrawString(font, infoText, textPos, Color.Yellow);
			closeButton.Draw(spriteBatch);
        }
    }
}
