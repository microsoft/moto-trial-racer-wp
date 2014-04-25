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
using System.IO;
using System.IO.IsolatedStorage;
using System.Globalization;
using MotoTrialRacer.Components;

namespace MotoTrialRacer
{

    /// <summary>
    /// The class for different levels of the game.
    /// Implements the IContactListener class and handles the contacts between Box2D Shapes.
    /// </summary>
    public class Level : IContactListener, IDrawable
    {
        // For converting Box2D meters to screen pixels and vice versa
        public const float FACTOR = 85.33f;
		public const float DEG_TO_RAD = 0.0174533f;
		public const float RAD_TO_DEG = 57.295780f;
		
		public event Action<Level> TireFail;
		public event Action<Level> HeadFail;
		public event Action<Level> Win;

		protected Matrix transform = Matrix.Identity;
		protected RotationData rotationData;
        protected World world;
        protected List<LevelComponent> components = new List<LevelComponent>();
        protected Bike bike;
        protected ContentManager content;
        protected AudioPlayer audioPlayer;
        protected float[] camPos = { 0, 0 };
        protected float zoom = 1;

		private NumberFormatInfo provider = new NumberFormatInfo();
        private bool bikeShouldBeReleased = false;		
		private float[] bikeSpeed = new float[2];

        /// <summary>
        /// Creates a new predefined level
        /// </summary>
        /// <param name="number">The number of the level (1, 2 or 3)</param>
        /// <param name="pAudioPlayer">The AudioPlayer intance that playes all the sound 
        ///                            effects</param>
        /// <param name="pContent">Used ContentManager</param>
        /// <param name="pSpriteBatch">Used SpriteBatch</param>
        public Level(int number, AudioPlayer pAudioPlayer, ContentManager pContent)
        {
            content = pContent;
            audioPlayer = pAudioPlayer;
            world = new World(new Vector2(0.0f, 5.0f), true);
            world.ContactListener = this;
            rotationData = new RotationData();
            bike = new Bike(bikeSpeed, rotationData, world, camPos, content);
            provider.NumberDecimalSeparator = ".";

            try
            {
				using (Stream stream = TitleContainer.OpenStream("Content/Levels/Level_" + number + ".lvl"))
				{
					LoadLevel(stream);
				}
            }
            catch (FileNotFoundException)
            {
            }
        }

        /// <summary>
        /// Creates a new user defined custom level
        /// </summary>
        /// <param name="fileName">The name of the file that defines the level</param>
        /// <param name="pAudioPlayer">The AudioPlayer intance that playes all the sound 
        ///                            effects</param>
        /// <param name="pContent">Used ContentManager</param>
        public Level(String fileName, AudioPlayer pAudioPlayer, ContentManager pContent)
        {
            content = pContent;
            audioPlayer = pAudioPlayer;
            world = new World(new Vector2(0.0f, 5.0f), true);
            world.ContactListener = this;
            rotationData = new RotationData();
            bike = new Bike(bikeSpeed, rotationData, world, camPos, content);
            provider.NumberDecimalSeparator = ".";

            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();

            // open isolated storage, and read the savefile.
            if (savegameStorage.FileExists(fileName))
            {
                using (IsolatedStorageFileStream fs = savegameStorage.OpenFile(fileName, System.IO.FileMode.Open))
				{
					LoadLevel(fs);
                }
            }
        }

        /// <summary>
        /// Enables user control
        /// </summary>
        public void EnableControl()
        {
            rotationData.EnableRotation();
        }

		private void LoadLevel(Stream stream)
		{
			using (StreamReader reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();
					string[] pieces = line.Split(':');

					if (pieces.Length >= 3)
					{
						float x = (float)Convert.ToDouble(pieces[1], provider);
						float y = (float)Convert.ToDouble(pieces[2], provider);
						float angle = 0;
						
						if (pieces.Length > 3)
							angle = (float)Convert.ToDouble(pieces[3], provider);

						switch (pieces[0])
						{
							case "start":
								bike.SetInitPos(x, y);
								break;
							case "grass":
								Add(LevelComponentType.grass, x, y, angle / Level.DEG_TO_RAD);
								break;
							case "jump":
								Add(LevelComponentType.jump, x, y, angle / Level.DEG_TO_RAD);
								break;
							case "nail":
								Add(LevelComponentType.nail, x, y, angle / Level.DEG_TO_RAD);
								break;
							case "finish":
								Add(LevelComponentType.finish, x, y, angle / Level.DEG_TO_RAD);
								break;
						}
					}
				}
			}
		}

        /// <summary>
        /// Create a new empty level with no components in it
        /// </summary>
        /// <param name="pAudioPlayer">The AudioPlayer intance that playes all the sound 
        ///                            effects</param>
        /// <param name="pContent">Used ContentManager</param>
        public Level(AudioPlayer pAudioPlayer, ContentManager pContent)
        {
            content = pContent;
            audioPlayer = pAudioPlayer;
            world = new World(new Vector2(0.0f, 5.0f), true);
            world.ContactListener = this;
            rotationData = new RotationData();
            bike = new Bike(bikeSpeed, rotationData, world, camPos, content);
        }

        /// <summary>
        /// A callback function for Box2D to inform that two shapes are beginning to contact with 
        /// each other. Collects the information of the contacts and that information will be 
        /// handled after the physics simulation iterations are completed.
        /// </summary>
        /// <param name="contact">Box2D Contact</param>
        public virtual void BeginContact(Contact contact)
        {
            if (!bike.OffTheBike)
            {
                UserData data = (UserData)contact.GetFixtureA().GetBody().GetUserData();
                if (data != null)
                {
					if (data.Name == "finish")
					{
						if (Win != null)
							Win(this);
					}
					else if (data.Name == "nail")
					{
						if (TireFail != null)
							TireFail(this);
					}
					else if ( (data.Name == "head" && contact.GetFixtureB().GetBody().GetType() == BodyType.Static) ||
					          (data.Name == null && ((UserData)contact.GetFixtureB().GetBody().GetUserData()).Name == "head") )
					{
						bikeShouldBeReleased = true;

						if (HeadFail != null)
							HeadFail(this);
					}
                }
            }
        }

        /// <summary>
        /// This is one the methods of the IContactListener interface.
        /// No implementation.
        /// </summary>
        /// <param name="contact">Box2D Contact</param>
        public virtual void EndContact(Contact contact)
        {
        }

        /// <summary>
        /// This is one the methods of the IContactListener interface.
        /// No implementation.
        /// </summary>
        /// <param name="contact">Box2D Contact</param>
        /// <param name="oldManifold">Box2D Manifold</param>
        public void PreSolve(Contact contact, ref Manifold oldManifold)
        {
        }

        /// <summary>
        /// This is one the methods of the IContactListener interface.
        /// No implementation.
        /// </summary>
        /// <param name="contact">Box2D Contact</param>
        /// <param name="impulse">Box2D ContactImpulse</param>
        public void PostSolve(Contact contact, ref ContactImpulse impulse)
        {
        }

        /// <summary>
        /// Rotates the motorcycle counterclockwise
        /// </summary>
        public void leanBikeBackwards()
        {
            bike.leanBackwards();
        }

        /// <summary>
        /// Rotates the motorcycle clockwise
        /// </summary>
        public void leanBikeForwards()
        {
            bike.leanForwards();
        }

        /// <summary>
        /// Stops the rotation of the rear wheel
        /// </summary>
        public virtual void StopBikeMotor()
        {
            bike.StopMotor();
        }

        /// <summary>
        /// Stops the rotation of the rear wheel
        /// </summary>
        public virtual void ResetBike()
        {
            bike.Reset();
        }

        /// <summary>
        /// Makes the motorcycle to ignore the user inputs
        /// </summary>
        public virtual void disableBikeControls()
        {
            bike.disableControls();
        }

        /// <summary>
        /// Adds a new component to this level
        /// </summary>
        /// <param name="type">The type of the component</param>
        /// <param name="x">The x position of the component</param>
		/// <param name="y">The y position of the component</param>
        /// <param name="angle">The rotation angle of the component</param>
        /// <returns></returns>
        public LevelComponent Add(LevelComponentType type, float x, float y, float angle)
        {
			Vector2 pos = new Vector2(x, y);
			LevelComponent component = null;
            
			switch (type)
            {
                case LevelComponentType.grass:
                    component = new Ground(world, content, pos, angle, 400, 60);
                    break;
                case LevelComponentType.jump:
                    component = new Jump(world, content, pos, angle, 150, 85);
					break;
                case LevelComponentType.nail:
                    component = new SpikeMat(world, content, pos, angle, 250, 70);
					break;
                case LevelComponentType.finish:
                    component = new Finish(world, content, pos, angle, 100, 200);
					break;
            }

			if (component != null)
				components.Add(component);

			return component;
        }

        /// <summary>
        /// Updates the whole game world and components in it. Tells Box2D to simulate the physics
        /// one step forward. Updates the graphics tranformation matrix correspongin to the speed 
        /// and position of the motorcycle.
        /// </summary>
        public virtual void Update()
        {
            world.Step(0.016666f, 10, 10);
            if (bikeShouldBeReleased)
            {
                bike.Release();
                bikeShouldBeReleased = false;
            }
            world.ClearForces();

            zoom = 0.96f * zoom - bikeSpeed[0] * 0.003333f + 0.056f;
            transform.M11 = zoom;
            transform.M22 = zoom;
            transform.M41 = -camPos[0] * zoom + 300;
            transform.M42 = -camPos[1] * zoom + 350;

            audioPlayer.SetMotorPitch(bikeSpeed[1] * 0.05f - 1);
            bike.Update();
        }

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null,
							  null, transform);

			for (int i = 0; i < components.Count; i++)
				components[i].Draw(spriteBatch);
			
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transform);
			bike.Draw(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin();
		}
	}
}
