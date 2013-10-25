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
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input.Touch;

namespace MotoTrialRacer.UI
{
    /// <summary>
    /// This class defines the text input UI component
    /// </summary>
    class TextInput	: IDrawable
    {
		public String Text { get; set; }
		public String HintText { get; private set; }

        private SpriteFont font;
        private Color hintColor = new Color(1.0f, 1.0f, 0.0f, 0.0f);
        private Vector2 textPos;
        private Texture2D texture;
        private Rectangle destination;
        private int width;
        private int height;

        /// <summary>
        /// Creates a new text input UI component
        /// </summary>
        /// <param name="hint">The hint text that is shown when the text input is empty</param>
        /// <param name="pos">The position of the component</param>
        /// <param name="contentManager">Used ContentManager</param>
        /// <param name="spriteBatch">Used SpriteBatch</param>
        /// <param name="pWidth">The width of the component</param>
        /// <param name="pHeight">The height of the component</param>
        public TextInput(String hint, Vector2 pos, ContentManager contentManager, int pWidth, int pHeight)
        {
            HintText = hint;
            Text = HintText;
            width = pWidth;
            height = pHeight;
            texture = new Texture2D(Game1.Graphics.GraphicsDevice, width, height);
            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++)
            {
                if (i < 2 * width || colors.Length - 2 * width < i ||
                    i % width < 2 || width - 3 < i % width)
                    colors[i] = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                else
                    colors[i] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
            destination = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            texture.SetData<Color>(colors);
            font = contentManager.Load<SpriteFont>("SpriteFont1");
            textPos = new Vector2(pos.X + width*0.1f, 
                                  pos.Y + height*0.5f - font.MeasureString(HintText).Y*0.5f);
        }

        /// <summary>
        /// Checks if the text input is touched and if it is, then opens the virtual keyboard
        /// </summary>
        /// <param name="touchLocation">The touch location</param>
        public void Update(TouchLocation touchLocation)
        {
            if (destination.Contains(new Point((int)touchLocation.Position.X, 
                                               (int)touchLocation.Position.Y)))
            {
                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    if (!Guide.IsVisible)
                    {
                        if (Text == HintText)
                        {
                            Guide.BeginShowKeyboardInput(PlayerIndex.One, "Moto Trial Racer",
                                                         HintText, "", 
                                                         delegate(IAsyncResult result)
                            {
                                String tempText = Guide.EndShowKeyboardInput(result);
                                if (tempText != null)
                                {
                                    if (tempText == "")
                                        Text = HintText;
                                    else
                                        Text = tempText;
                                }
                            }, null);
                        }
                        else
                        {
                            Guide.BeginShowKeyboardInput(PlayerIndex.One, "Moto Trial Racer", 
                                                         HintText, Text, 
                                                         delegate(IAsyncResult result)
                            {
                                String tempText = Guide.EndShowKeyboardInput(result);
                                if (tempText != null)
                                {
                                    if (tempText == "")
                                        Text = HintText;
                                    else
                                        Text = tempText;
                                }
                            }, null);
                        }
                    }
                }
            }
        }

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, destination, Color.White);
			spriteBatch.DrawString(font, Text, textPos, hintColor);
		}
	}
}
