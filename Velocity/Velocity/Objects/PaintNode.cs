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
	public class PaintNode : VelocityNode
	{
		public float paintTime = 60;
		private float xdest, ydest;

		public PaintNode(float _x, float _y,  float _xdest, float _ydest, float _xspeed, float _yspeed)
			: base(_x, _y)
		{
			xspeed = _xspeed;
			yspeed = _yspeed;
			xdest = _xdest;
			ydest = _ydest;
			stuckTo = this;
			vz = null;
		}

		protected override void init()
		{
			objType = "PaintNode";
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

		protected override void doCollision(obj other, bool isPrimary)
		{
			if (stuckTo == this)
			{
				if (other.objType == "Wall")
					StayHere();
				if (other.objType == "MovingPlatform")
					StayHere();
				if (other.objType == "Box")
					StayHere();
			}
			if (other.objType == "VelocityZone")
				vZoneCollide(other, isPrimary);
			if (other.objType == "Player")
				if (stuckTo == null)
					if (plr.nodeShot != null)
						destroy();
		}

		private void StayHere()
		{
			xspeed = 0; yspeed = 0;
			if (plr != null)
				plr.velocityShot = null;
			bb = new BB(x, y, resize);
		}

		public override void unStick()
		{
			if (vz != null)
				vz.paintTime = 0;

			paintedTime = 0;
			base.unStick();
		}

		protected override void dotick()
		{
			if (stuckTo == null)
			{
				float dist = (float)Math.Sqrt(Math.Abs(x - plr.x) * Math.Abs(x - plr.x)
					+ Math.Abs(y - plr.y) * Math.Abs(y - plr.y));

				float ang;
				ang = (float)Math.Atan((plr.y - y) / (plr.x - x));
				if (plr.x < x)
					ang += (float)Math.PI;
				xspeed = (float)Math.Cos(ang) * shotSpeed;
				yspeed = (float)Math.Sin(ang) * shotSpeed;
			}

			if ((stuckTo == null) || (stuckTo == this))
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

				capSpeed(terminalVelocity);

				x += xspeed * factor;
				y += yspeed * factor;

				factorSet = false;
				newFactor = 1;
				newGravFactor = 1;
			}

			if (vz != null)
			{
				vz.x = x;
				vz.y = y;
				vz.paintTime = paintTime;
			}
			if ((Math.Abs(x) > 1000) || (Math.Abs(y) > 1000))
				destroy();

			if (Math.Abs(x - xdest) < Math.Abs(xspeed * factor) && Math.Abs(y - ydest) < Math.Abs(yspeed * factor) && !destroyed)
			{
				x = xdest;
				y = ydest;
				StayHere();
			}
		}

		public void setStats(Player _plr)
		{
			plr = _plr;
		}

		protected override void destroy()
		{
			plr.nodeShot = null;
			base.destroy();
		}
	}
}
