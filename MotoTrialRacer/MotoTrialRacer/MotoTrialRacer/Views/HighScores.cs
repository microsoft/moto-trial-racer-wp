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
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Graphics;
using MotoTrialRacer.UI;

namespace MotoTrialRacer
{
    /// <summary>
    /// This class defines the HighScores view.
    /// Derives from View.
    /// </summary>
    class HighScores : View
    {
        private Texture2D topic;
        private Vector2 topicPos;
        private Vector2 namePos;
        private String level;
        private Vector2 listPos = new Vector2(280, 130);
        private Button closeButton;
        private Button backButton;
        private RecordHandler recordHandler;
        private SpriteFont font;
        private String scoresList = "";

        /// <summary>
        /// Creates a new HighScores view
        /// </summary>
        /// <param name="game">The Game instance that will show this view</param>
        /// <param name="levelName">The name of the level which high scores this view will
        ///                         show</param>
        public HighScores(Game1 game, String levelName) : base(game)
        {
            level = levelName.Replace('_', ' ');
            recordHandler = new RecordHandler(level);
            recordHandler.LoadRecords();
            for (int i = 0; i < recordHandler.Records.Count; i++)
            {
                TimeSpan time = TimeSpan.FromMilliseconds(recordHandler.Records[i].Time);

                scoresList += (i + 1) + "  " + recordHandler.Records[i].Name + "  " + 
                              String.Format("{0:d2}:{1:d2}:{2:d3}", 
                                            time.Minutes, time.Seconds, time.Milliseconds) + "\n";
            }
            font = game.Content.Load<SpriteFont>("SpriteFont1");
            topic = game.Content.Load<Texture2D>("Images/highScores");
            topicPos = new Vector2(game.getWidth() * 0.5f - topic.Width * 0.5f, 0);
            namePos = new Vector2(game.getWidth() * 0.5f - font.MeasureString(level).X * 0.5f,
                                  topic.Height);

            closeButton = new Button("ok", new Vector2(game.getWidth() * 0.7f, 
                                           game.getHeight() * 0.75f), 
                                           game.Content);

            closeButton.Position = new Vector2(game.getWidth() * 0.5f - closeButton.Width * 0.5f, 
                                               game.getHeight() * 0.78f);

			closeButton.ButtonPressed += new Action<Button>(closeButton_ButtonPressed);


            backButton = new Button("back", new Vector2(game.relativeX(10), game.relativeY(430)), 
											game.Content,
											true);
            backButton.Width = 70; backButton.Height = 43;
			backButton.ButtonPressed += new Action<Button>(backButton_ButtonPressed);

        }

		void backButton_ButtonPressed(Button sender)
		{
			if (level != "level1" && level != "level2" && level != "level3")
				game.ShowMyLevelsSelector(false);
			else
				game.ShowLevelSelector(false);
		}

		void closeButton_ButtonPressed(Button sender)
		{
			game.OpenMenu();
		}

        /// <summary>
        /// Updates the buttons in this view
        /// </summary>
        public override void Update()
        {
            base.Update();
            closeButton.Update(touchLocation);
            backButton.Update(touchLocation);
        }

        /// <summary>
        /// Draws all the components in this view
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
			base.Draw(spriteBatch);
            spriteBatch.Draw(topic, topicPos, Color.White);
            spriteBatch.DrawString(font, level, namePos, Color.Yellow);
            spriteBatch.DrawString(font, scoresList, listPos, Color.Yellow);
			closeButton.Draw(spriteBatch);
			backButton.Draw(spriteBatch);
        }
    }
}
