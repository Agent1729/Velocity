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

namespace Velocity.Objects
{
	public class Wall : VelocityObj
	{
		public Wall(float x, float y) : base(x, y) { }

		protected override void init()
		{
			objType = "Wall";
			spriteName = "wall_block";
			base_width = 16; base_height = 16;
			width = base_width; height = base_height;
			depth = 5;
			collisionStatic = true;
			isSolid = true;
			takesControls = false;
			canBePushed = false;
			hasGravity = false;
			canAbsorb = false;
			hasBeenFrictioned = false;
		}

		//protected override void dotick() { }
	}
}
