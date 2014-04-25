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
using Microsoft.Xna.Framework.Input.Touch;
using Box2D.XNA;

namespace MotoTrialRacer.Components
{
    /// <summary>
    /// An enumeration to describe the type of a level component
    /// </summary>
    public enum LevelComponentType
    {
        grass,
        jump,
        nail,
        finish
    }

    /// <summary>
    /// The base class for all level components
    /// </summary>
    public class LevelComponent : IDrawable
    {
        protected Texture2D texture;
        protected Rectangle screenPos;
        protected Color unActiveColor = Color.White;
        protected Color activeColor = new Color(0.5f, 1.0f, 0.5f, 1.0f);
        protected Color color = Color.White;
        protected float x, y, width, height;
        protected Body body;
        protected float[] camPos = new float[2];
        protected bool pressed = false;
        protected Rectangle sourceRect;
        protected Vector2 origin;

		public String Name { get; protected set; }
		public Vector2 Pos { get; set; }

        public float X 
		{
            get
            {
                return x;
            }
            set
            {
                screenPos.X = (int)value;
            }
        }
        
		public float Y
        {
            get
            {
                return y;
            }
            set
            {
                screenPos.Y = (int)value;
            }
        }
        
		public float Width
        {
            get
            {
                return width;
            }
            set
            {
                screenPos.Width = (int)value;
            }
        }
        
		public float Height
        {
            get
            {
                return height;
            }
            set
            {
                screenPos.Height = (int)value;
            }
        }
        
		public float Angle
        {
            get
            {
                return body.GetAngle();
            }
        }

        /// <summary>
        /// Creates a new level component
        /// </summary>
        /// <param name="world">The Box2D world that will hold this component</param>
        /// <param name="content">Used ContentManager</param>
        /// <param name="pSpriteBatch">Used SpriteBatch</param>
        /// <param name="pos">The position of the component</param>
        /// <param name="angle">The angle of the component</param>
        /// <param name="pWidth">The width of the component</param>
        /// <param name="pHeight">The height of the component</param>
        public LevelComponent(World world, ContentManager content, 
                              Vector2 pos, float angle, float pWidth, float pHeight)
        {
			Pos = new Vector2();
            x = pos.X;
            y = pos.Y;
            width = pWidth;
            height = pHeight;
            sourceRect = new Rectangle(0, 0, (int)width, (int)height);
            screenPos = new Rectangle(0, 0, (int)width, (int)height);
            origin = new Vector2(sourceRect.Width * 0.5f, sourceRect.Height * 0.5f);

            BodyDef bodyDef = new BodyDef();
            bodyDef.type = BodyType.Static;
            bodyDef.position = pos;
            bodyDef.position.X /= Level.FACTOR;
            bodyDef.position.Y /= Level.FACTOR;
            bodyDef.angle = angle * Level.DEG_TO_RAD;

            body = world.CreateBody(bodyDef);
        }

        /// <summary>
        /// Sets the position of the camera
        /// </summary>
        /// <param name="pCamPos">The position of the camera</param>
        public void SetCamPos(float[] pCamPos)
        {
            camPos = pCamPos;
        }

        /// <summary>
        /// Updates the level component if it's wanted to react to the user touch inputs
        /// </summary>
        /// <param name="touchLocation">The location of the touch</param>
        /// <returns>Returns true if the component asumes that the touch was dedicated to it,
        ///          false otherwise</returns>
        public virtual bool Update(TouchLocation touchLocation)
        {
            Vector2 touch = new Vector2(touchLocation.Position.X + camPos[0] - Pos.X, 
                                        touchLocation.Position.Y + camPos[1] - Pos.Y);
            touch = Vector2.Transform(touch, Matrix.CreateRotationZ(-body.GetAngle()));
            if (screenPos.Contains(new Point((int)(touch.X + Pos.X + width * 0.5), 
                                             (int)(touch.Y + Pos.Y + height * 0.5))))
            {
                if (touchLocation.State == TouchLocationState.Pressed)
                {
                    color = activeColor;
                    pressed = true;
                }
                else if (touchLocation.State == TouchLocationState.Released)
                {
                    color = unActiveColor;
                    pressed = false;
                    return true;
                }
            }

            if (touchLocation.State == TouchLocationState.Released)
            {
                color = unActiveColor;
                pressed = false;
                return false;
            }

            if (pressed)
            {
                Pos = new Vector2(touchLocation.Position.X + camPos[0], touchLocation.Position.Y + camPos[1]);
                body.SetTransform(Pos / Level.FACTOR, body.GetAngle());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the angle of the component
        /// </summary>
        /// <param name="angle">The angle of the component</param>
        public void SetAngle(float angle)
        {
            body.SetTransform(Pos / Level.FACTOR, angle);
        }

		public void Draw(SpriteBatch spriteBatch)
		{
			screenPos.X = (int)(body.GetPosition().X * Level.FACTOR);
			screenPos.Y = (int)(body.GetPosition().Y * Level.FACTOR);
			spriteBatch.Draw(texture,
							 screenPos,
							 sourceRect,
							 color,
							 body.GetAngle(),
							 origin,
							 SpriteEffects.None,
							 0.0f);
		}
	}
}
