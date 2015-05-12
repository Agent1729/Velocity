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
	public class VelocityZone : VelocityObj
	{
		public float vFactor;
		public float vGravFactor;
		public float scale = 1;
		public int type = 1;
		public bool timeDoesntAffectGrav = false;
		public float paintTime = 0;

		private string spriteName2, spriteName3, spriteName4, spriteName5, spriteName6;
		private Texture2D blueCircleSprite; private Texture2D orangeCircleSprite;
		private Texture2D purpleCircleSprite; private Texture2D lgreenCircleSprite;
		private Texture2D redCircleSprite; private Texture2D greenCircleSprite;

		public VelocityNode node = null;

		public VelocityZone(float x, float y) : base(x, y)
		{
			vFactor = .3f;
			vGravFactor = 1f;
			bb = new BB(x, y, width); // Circular
		}

		protected override void init()
		{
			objType = "VelocityZone";
			spriteName = "BlueCircle";
			spriteName2 = "OrangeCircle";
			spriteName3 = "PurpleCircle";
			spriteName4 = "LGreenCircle";
			spriteName5 = "RedCircle";
			spriteName6 = "GreenCircle";
			base_width = 70; base_height = 70;
			width = base_width; height = base_height;
			depth = 10;
			collisionStatic = false;
			isSolid = false;
			takesControls = true;
			canBePushed = false;
			hasGravity = false;
			canAbsorb = false;
			hasBeenFrictioned = false;
		}

		protected override void doLoadTexture(ContentManager Content)
		{
			blueCircleSprite = Content.Load<Texture2D>(spriteName);
			orangeCircleSprite = Content.Load<Texture2D>(spriteName2);
			purpleCircleSprite = Content.Load<Texture2D>(spriteName3);
			lgreenCircleSprite = Content.Load<Texture2D>(spriteName4);
			redCircleSprite = Content.Load<Texture2D>(spriteName5);
			greenCircleSprite = Content.Load<Texture2D>(spriteName6);
		}

		public void rescale(float s)
		{
			scale = s;
			width = base_width * scale;
			height = base_height * scale;
			WH.X = width; WH.Y = height;
			bb = new BB(x, y, width);
		}

		protected override void dotick()
		{
		}

		protected override void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			if (type == 1)
				drawSprite(spriteBatch, c, blueCircleSprite, drawP, Color.White, scale);
			else if (type == 2)
				drawSprite(spriteBatch, c, orangeCircleSprite, drawP, Color.White, scale);
			else if (type == 3)
				drawSprite(spriteBatch, c, purpleCircleSprite, drawP, Color.White, scale);
			else if (type == 4)
				drawSprite(spriteBatch, c, lgreenCircleSprite, drawP, Color.White, scale);
			else if (type == 5)
				drawSprite(spriteBatch, c, redCircleSprite, drawP, Color.White, scale);
			else if (type == 6)
				drawSprite(spriteBatch, c, greenCircleSprite, drawP, Color.White, scale);

		}

		public void Clear()
		{
			if (node != null)
				node.vz = null;
			node = null;
			x = -9999f;
			y = -9999f;
			paintTime = 0;
			scale = 1;
		}
	}
}
