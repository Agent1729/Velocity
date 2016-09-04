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
		private SpriteFont font;
		public int id;

		public Wall(float x, float y) : base(x, y) { }

		protected override void init()
		{
			objType = "Wall";
			mainSpriteName = "wall_block";
			base_width = 16; base_height = 16;
			width = base_width; height = base_height;
			depth = 5;
			collisionStatic = true;
			isSolid = true;
			takesControls = true;
			canBePushed = false;
			hasGravity = false;
			canAbsorb = false;
			hasBeenFrictioned = false;
		}

		protected override void doLoadTexture()
		{
			mainSprite = SpriteManager.getSprite(mainSpriteName);

			id = level.getNextWallID();

			font = FontManager.getFont("Font1");
		}

		//protected override void dotick() { }

		protected override void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			base.doDraw(spriteBatch, c);
			id = level.getObjNum(this);
			drawText(spriteBatch, c, font, id.ToString(), new Vector2(x-8, y-16), Color.Black);
		}

		protected override void doshiftPressed(object lvl)
		{
			//id = level.getObjNum(this);
		}
	}
}
