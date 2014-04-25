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
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using System.IO;

namespace MotoTrialRacer
{
    /// <summary>
    /// The stucture of the high score records
    /// </summary>
    struct Record
    {
        public string Name;
        public int Time;
    }

    /// <summary>
    /// This class handles all the high scores
    /// </summary>
    class RecordHandler
    {
		public List<Record> Records { get; private set; }
        
		private string fileName;
		private const char dataSeparator = ':';

        /// <summary>
        /// Creates a new record handler
        /// </summary>
        /// <param name="levelName">The name of the level 
        ///                         which records this instance handles</param>
        public RecordHandler(String levelName)
        {
            fileName = levelName + ".scr";
			Records = new List<Record>();
        }

        /// <summary>
        /// Loads the records to memory. 
        /// If there isn't any records yet, this method will create them.
        /// </summary>
        public void LoadRecords()
        {
            Load();

			if (Records.Count == 0)
			{
				CreateRecords();
				SaveRecords();
			}
        }

        /// <summary>
        /// Gets the placement of the player with specific level completion time
        /// </summary>
        /// <param name="time">The time in which the player has completed the level</param>
        /// <returns>The placement of the player</returns>
        public int GetPlacement(int time)
        {
            if (time < Records[0].Time)
                return 1;

            for (int i = 0; i < Records.Count-1; i++)
            {
                if (Records[i].Time < time && time < Records[i + 1].Time)
                    return i+2;
            }
            return -1;
        }

        /// <summary>
        /// Saves the records to IsolatedStorage
        /// </summary>
        public void SaveRecords()
        {
            IsolatedStorageFile recordsStorage = IsolatedStorageFile.GetUserStoreForApplication();

            using (IsolatedStorageFileStream fileStream = recordsStorage.OpenFile(fileName, System.IO.FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(fileStream))
				{
					foreach (Record r in Records)
						writer.WriteLine(r.Name + dataSeparator + r.Time);
				}
            }
        }

        /// <summary>
        /// Sets a new record
        /// </summary>
        /// <param name="placement">The placement of the record</param>
        /// <param name="name">The name of the player</param>
        /// <param name="time">The level completion time of the player</param>
        public void SetRecord(int placement, string name, int time)
        {
            for (int i = Records.Count-1; i > placement-1; i--)
            {
                Records[i] = Records[i - 1];
            }
            Records[placement - 1] = new Record {Name = name, Time = time};
        }

        /// <summary>
        /// Loads the records from IsolatedStorage
        /// </summary>
        private void Load()
        {
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForApplication();

            // open isolated storage, and write the savefile.
            if (savegameStorage.FileExists(fileName))
            {
                using (IsolatedStorageFileStream fs = savegameStorage.OpenFile(fileName, System.IO.FileMode.Open))
				{
					using (StreamReader r = new StreamReader(fs))
					{
						string text = r.ReadToEnd();
						string[] rows = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

						foreach (string row in rows)
						{
							string[] pieces = row.Split(dataSeparator);

							Records.Add(new Record	{
														Name = pieces[0],
														Time = Convert.ToInt32(pieces[1])
													});
						}
					}
                }
            }
        }

        /// <summary>
        /// Creates new records and initializes them with player
        /// name "Racer" and level completion time one minute
        /// </summary>
        private void CreateRecords()
        {
			for (int i = 0; i < 10; i++)
				Records.Add(new Record() { Name = "Racer" + (i + 1), Time = 60000 });
        }
    }
}
