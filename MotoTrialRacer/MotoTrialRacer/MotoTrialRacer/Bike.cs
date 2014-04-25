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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace MotoTrialRacer
{
    /// <summary>
    /// The class for defining the motorcycle and the driver and their behaviour
    /// </summary>
    public class Bike : IDrawable
    {
		public RotationData RotationData { get; private set; }
		public bool OffTheBike { get; private set; }

        private World world;
        private ContentManager content;
        private List<Body> parts = new List<Body>();
        private float[] camPos;
        private Vector2 origin = new Vector2(0, 0);
        private RevoluteJoint motor;
        
        // The bike 
        private Body frontWheel;
        private Body frontFork;
        private Body rearWheel;
        private Body rearFork;
        private Body bikeBody;
        
        // The driver
        private Body humanBody;
        private Body head;
        private Body arm;
        private Body hand;

        private Vector2 frontWheelInitPos = new Vector2(358, 250);
        private Vector2 frontForkInitPos = new Vector2(358, 240);
        private Vector2 rearWheelInitPos = new Vector2(229.0f, 178.0f);
        private Vector2 rearForkInitPos = new Vector2(254.0f, 186.0f);
        private float rearForkInitRot = 10.0f;
        private Vector2 bikeBodyInitPos = new Vector2(250, 80);
        private float bikeBodyInitRot = 27.0f;

        private Vector2 headInitPos = new Vector2(335, 78);
        private float headInitRot = 35;
        private Vector2 humanBodyInitPos = new Vector2(300, 114);
        private Vector2 handInitPos = new Vector2(344, 132);
        private float handInitRot = 45.0f;
        private Vector2 armInitPos = new Vector2(320, 105);
        private float armInitRot = 45.0f;
        private float[] bikeSpeed;

        private Joint handToBike;
        private Joint humanToBike;
        private Joint armToBike;

        /// <summary>
        /// Creates a new motorcycle and a driver into the given Box2D world.
        /// Creates all the parts of the motorcycle and driver and joints them together.
        /// </summary>
        /// <param name="pBikeSpeed">A pointer to the variable that describes the speed of the 
        ///                          motorcycle</param>
        /// <param name="pRotationData">RotationData to provide the information of the rotation
        ///                             of the device</param>
        /// <param name="pWorld">The Box2D world where the bike is created into</param>
        /// <param name="pCamPos">A pointer to the variable that describes the position of the
        ///                       camera</param>
        /// <param name="pContent">The used ContentManager instance</param>
        /// <param name="pSpriteBatch">The used SpriteBatch instance</param>
        public Bike(float []pBikeSpeed, RotationData pRotationData, World pWorld, float[] pCamPos,
                    ContentManager pContent)
        {
			OffTheBike = false;
            camPos = pCamPos;
            world = pWorld;
            content = pContent;
            RotationData = pRotationData;

            bikeSpeed = pBikeSpeed;

            frontWheel = CreateCirclePart("wheel", frontWheelInitPos, 35.0f, 0, 0.1f, 0.9f, 0.2f);
            frontFork = CreateBoxPart("susp_lower_long", frontForkInitPos, 20.53f, 21.33f, 0, 0.8f,
                                      1.0f, 0.2f);
            rearWheel = CreateCirclePart("rearWheel", rearWheelInitPos, 32.0f, 0, 0.4f, 1.0f, 
                                         0.2f);
            rearFork = CreateBoxPart("rearFork", rearForkInitPos, 64.0f, 17.0f, rearForkInitRot, 
                                     0.5f, 1.0f, 0.2f);
            bikeBody = CreateBikeBody(bikeBodyInitPos, bikeBodyInitRot, 0.5f, 1.0f, 0.2f);

            RevoluteJointDef motorDef = new RevoluteJointDef();
            motorDef.Initialize(rearWheel, rearFork, rearWheel.GetWorldCenter());
            motorDef.maxMotorTorque = 2.0f;
            motorDef.enableMotor = true;
            motor = (RevoluteJoint)world.CreateJoint(motorDef);

            RevoluteJointDef rearForkBodyDef = new RevoluteJointDef();
            Vector2 anchor = rearFork.GetWorldCenter();
            anchor.X += (32.0f / Level.FACTOR);
            anchor.Y += (13.5f / Level.FACTOR);
            rearForkBodyDef.Initialize(rearFork, bikeBody, anchor);
            rearForkBodyDef.bodyA = rearFork;
            rearForkBodyDef.bodyB = bikeBody;
            rearForkBodyDef.maxMotorTorque = 300.0f;
            world.CreateJoint(rearForkBodyDef);

            RevoluteJointDef frontWheelJointDef = new RevoluteJointDef();
            frontWheelJointDef.Initialize(frontWheel, frontFork, frontWheel.GetWorldCenter());
            frontWheelJointDef.maxMotorTorque = 300.0f;
            world.CreateJoint(frontWheelJointDef);

            DistanceJointDef frontSuspToBikeDef = new DistanceJointDef();
            frontSuspToBikeDef.Initialize(bikeBody, frontFork, 
                                          frontFork.GetWorldCenter() + new Vector2(0, 0.4f),
                                          frontFork.GetWorldCenter());
            frontSuspToBikeDef.frequencyHz = 4.0f;
            frontSuspToBikeDef.dampingRatio = 0.1f;
            frontSuspToBikeDef.collideConnected = true;
            world.CreateJoint(frontSuspToBikeDef);

            DistanceJointDef rearForkDistanceDef = new DistanceJointDef();
            rearForkDistanceDef.Initialize(bikeBody, rearFork,
                                           rearFork.GetWorldCenter() + new Vector2(0, 0.4f),
                                           rearFork.GetWorldCenter());
            rearForkDistanceDef.frequencyHz = 7.0f;
            rearForkDistanceDef.dampingRatio = 0.1f;
            rearForkDistanceDef.collideConnected = true;
            world.CreateJoint(rearForkDistanceDef);

            PrismaticJointDef fSuspBikePrismaticDef = new PrismaticJointDef();
            fSuspBikePrismaticDef.Initialize(bikeBody, frontFork, bikeBody.GetWorldCenter(),
                                             new Vector2(0, 1));
            fSuspBikePrismaticDef.enableLimit = true;
            fSuspBikePrismaticDef.lowerTranslation = -0.2f;
            fSuspBikePrismaticDef.upperTranslation = 0.2f;
            fSuspBikePrismaticDef.collideConnected = true;
            world.CreateJoint(fSuspBikePrismaticDef);

            humanBody = CreateBoxPart("human", humanBodyInitPos, 17.0f, 64.0f, 0, 0.1f, 1.0f, 
                                      0.2f);
            head = CreateBoxPart("head", headInitPos, 38.4f, 29.9f, headInitRot, 0.1f, 1.0f, 
                                 0.2f);
            hand = CreateBoxPart("hand", handInitPos, 34.13f, 8.53f, handInitRot, 0.1f, 1.0f, 
                                 0.2f);
            arm = CreateBoxPart("arm", armInitPos, 42.67f, 8.53f, armInitRot, 0.1f, 1.0f, 0.2f);

            WeldJointDef headToHumanDef = new WeldJointDef();
            headToHumanDef.Initialize(head, humanBody, head.GetWorldCenter());
            world.CreateJoint(headToHumanDef);

            RevoluteJointDef humanToBikeDef = new RevoluteJointDef();
            anchor = humanBody.GetWorldCenter();
            anchor.Y += (30.0f / Level.FACTOR);
            humanToBikeDef.Initialize(humanBody, bikeBody, anchor);
            humanToBikeDef.maxMotorTorque = 300.0f;
            humanToBike = world.CreateJoint(humanToBikeDef);

            RevoluteJointDef humanToArmDef = new RevoluteJointDef();
            anchor = arm.GetWorldPoint(new Vector2(-21.33f / Level.FACTOR, 4.26f / Level.FACTOR));
            humanToArmDef.Initialize(humanBody, arm, anchor);
            humanToArmDef.maxMotorTorque = 300.0f;
            world.CreateJoint(humanToArmDef);

            RevoluteJointDef armToHandDef = new RevoluteJointDef();
            anchor = arm.GetWorldPoint(new Vector2(21.33f / Level.FACTOR, 4.26f / Level.FACTOR));
            armToHandDef.Initialize(arm, hand, anchor);
            armToHandDef.maxMotorTorque = 300.0f;
            world.CreateJoint(armToHandDef);

            RevoluteJointDef handToBikeDef = new RevoluteJointDef();
            anchor = hand.GetWorldPoint(new Vector2(17.06f / Level.FACTOR, 4.26f / Level.FACTOR));
            handToBikeDef.Initialize(hand, bikeBody, anchor);
            handToBikeDef.maxMotorTorque = 300.0f;
            handToBike = world.CreateJoint(handToBikeDef);

            DistanceJointDef armToBikeDef = new DistanceJointDef();
            armToBikeDef.Initialize(hand, bikeBody, 
                                    hand.GetWorldPoint(new Vector2(-17.00f / Level.FACTOR,
                                                       4.26f / Level.FACTOR)),
                                    bikeBody.GetWorldCenter());
            armToBikeDef.length = 40.0f / Level.FACTOR;
            armToBikeDef.frequencyHz = 10.0f;
            armToBikeDef.dampingRatio = 1.0f;
            armToBikeDef.collideConnected = true;
            armToBike = world.CreateJoint(armToBikeDef);
        }

        /// <summary>
        /// Create a Box2D body and loads the texture for it
        /// </summary>
        /// <param name="name">Body's textures Content name</param>
        /// <param name="pos">Body's position</param>
        /// <param name="angle">Body's rotation angle</param>
        /// <returns></returns>
        private Body CreateBody(String name, Vector2 pos, float angle)
        {
            BodyDef bodyDef = new BodyDef();      
            bodyDef.type = BodyType.Dynamic;
            bodyDef.position = pos;
            bodyDef.position.X /= Level.FACTOR;
            bodyDef.position.Y /= Level.FACTOR;
            bodyDef.angle = angle * Level.DEG_TO_RAD;

            Body body = world.CreateBody(bodyDef);
            Texture2D texture = content.Load<Texture2D>("Images/" + name);
            body.SetUserData(new UserData(name, texture));

            return body;
        }

        /// <summary>
        /// Creates one part of the motorcycle or driver
        /// </summary>
        /// <param name="shape">The shape of the part</param>
        /// <param name="name">The Content name of the texture that belongs to this part</param>
        /// <param name="pos">The position of the part</param>
        /// <param name="angle">The rotation angle of the part</param>
        /// <param name="density">The density of the part</param>
        /// <param name="friction">The friction of the part</param>
        /// <param name="restitution">The restitution of the part</param>
        /// <returns></returns>
        private Body CreatePart(Shape shape, String name, Vector2 pos, float angle, float density,
                                float friction, float restitution)
        {

            Body body = CreateBody(name, pos, angle);

            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = density;
            fixtureDef.friction = friction;
            fixtureDef.restitution = restitution;

            body.CreateFixture(fixtureDef);

            parts.Add(body);
            return body;
        }

        /// <summary>
        /// Creates a circle shaped part of the motocycle
        /// </summary>
        /// <param name="name">The Content name of the texture that belongs to this part</param>
        /// <param name="pos">The position of the part</param>
        /// <param name="radius">The radius of the circle shape</param>
        /// <param name="angle">The rotation angle of the part</param>
        /// <param name="density">The density of the part</param>
        /// <param name="friction">The friction of the part</param>
        /// <param name="restitution">The restitution of the part</param>
        /// <returns></returns>
        private Body CreateCirclePart(String name, Vector2 pos, float radius, float angle,
                                      float density, float friction, float restitution)
        {
            CircleShape shape = new CircleShape();
            shape._radius = radius / Level.FACTOR;
            return CreatePart(shape, name, pos, angle, density, friction, restitution);
        }

        /// <summary>
        /// Creates a rectangle shaped part of the motorcycle or driver
        /// </summary>
        /// <param name="name">The Content name of the texture that belongs to this part</param>
        /// <param name="pos">The position of the part</param>
        /// <param name="width">The width of the rectangle shape</param>
        /// <param name="height">The height of the rectangle shape</param>
        /// <param name="angle">The rotation angle of the part</param>
        /// <param name="density">The density of the part</param>
        /// <param name="friction">The friction of the part</param>
        /// <param name="restitution">The restitution of the part</param>
        /// <returns></returns>
        private Body CreateBoxPart(String name, Vector2 pos, float width, float height, 
                                   float angle, float density, float friction, float restitution)
        {
            PolygonShape shape = new PolygonShape();
            shape.SetAsBox(width * 0.5f / Level.FACTOR, height * 0.5f / Level.FACTOR);

            Body body = CreatePart(shape, name, pos, angle, density, friction, restitution);
            ((UserData)body.GetUserData()).Width = width;
            ((UserData)body.GetUserData()).Height = height;
            return body;
        }

        /// <summary>
        /// Adds a polygon to a body
        /// </summary>
        /// <param name="body">The body where the polygon should be added</param>
        /// <param name="vertices">The vertices of the polygon</param>
        /// <param name="density">The density of the polygon</param>
        /// <param name="friction">The friction of the polygon</param>
        /// <param name="restitution">The restitution of the polygon</param>
        private void AddPolygonToBody(Body body, Vector2[] vertices, float density, float friction,
                                      float restitution)
        {
            PolygonShape shape = new PolygonShape();
            shape.Set(vertices, vertices.Length);

            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.shape = shape;
            fixtureDef.density = density;
            fixtureDef.friction = friction;
            fixtureDef.restitution = restitution;

            body.CreateFixture(fixtureDef);
        }

        /// <summary>
        /// Creates the body of the motorcycle
        /// </summary>
        /// <param name="pos">The position of the motorcycle body</param>
        /// <param name="angle">The rotation angle of the motorcycle body</param>
        /// <param name="density">The density of the motorcycle body</param>
        /// <param name="friction">The friction of the motorcycle body</param>
        /// <param name="restitution">The restitution of the motorcycle body</param>
        /// <returns></returns>
        private Body CreateBikeBody(Vector2 pos, float angle, float density, float friction,
                                    float restitution)
        {
            Body body = CreateBody("bikeBody", pos, angle);

            Vector2[] tailVertices = {new Vector2(0, 22.0f/Level.FACTOR),
                                      new Vector2(32.0f/Level.FACTOR, 26.0f/Level.FACTOR),
                                      new Vector2(32.0f/Level.FACTOR, 32.0f/Level.FACTOR)};
            AddPolygonToBody(body, tailVertices, density, friction, restitution);

            Vector2[] rearVertices = {new Vector2(32.0f/Level.FACTOR, 26.0f/Level.FACTOR),
                                      new Vector2(120.0f*0.64f/Level.FACTOR,
                                                  45.0f*0.64f/Level.FACTOR),
                                      new Vector2(120.0f*0.64f/Level.FACTOR, 
                                                  100.0f*0.64f/Level.FACTOR),
                                      new Vector2(90.0f*0.64f/Level.FACTOR, 
                                                  100.0f*0.64f/Level.FACTOR),
                                      new Vector2(50.0f*0.64f/Level.FACTOR, 
                                                  50.0f*0.64f/Level.FACTOR)};
            AddPolygonToBody(body, rearVertices, density, friction, restitution);

            Vector2[] frontVertices = {new Vector2(120.0f*0.64f/Level.FACTOR, 
                                                   45.0f*0.64f/Level.FACTOR),
                                       new Vector2(180.0f*0.64f/Level.FACTOR, 
                                                   35.0f*0.64f/Level.FACTOR),
                                       new Vector2(180.0f*0.64f/Level.FACTOR, 
                                                   140.0f*0.64f/Level.FACTOR),
                                       new Vector2(120.0f*0.64f/Level.FACTOR, 
                                                   140.0f*0.64f/Level.FACTOR)};
            AddPolygonToBody(body, frontVertices, density, friction, restitution);

            Vector2[] noseVertices = {new Vector2(180.0f*0.64f/Level.FACTOR, 0),
                                      new Vector2(210.0f*0.64f/Level.FACTOR, 0),
                                      new Vector2(230.0f*0.64f/Level.FACTOR, 
                                                  60.0f*0.64f/Level.FACTOR),
                                      new Vector2(180.0f*0.64f/Level.FACTOR, 
                                                  135.0f*0.64f/Level.FACTOR)};
            AddPolygonToBody(body, noseVertices, density, friction, restitution);

            parts.Add(body);
            return body;
        }

        /// <summary>
        /// Sets the initial position for the motorcycle and driver
        /// </summary>
        /// <param name="pX">x coordinate of the new initial position</param>
        /// <param name="pY">y coordinate of the new initial position</param>
        public void SetInitPos(float pX, float pY)
        {
            float x = pX - 250;
            float y = pY - 80;
            frontWheelInitPos = new Vector2(358+x, 250+y);
            frontForkInitPos = new Vector2(358+x, 240+y);
            rearWheelInitPos = new Vector2(229+x, 178+y);
            rearForkInitPos = new Vector2(254+x, 186+y);
            bikeBodyInitPos = new Vector2(250+x, 80+y);

            headInitPos = new Vector2(335+x, 78+y);
            humanBodyInitPos = new Vector2(300+x, 114+y);
            handInitPos = new Vector2(344+x, 132+y);
            armInitPos = new Vector2(320+x, 105+y);
        }

        /// <summary>
        /// Resets the bike and the driver to their initial positions
        /// Resets all angular and linear speeds
        /// Recreates the joints if they were destroyed
        /// </summary>
        public void Reset()
        {            
            foreach (var part in parts)
            {
                part.SetAngularVelocity(0);
				part.SetLinearVelocity(Vector2.Zero);
            }
            
			frontWheel.SetTransform(frontWheelInitPos / Level.FACTOR, 0);
            frontFork.SetTransform(frontForkInitPos / Level.FACTOR, 0);
            rearWheel.SetTransform(rearWheelInitPos / Level.FACTOR, 0);
            rearFork.SetTransform(rearForkInitPos / Level.FACTOR, 
                                  rearForkInitRot * Level.DEG_TO_RAD);
            bikeBody.SetTransform(bikeBodyInitPos / Level.FACTOR, 
                                  bikeBodyInitRot * Level.DEG_TO_RAD);

            head.SetTransform(headInitPos / Level.FACTOR, headInitRot * Level.DEG_TO_RAD);
            humanBody.SetTransform(humanBodyInitPos / Level.FACTOR, 0);
            hand.SetTransform(handInitPos / Level.FACTOR, handInitRot * Level.DEG_TO_RAD);
            arm.SetTransform(armInitPos / Level.FACTOR, armInitRot * Level.DEG_TO_RAD);

            if (OffTheBike)
            {
                RevoluteJointDef handToBikeDef = new RevoluteJointDef();
                Vector2 anchor = hand.GetWorldPoint(new Vector2(17.06f / Level.FACTOR, 
                                                                4.26f / Level.FACTOR));
                handToBikeDef.Initialize(hand, bikeBody, anchor);
                handToBikeDef.maxMotorTorque = 300.0f;
                handToBike = world.CreateJoint(handToBikeDef);

                RevoluteJointDef humanToBikeDef = new RevoluteJointDef();
                anchor = humanBody.GetWorldCenter();
                anchor.Y += (30.0f / Level.FACTOR);
                humanToBikeDef.Initialize(humanBody, bikeBody, anchor);
                humanToBikeDef.maxMotorTorque = 300.0f;
                humanToBike = world.CreateJoint(humanToBikeDef);

                DistanceJointDef armToBikeDef = new DistanceJointDef();
                armToBikeDef.Initialize(hand, bikeBody, 
                                        hand.GetWorldPoint(new Vector2(-17.00f / Level.FACTOR, 
                                                                       4.26f / Level.FACTOR)), 
                                        bikeBody.GetWorldCenter());
                armToBikeDef.length = 40.0f / Level.FACTOR;
                armToBikeDef.frequencyHz = 10.0f;
                armToBikeDef.dampingRatio = 1.0f;
                armToBikeDef.collideConnected = true;
                armToBike = world.CreateJoint(armToBikeDef);

                OffTheBike = false;
            }
            
			if (!bikeBody.IsAwake()) 
				bikeBody.SetAwake(true);
            
			if (RotationData.device)
                motor._enableMotor = true;
        }

        /// <summary>
        /// Destroys the joints between the driver and the motorcycle
        /// </summary>
        public void Release()
        {
            OffTheBike = true;
            world.DestroyJoint(handToBike);
            handToBike = null;
            world.DestroyJoint(humanToBike);
            humanToBike = null;
            world.DestroyJoint(armToBike);
            armToBike = null;
        }

        /// <summary>
        /// Makes the motorcycle to ignore the user inputs
        /// </summary>
        public void disableControls()
        {
            motor._enableMotor = false;
            world.ClearForces();
        }

        /// <summary>
        /// Updates the positions of the parts and camera, the motorcycle speed and user inputs
        /// </summary>
        public void Update()
        {
            Vector2 pos = parts[0].GetPosition();
            Vector2 pos2 = parts[2].GetPosition();
            camPos[0] = Math.Min(pos.X, pos2.X) * Level.FACTOR;
            camPos[1] = Math.Max(pos.Y, pos2.Y) * Level.FACTOR;
            bikeSpeed[0] = parts[0].GetLinearVelocity().Length();
            bikeSpeed[1] = parts[2].GetAngularVelocity();

            if (!RotationData.device)
            {
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    motor._enableMotor = true;
                    RotationData.xRot = -30;
                }

                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    motor._enableMotor = true;
                    RotationData.xRot = 0;
                }
                
				if (keyboardState.IsKeyDown(Keys.Left))
                    leanBackwards();
                
				if (keyboardState.IsKeyDown(Keys.Right))
                    leanForwards();
                
				if (keyboardState.IsKeyUp(Keys.Up) && keyboardState.IsKeyUp(Keys.Down))
                {
                    motor._enableMotor = false;
                    RotationData.xRot = 0;
                }
            }
            motor._motorSpeed = RotationData.xRot;
        }

        /// <summary>
        /// Rotates the motorcycle counterclockwise
        /// </summary>
        public void leanBackwards()
        {
            humanBody.ApplyTorque(-5);
            bikeBody.ApplyTorque(-50);
        }

        /// <summary>
        /// Rotates the motorcycle clockwise
        /// </summary>
        public void leanForwards()
        {
            humanBody.ApplyTorque(5);
            bikeBody.ApplyTorque(50);
        }

        /// <summary>
        /// Stops the rotation of the rear wheel
        /// </summary>
        public void StopMotor()
        {
            motor._motorSpeed = 0;
        }
        
        /// <summary>
        /// Stops the rotation of the rear wheel
        /// </summary>
        public void RearBreak()
        {
            motor._enableMotor = true;
            motor._motorSpeed = 0;
        }

        public void FrontBreak()
        {
        }

        /// <summary>
        /// Draw all the parts of the motorcycle and driver
        /// </summary>
		public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Body body in parts) 
            {
                Vector2 pos = body.GetPosition();
                UserData userData = (UserData)body.GetUserData();
                Texture2D texture = userData.Texture;
                pos *= Level.FACTOR;
                if (userData.Name == "susp_lower_long")
                {
                    origin.X = texture.Width * 0.5f;
                    origin.Y = texture.Height - 10.0f;
                }
                else if (userData.Name == "bikeBody")
                {
                    origin.X = 0;
                    origin.Y = 0;
                }
                else if (userData.Name == "rearFork")
                {
                    origin.X = texture.Width * 0.5f;
                    origin.Y = texture.Height * 0.75f;
                }
                else if (userData.Name == "human")
                {
                    origin.X = texture.Width - userData.Width*2.0f;
                    origin.Y = userData.Height * 0.5f;
                }
                else
                {
                    origin.X = texture.Width * 0.5f;
                    origin.Y = texture.Height * 0.5f;
                }
                spriteBatch.Draw(texture,
                                 pos,
                                 null,
                                 Color.White,
                                 body.GetAngle(),
                                 origin,
                                 1.0f,
                                 SpriteEffects.None,
                                 0.0f);
            }
        }
	}
}
