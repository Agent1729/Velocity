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

namespace Velocity.Objects
{
	public class Rocket : VelocityObj
	{
		private float xrspd, yrspd;
		private float spd = 3.5f, turnSpd = .05f;
		private float rocketAng;
		private obj lockedOn;

		public Rocket(float _x, float _y, float _xspd, float _yspd, obj _lockedOn) : base(_x, _y)
		{
			xrspd = _xspd; yrspd = _yspd;
			lockedOn = _lockedOn;
			terminalVelocity = 10;

			rocketAng = calcAng(xrspd, yrspd);
		}

		protected override void init()
		{
			objType = "Rocket";
			spriteName = "StickyNode";
			base_width = 4; base_height = 4;
			width = base_width; height = base_height;
			depth = 5;
			collisionStatic = false;
			isSolid = false;
			takesControls = false;
			canBePushed = false;
			hasGravity = false;
			canAbsorb = false;
			hasBeenFrictioned = false;

			base.init();
		}

		protected override void dotick()
		{
			if (paintedTime <= 0)
			{
				factor = 1;
				gravFactor = 1;
			}
			else
				paintedTime--;

			if (factorSet)
			{
				factor = newFactor;
				gravFactor = newGravFactor;
			}

			if (hasGravity)
			{
				if (!factorAffectsGrav)
					yspeed += gravity * factor * gravFactor;
				else
					yspeed += gravity * gravFactor;
			}

			capSpeed(terminalVelocity);

			//Calculate angle and speed
			getNewAngle(lockedOn);
			getNewSpeed();

			x += (xspeed + xrspd) * factor;
			y += (yspeed + yrspd) * factor;
			//Move(xspeed * factor, yspeed * factor, true);

			factorSet = false;
			newFactor = 1;
			newGravFactor = 1;
		}

		private void getNewAngle(obj o)
		{
			float xd = o.x - x, yd = o.y - y;
			float angP = calcAng(xd, yd);

			float cw = angP - rocketAng;
			if (cw < 0)
				cw += (float)Math.PI * 2;
			if (cw <= Math.PI)	//Turn clockwise
			{
				if (Math.Abs(angP - rocketAng) <= turnSpd * factor)
					rocketAng = angP;
				else
					rocketAng += turnSpd * factor;
			}
			else			//Turn counterclockwise
			{
				if (Math.Abs(rocketAng - angP) <= turnSpd * factor)
					rocketAng = angP;
				else
					rocketAng -= turnSpd * factor;
			}

			while (rocketAng < 0)
				rocketAng += (float)Math.PI * 2;
			while (rocketAng > Math.PI * 2)
				rocketAng -= (float)Math.PI * 2;
		}

		private void getNewSpeed()
		{
			xrspd = (float)Math.Cos(rocketAng) * spd;
			yrspd = (float)Math.Sin(rocketAng) * spd;
		}

		protected override void doCollision(obj other, bool isPrimary)
		{
			if (other.objType == "Wall")
				destroy();
			if (other.objType == "Box")
				destroy();
			if (other.objType == "MovingPlatform")
				destroy();
			if (other.objType == "VelocityZone")
				vZoneCollide(other, isPrimary);
			if (other.objType == "Player")
				playerCollide(other);
		}

		protected override void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			base.doDraw(spriteBatch, c);
			//drawLine(spriteBatch, c, blackPixel, new Vector2(x, y), new Vector2(x + (float)Math.Cos(rocketAng) * spd * 8, y + (float)Math.Sin(rocketAng) * spd * 8));
		}

		private void playerCollide(obj other)
		{
			Player p = (Player)other;
			//p.Die();
			destroy();
		}
	}
}
