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
using Box2D.XNA;
using System.IO;
using System.IO.IsolatedStorage;
using System.Globalization;
using MotoTrialRacer.Components;

namespace MotoTrialRacer
{
	/// <summary>
	/// The class for holding the information of LevelComponent bodies.
	/// </summary>
	class UserData
	{
		public String Name;
		public Texture2D Texture;
		public float Width;
		public float Height;

		public UserData(String pName, Texture2D pTexture)
		{
			Name = pName;
			Texture = pTexture;
		}
	}
}
