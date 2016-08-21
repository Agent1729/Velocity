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

namespace Velocity
{
	public class RegionVisualizer : obj
	{
		private SpriteFont font;

		public RegionVisualizer(float x, float y) : base(x, y) { }

		protected override void init()
		{
			objType = "RegionVisualizer";
			mainSpriteName = "wall_block";
			base_width = 16; base_height = 16;
			width = base_width; height = base_height;
			depth = -10;
			collisionStatic = true;
			isSolid = false;
			takesControls = false;
			canBePushed = false;
			canAbsorb = false;
			hasBeenFrictioned = false;
		}

		protected override void doLoadTexture()
		{
			blackPixel = SpriteManager.getSprite("BlackPixel");
			font = FontManager.getFont("Font1");
		}

		protected override void dotick() { }

		protected override void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			for (int x = 0; x < level.regionsX; x++)
				for (int y = 0; y < level.regionsY; y++)
				{
					float x1 = x * level.regionSize;
					float y1 = y * level.regionSize;
					float x2 = x1 + level.regionSize;
					float y2 = y1 + level.regionSize;

					int regionNum = y * level.regionsX + x;

					if (false)
					{
						drawLine(spriteBatch, c, blackPixel, new Vector2(x1, y1), new Vector2(x2, y1));
						drawLine(spriteBatch, c, blackPixel, new Vector2(x2, y1), new Vector2(x2, y2));
						drawLine(spriteBatch, c, blackPixel, new Vector2(x2, y2), new Vector2(x1, y2));
						drawLine(spriteBatch, c, blackPixel, new Vector2(x1, y2), new Vector2(x1, y1));

						drawText(spriteBatch, c, font, level.regions[regionNum].objs.Count.ToString(), new Vector2(x1+5, y1), Color.Black);
					}
				}
		}
	}
}
