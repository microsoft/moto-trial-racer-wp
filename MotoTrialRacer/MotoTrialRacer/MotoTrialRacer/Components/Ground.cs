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
    /// This class defines the level component for the ground.
    /// Derives from LevelComponent.
    /// </summary>
    class Ground : LevelComponent
    {
        /// <summary>
        /// Creates a new ground component
        /// </summary>
        /// <param name="world">The Box2D world that will hold this component</param>
        /// <param name="content">Used ContentManager</param>
        /// <param name="pos">The position of this component</param>
        /// <param name="angle">The rotation angle of this component</param>
        /// <param name="width">The width of this component</param>
        /// <param name="height">The height of this component</param>
        public Ground(World world, ContentManager content, Vector2 pos,
                      float angle, float width, float height) :
            base(world, content, pos, angle, width, height)
        {
            Name = "grass";
            texture = content.Load<Texture2D>("Images/" + Name);
            PolygonShape shape = new PolygonShape();
            shape.SetAsBox(width * 0.5f / Level.FACTOR, height * 0.5f / Level.FACTOR);
            body.CreateFixture(shape, 1.0f).SetFriction(1.0f);
            origin = new Vector2(screenPos.Width * 0.5f, texture.Height * 0.5f);
            sourceRect = new Rectangle(0, 0, (int)width, texture.Height);
        }
    }
}
