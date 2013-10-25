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
    /// The base class for all the views that are shown on top the game world.
    /// Implements the ButtonListener interface
    /// </summary>
    class View : IDrawable
    {
        protected Game1 game;
        protected TouchLocation touchLocation;
        protected bool touchChanged = false;
        private Texture2D background;
        private Rectangle destination;

        /// <summary>
        /// Creates a new view
        /// </summary>
        /// <param name="pGame">The Game instance that will show this view</param>
        public View(Game1 pGame)
        {
            game = pGame;
            destination = new Rectangle(0, 0, pGame.getWidth(), pGame.getHeight() );
            background = new Texture2D(game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] colors = new Color[background.Width * background.Height];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = new Color(0.0f, 0.0f, 0.0f, 0.4f);
            background.SetData<Color>(colors);
        }

        /// <summary>
        /// Updates the state and location of the touch
        /// </summary>
        public virtual void Update()
        {
            TouchCollection touchCollection = TouchPanel.GetState();

            if (touchCollection.Count() > 0)
            {
                touchLocation = touchCollection[0];
                touchChanged = true;
            }
            else if (touchChanged)
                touchChanged = false;
        }

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(background, destination, Color.White);   
		}
	}
}
