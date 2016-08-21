using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Velocity.Objects;
using Velocity.Levels;

namespace Velocity.Levels
{
	public class Level2 : Level
	{
		protected override void doInitialize(VelocityGame game)
		{
			levelNum = 2;

			addObj(game, new Wall(50, 100));
			addObj(game, new Wall(200, 200));
			addObj(game, new Wall(300, 275));
			//addObj(game, new Wall(0, 370));
			//addObj(game, new Wall(-150, 270));
			//addObj(game, new Wall(-175, 200));
			addObj(game, new MovingPlatform(450, 150, true, 3, 150));
			addObj(game, new MovingPlatform(0, 100, false, 3, 364-64));

			addObj(game, new Wall(300, 150));
			//addObj(game, new Box(300, 100));
			VelocityZone v = (VelocityZone)addObj(game, new VelocityZone(200, 350));

			Player p = (Player)addObj(game, new Player(0, 0, 1, v));
			p.setGunsOwned(true);

			drawWallLine(game, 16, 464, 16, true);
			drawWallLine(game, 400, 200, 3, false);
			//addObj(game, new Wall(16 + 16 * 32, 432));

			addObj(game, new EnemyRocketLauncher(450, 30));
			addObj(game, new Wall(482, 30));
			addObj(game, new RegionVisualizer(0, 0));
		}

		protected void drawWallLine(VelocityGame game, float _x, float _y, int n, bool horizontal)
		{
			float x = _x, y = _y;
			for (int i = 0; i < n; i++)
			{
				addObj(game, new Wall(x, y));
				if (horizontal)
					x += 32;
				else
					y += 32;
			}
		}
	}
}
