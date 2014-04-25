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
using Box2D.XNA;

namespace MotoTrialRacer.Components
{
    /// <summary>
    /// The class that defines a level component for the finish line.
    /// A level is completed when the player contacts with this component.
    /// Derives from LevelComponent.
    /// </summary>
    class Finish : LevelComponent
    {
        /// <summary>
        /// Creates a new Finish component
        /// </summary>
        /// <param name="world">The Box2D world that will hold this component</param>
        /// <param name="content">Used ContentManager</param>
        /// <param name="pos">The position of the component</param>
        /// <param name="angle">The rotation angle of the component</param>
        /// <param name="width">The width of the component</param>
        /// <param name="height">The height of the component</param>
        public Finish(World world, ContentManager content, Vector2 pos,
                      float angle, float width, float height) :
            base(world, content, pos, angle, width, height)
        {
            Name = "finish";
            texture = content.Load<Texture2D>("Images/" + Name);
            PolygonShape shape = new PolygonShape();
            shape.SetAsBox(width * 0.5f / Level.FACTOR, height * 0.5f / Level.FACTOR);
            body.CreateFixture(shape, 1.0f).SetFriction(1.0f);
            body.SetUserData(new UserData(Name, null));
            origin = new Vector2(screenPos.Width * 0.5f, texture.Height * 0.5f);
            sourceRect = new Rectangle(0, 0, (int)width, texture.Height);
        }
    }
}
