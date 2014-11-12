Moto Trial Racer XNA
====================

Moto Trial Racer XNA is a motorcycle racing game on Windows Phone 7 & XNA Game 
Studio 4.0, demonstrating especially the use of Box2D.XNA, an XNA binding for 
the Box2D physics engine. This application has been ported from a 
corresponding Qt/QML example, and thereby also demonstrates and provides 
guidance on the porting and co-development between Qt and Windows Phone. The 
application's graphics are reused from the QML version.

The game has three predefined levels and a level editor with which the user 
can create as many new levels as he/she likes.

![Screenshot](doc/screenshots/xna_moto.png?raw=true)

This project is hosted in a GitHub:
https://github.com/Microsoft/moto-trial-racer-wp

For more information about the implementation see the wiki page:
https://github.com/Microsoft/moto-trial-racer-wp/wiki

This project is compatible with Windows Phone 7 or later. Developed with
Microsoft Visual Studio 2010 Express for Windows Phone and XNA Game Studio 4.0.
The game has been tested to work on Nokia Lumia 800 and Nokia Lumia 900.


Instructions
--------------------------------------------------------------------------------

The goal is to get the motorcycle and the driver to the chequered flag as fast
as possible. The game ends if any part of the motorcycle or player touches the
spikes, or if the player's head touches any other object. When the player gets
to the chequered flag fast enough, he/she gets his/her name and time on the
high scores list. The game can be paused, restarted, and exited at any time
via the options menu.

The motorcycle is accelerated by tilting the device towards the player and
braked by tilting the device away from the player. The center of mass can be
moved backwards/forwards by pressing the arrow buttons in the top corners of 
the game screen.

**Level Editor:**

New objects can be added by pressing the object buttons in the top of the
screen. After a new object is added, it can be dragged to any position. An
active object can be rotated by using two fingers. When the level is completed,
it can be named and saved. The custom levels can be deleted by pressing and
holding down a level button in the level selecting view for two seconds.

**Building and deploying the application on phone:**

* For building the application see http://msdn.microsoft.com/en-us/library/ff928362.aspx
* For deployment see http://msdn.microsoft.com/en-us/library/gg588378.aspx

**Important files and classes:**

* `Game1.cs`: The main game class derived from Microsoft.Xna.Framework.Game. 
  In this class the game content is loaded, game update requests are received,
  the drawing is handled, the game mechanics like timing, winning, and failing
  are implemented, and all the views are handled.
* `Level.cs`: All handling of the game world and physics is done in this class.
* `Bike.cs`: This class defines all the parts of the motorcycle and the driver,
  all the joints between the parts, and all behaviour of the parts and joints.
* `LevelEditor.cs`: The class that defines the level editor allowing to create 
  own custom levels.
* `RotationData.cs`: This class handles the acceleration sensor data of the
  device
* `World.cs`: The world class manages all physics entities, dynamic simulation,
  and asynchronous queries. The world also contains efficient 
  memory management facilities.

**Accelerometer:**

The application heavily bases on accelerometer sensor. Please follow the link 
below for instruction about using accelerometer in applcations. Accelerometer
can be also tested now on the Windows Phone Emulator.

http://msdn.microsoft.com/en-us/library/ff431810.aspx

   
Related documentation
--------------------------------------------------------------------------------

Getting Started Guide:
http://create.msdn.com/en-us/home/getting_started

Learn About Windows Phone 7 Development:
http://msdn.microsoft.com/fi-fi/ff380145

App Hub, develop for Windows Phone:
http://create.msdn.com

Game Development:
http://create.msdn.com/en-us/education/gamedevelopment


License
-------------------------------------------------------------------------------
You can find license details in Licence.txt file provided with this project
or online at
https://github.com/Microsoft/moto-trial-racer-wp/blob/master/Licence.txt


Version history
--------------------------------------------------------------------------------

* Version 1.2: Code level improvements
* Version 1.1: General improvements on code level
* Version 1.0: The first non-beta version
* Version 0.4: Some bugs related to the level editor are fixed
* Version 0.3: The levels and the corresponding high scores can be saved and loaded
* Version 0.2: The first beta version of the level editor is added
* Version 0.1: The initial version
