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
	public class StickyNode : VelocityNode
	{
		private float xoff, yoff;

		public StickyNode(float _x, float _y, float _xspeed, float _yspeed) : base(_x, _y)
		{
			xspeed = _xspeed;
			yspeed = _yspeed;
			stuckTo = this;
			xoff = 0; yoff = 0;
			vz = null;
		}

		protected override void init()
		{
			objType = "StickyNode";
			spriteName = "StickyNode";
			base_width = 4; base_height = 4;
			width = base_width; height = base_height;
			depth = 6;
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
					StickTo(other);
				if (other.objType == "MovingPlatform")
					StickTo(other);
				if (other.objType == "Box")
					StickTo(other);
			}
			if (other.objType == "VelocityZone")
				vZoneCollide(other, isPrimary);
			if (other.objType == "Player")
				if (stuckTo == null)
					if(plr.nodeShot!=null)
						destroy();
		}

		private void StickTo(obj o)
		{
			xspeed = 0; yspeed = 0;

			findStickCoords(o);

			xoff = o.x - x; yoff = o.y - y;
			stuckTo = o;
			if (plr != null)
				plr.velocityShot = null;
			bb = new BB(x, y, resize);
		}

		private void findStickCoords(obj o)
		{
			if (Math.Abs(o.bb.l - x) < base_width)
				x = o.bb.l;
			else if (Math.Abs(o.bb.r - x) < base_width)
				x = o.bb.r;

			if (Math.Abs(o.bb.u - y) < base_height)
				y = o.bb.u;
			else if (Math.Abs(o.bb.d - y) < base_height)
				y = o.bb.d;
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
			else
			{
				x = stuckTo.x - xoff;
				y = stuckTo.y - yoff;
			}

			if (vz != null)
			{
				vz.x = x;
				vz.y = y;
			}
			if ((Math.Abs(x) > 9999) || (Math.Abs(y) > 9999))
				destroy();
		}

		protected override void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			if (stuckTo != null)
			{
				x = stuckTo.x - xoff;
				y = stuckTo.y - yoff;
			}
			drawSprite(spriteBatch, c, sprite, drawP);
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