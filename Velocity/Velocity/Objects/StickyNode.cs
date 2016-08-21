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
		private float px, py;

		//stuckTo == null	//Not stuck to anything, flying back to player?
		//stuckTo == this	//Not stuck to anything, flying to dest?
		//stuckTo == obj	//Stuck to obj

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
			mainSpriteName = "StickyNode";
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
			{
				if(other!=vz)
					vZoneCollide(other, isPrimary);
			}
			if (other.objType == "Player")
				if (stuckTo == null)
					if(plr.nodeShot!=null)
						destroy();
		}

		private void StickTo(obj o)
		{
			findStickCoords(o);

			xspeed = 0; yspeed = 0;

			xoff = o.x - x; yoff = o.y - y;
			stuckTo = o;
			if (plr != null)
				plr.velocityShot = null;
			bb = new BB(x, y, resize);
		}

		private void findStickCoords(obj o)
		{
			bool somethingSet = false;
			Vector2 col;
			float px1 = px;
			float py1 = py;
			if (x - px > 0) px1 -= width;
			else px1 += width;
			if (y - py > 0) py1 -= height;
			else py1 += height;

			x = px;
			y = py;
			setRegions();
			int tries = 0;
			while(!(level.collisionList(this, true).Contains(o)))
			{
				x += xspeed * .1f;
				y += yspeed * .1f;
				setRegions();
				tries++;
				if(tries==1000)
				{
					x = px;
					y = py;
					break;
				}
			}
			somethingSet = true;

			if(!somethingSet)
			if ((col = BB.lineCollisionPoint(px1, py1, x, y, o.bb.l, o.bb.u, o.bb.l, o.bb.d)).X != -1)		//Left
			{
				x = col.X;
				y = col.Y;
				somethingSet = true;
			}
			else if ((col = BB.lineCollisionPoint(px1, py1, x, y, o.bb.r, o.bb.u, o.bb.r, o.bb.d)).X != -1)	//Right
			{
				x = col.X;
				y = col.Y;
				somethingSet = true;
			}
			else if ((col = BB.lineCollisionPoint(px1, py1, x, y, o.bb.l, o.bb.u, o.bb.r, o.bb.u)).X != -1)	//Up
			{
				x = col.X;
				y = col.Y;
				somethingSet = true;
			}
			else if ((col = BB.lineCollisionPoint(px1, py1, x, y, o.bb.l, o.bb.d, o.bb.r, o.bb.d)).X != -1)	//Down
			{
				x = col.X;
				y = col.Y;
				somethingSet = true;
			}
			else
			{
				if (Math.Abs(o.bb.l - x) < base_width)
				{
					x = o.bb.l;
					somethingSet = true;
				}
				else if (Math.Abs(o.bb.r - x) < base_width)
				{
					x = o.bb.r;
					somethingSet = true;
				}

				if (Math.Abs(o.bb.u - y) < base_height)
				{
					y = o.bb.u;
					somethingSet = true;
				}
				else if (Math.Abs(o.bb.d - y) < base_height)
				{
					y = o.bb.d;
					somethingSet = true;
				}

				if (!somethingSet)
				{
					//BAD
					if (Math.Abs(o.x - x) > Math.Abs(o.y - y))
					{
						if (x > o.x)
							x = o.bb.r;
						else
							x = o.bb.l;
						somethingSet = true;
					}
					else
					{
						if (y > o.y)
							y = o.bb.d;
						else
							y = o.bb.u;
						somethingSet = true;
					}
				}
			}
			
			setRegions();
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
				//Move(xspeed * factor, yspeed * factor, true);

				factorSet = false;
				newFactor = 1;
				newGravFactor = 1;
			}
			else
			{
				x = stuckTo.x - xoff;
				y = stuckTo.y - yoff;
			}
			setRegions();

			if (vz != null)
			{
				vz.x = x;
				vz.y = y;
				vz.setRegions();
			}
			if ((Math.Abs(x) > 9999) || (Math.Abs(y) > 9999))
				destroy();
		}

		protected override void doEndTick()
		{
			if (stuckTo != null)
			{
				x = stuckTo.x - xoff;
				y = stuckTo.y - yoff;
				setRegions();
			}
			if (vz != null)
			{
				vz.x = x;
				vz.y = y;
				vz.setRegions();
			}
			px = x;
			py = y;

			base.doEndTick();
		}

		protected override void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			drawSprite(spriteBatch, c, mainSprite, drawP);
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