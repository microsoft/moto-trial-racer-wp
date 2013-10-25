/**
 * Copyright (c) 2011 Nokia Corporation and/or its subsidiary(-ies).
 * All rights reserved.
 *
 * For the applicable distribution terms see the license text file included in
 * the distribution.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace MotoTrialRacer.UI
{
    /// <summary>
    /// The class that defines a button UI component
    /// </summary>
    class Button : IDrawable
    {
		public event Action<Button> ButtonPressed;
		public event Action<Button> ButtonLongPressed;

		public String Name { get; private set; }

        private int width;
        private int height;
        private Rectangle destination;
        private Rectangle pressedDestination;
        private Rectangle currentDestination;
        private Texture2D currentTexture;
        private Texture2D texture;
        private Texture2D pressedTexture;
        private Vector2 position;
        private SpriteFont font;
        private int pressedTime = 0;

        /// <summary>
        /// Creates a new button that has an image in Content for up and pressed states
        /// If the images name is for example "image.png" the pressed state image has to be
        /// "image_selected.png"
        /// </summary>
        /// <param name="pName">The name of the image in Content</param>
        /// <param name="pListener">The listener for this button</param>
        /// <param name="pos">The position of this button</param>
        /// <param name="contentManager">Used ContentManager</param>
        /// <param name="pSpriteBatch">Used SpriteBatch</param>
        public Button(String pName, Vector2 pos,
                      ContentManager contentManager)
        {
            position = pos;
            Name = pName;
            texture = contentManager.Load<Texture2D>("Images/" + Name);
            currentTexture = texture;
            pressedTexture = contentManager.Load<Texture2D>("Images/" + Name + "_select");
            width = texture.Width;
            height = texture.Height;
            destination = new Rectangle((int)position.X, (int)position.Y, width, height);
            currentDestination = destination;
            pressedDestination = new Rectangle((int)(position.X + 0.05f * width),
                                               (int)(position.Y + 0.05f * height), 
                                               (int)(width * 0.9f), (int)(height * 0.9f));
        }

        /// <summary>
        /// Creates a new button that has an image in Content for up state only of doesn't an
        /// image at all
        /// </summary>
        /// <param name="pName">The name of the image in Content if there is one. Otherwise the
        ///                     text on the button</param>
        /// <param name="pListener">The listener for this button</param>
        /// <param name="pos">The position of this button</param>
        /// <param name="contentManager">Used ContentManager</param>
        /// <param name="pSpriteBatch">Used SpriteBatch</param>
        /// <param name="isTexture">To tell if there is an image for the button or not. True if
        ///                         there is, false otherwise</param>
        public Button(String pName, Vector2 pos,
                      ContentManager contentManager, bool isTexture)
        {
            position = pos;
            Name = pName;
            if (isTexture)
            {
                texture = contentManager.Load<Texture2D>("Images/" + Name);
                currentTexture = texture;
                pressedTexture = null;
                width = texture.Width;
                height = texture.Height;
                destination = new Rectangle((int)position.X, (int)position.Y, width, height);
                currentDestination = destination;
                pressedDestination = new Rectangle((int)(position.X + 0.05f * width),
                                                   (int)(position.Y + 0.05f * height), 
                                                   (int)(width * 0.9f), (int)(height * 0.9f));
            }
            else
            {
                font = contentManager.Load<SpriteFont>("SpriteFont2");
                Vector2 dims = font.MeasureString(Name);
                width = (int)(dims.X*1.1f);
                height = (int)(dims.Y*1.05f);

				RenderTarget2D renderTarget = new RenderTarget2D(Game1.Graphics.GraphicsDevice, width, height);
				SpriteBatch spriteBatch = new SpriteBatch(Game1.Graphics.GraphicsDevice);
                spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);

                Texture2D bgTexture = new Texture2D(spriteBatch.GraphicsDevice, width, height);
                Color[] colors = new Color[width * height];
                for (int i = 0; i < colors.Length; i++)
                {
                    if (i < 2 * width || colors.Length - 2 * width < i ||
                        i % width < 2 || width - 3 < i % width)
                        colors[i] = Color.Yellow;
                    else
                        colors[i] = Color.Transparent;
                }
                destination = new Rectangle((int)pos.X, (int)pos.Y, width, height);
                bgTexture.SetData<Color>(colors);
                spriteBatch.Begin();
                spriteBatch.Draw(bgTexture, Vector2.Zero, Color.White);
                spriteBatch.DrawString(font, Name, new Vector2(width*0.5f - dims.X*0.5f, 
                                                               height*0.5f - dims.Y*0.5f),
                                       Color.Yellow);
                spriteBatch.End();

                spriteBatch.GraphicsDevice.SetRenderTarget(null);
                texture = (Texture2D)renderTarget;

                texture.GetData<Color>(colors);

                for (int i = 0; i < colors.Length; i++)
                {
                    if (!colors[i].Equals(Color.Yellow))
                      colors[i] = Color.Transparent;
                }

                texture.SetData<Color>(colors);

                currentTexture = texture;
                pressedTexture = null;
                destination = new Rectangle((int)position.X, (int)position.Y, width, height);
                currentDestination = destination;
                pressedDestination = new Rectangle((int)(position.X + 0.05f * width), 
                                                   (int)(position.Y + 0.05f * height), 
                                                   (int)(width * 0.9f), (int)(height * 0.9f));
            }
        }

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                destination = new Rectangle((int)position.X, (int)position.Y, width, height);
                currentDestination = destination;
                pressedDestination = new Rectangle((int)(position.X + 0.05f * width), 
                                                   (int)(position.Y + 0.05f * height), 
                                                   (int)(width * 0.9f), (int)(height * 0.9f));
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                destination = new Rectangle((int)position.X, (int)position.Y, width, height);
                currentDestination = destination;
                pressedDestination = new Rectangle((int)(position.X + 0.05f * width), 
                                                   (int)(position.Y + 0.05f * height), 
                                                   (int)(width * 0.9f), (int)(height * 0.9f));
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                bool pressed = (currentDestination == pressedDestination);
                position = value;
                destination = new Rectangle((int)position.X, (int)position.Y, width, height);
                pressedDestination = new Rectangle((int)(position.X + 0.05f * width), 
                                                   (int)(position.Y + 0.05f * height), 
                                                   (int)(width * 0.9f), (int)(height * 0.9f));
                if (pressed)
                    currentDestination = pressedDestination;
                else 
                    currentDestination = destination;
            }
        }

        public int X
        {
            get
            {
                return destination.X;
            }
            set
            {
                bool pressed = (currentDestination == pressedDestination);
                position.X = value;
                destination.X = value;
                pressedDestination.X = (int)(position.X + 0.05f * width);
                if (pressed)
                    currentDestination = pressedDestination;
                else 
                    currentDestination = destination;
            }
        }

        public int Y
        {
            get
            {
                return destination.Y;
            }
            set
            {
                bool pressed = (currentDestination == pressedDestination);
                position.Y = value;
                destination.Y = value;
                pressedDestination.Y = (int)(position.Y + 0.05f * height);
                if (pressed)
                    currentDestination = pressedDestination;
                else
                    currentDestination = destination;
            }
        }

        /// <summary>
        /// Updates the button state.
        /// </summary>
        /// <param name="touchLocation">The location of the users touch</param>
        /// <returns>Returns true if the button asumes that the touch was dedicated to it, 
        ///          false otherwise</returns>
        public bool Update(TouchLocation touchLocation)
        {
            if (currentDestination == pressedDestination)
            {
                pressedTime++;
                if (pressedTime > 120)
                {
					if (ButtonLongPressed != null)
						ButtonLongPressed(this);
                    
					currentDestination = destination;
                    currentTexture = texture;
                    pressedTime = 0;
                    return true;
                }
            }
            if (destination.Contains(new Point((int)touchLocation.Position.X, 
                                               (int)touchLocation.Position.Y)))
            {
                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    if (pressedTexture != null)
                        currentTexture = pressedTexture;
                    currentDestination = pressedDestination;
                    return true;
                }
                if (touchLocation.State == TouchLocationState.Released && 
                    currentDestination == pressedDestination)
                {
                    currentDestination = destination;
                    currentTexture = texture;
                    
					if (ButtonPressed != null)
						ButtonPressed(this);

                    pressedTime = 0;
                    return true;
                }
                return false;
            }
            if (touchLocation.State == TouchLocationState.Released)
            {
                currentDestination = destination;
                currentTexture = texture;
                pressedTime = 0;
                return false;
            }
            return false;
        }

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(currentTexture, currentDestination, Color.White); 
		}
	}
}
