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
	public class Level1 : Level
	{
		protected override void doInitialize(VelocityGame game)
		{
			levelNum = 1;
			VelocityZone v = (VelocityZone)addObj(game, new VelocityZone(-1000, -1000));
			Player p1 = (Player)addObj(game, new Player(0, 0, 1, v));
			p1.setGunsOwned(false);

			addObj(game, new Wall(100, 100));
			//obj o1 = new obj(0, 0, 1, 1);
			//objs.Add(o1);
		}
	}
}
