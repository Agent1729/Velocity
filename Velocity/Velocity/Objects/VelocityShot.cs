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
	public class VelocityShot : VelocityObj
	{
		private float xdest, ydest;
		private float scale, vFactor, vGravFactor;
		private int type;
		private VelocityZone vz;
		private Player plr;
		public VelocityNode seeking;
		private float shotSpeed = 10;

		public VelocityShot(float _x, float _y, float _xspeed, float _yspeed) : base(_x, _y)
		{
			xspeed = _xspeed;
			yspeed = _yspeed;
			xdest = 0; ydest = 0;
			scale = 0; vFactor = 0; vGravFactor = 0;
			type = 0;
			vz = null;
			seeking = null;
		}

		protected override void init()
		{
			objType = "StickyShot";
			mainSpriteName = "StickyNode";
			base_width = 4; base_height = 4;
			width = base_width; height = base_height;
			depth = 0;
			collisionStatic = false;
			isSolid = false;
			takesControls = false;
			canBePushed = false;
			hasGravity = false;
			canAbsorb = false;
			hasBeenFrictioned = false;

			base.init();
		}

		public void setVelocityStats(float _xdest, float _ydest, float _scale, float _vFac, float _vGFac, int _type, VelocityZone _vz, Player _plr)
		{
			xdest = _xdest;
			ydest = _ydest;
			scale = _scale;
			vFactor = _vFac;
			vGravFactor = _vGFac;
			type = _type;
			vz = _vz;
			plr = _plr;
		}

		protected override void doCollision(obj other, bool isPrimary)
		{
			if (other.objType == "Wall")
				destroy();
			if (other.objType == "MovingPlatform")
				destroy();
			if (other.objType == "Box")
				destroy();
			if (other.objType == "VelocityZone")
				vZoneCollide(other, isPrimary);
			if (other.objType == "StickyNode")
			{
				VelocityNode o = (VelocityNode)other;
				if ((o.xspeed == 0) && (o.yspeed == 0))
				{
					xdest = o.x;
					ydest = o.y;
					setPortal(o);
				}
			}
			if (other.objType == "PaintNode")
			{
				VelocityNode o = (VelocityNode)other;
				if ((o.xspeed == 0) && (o.yspeed == 0))
				{
					xdest = o.x;
					ydest = o.y;
					setPortal(o);
				}
			}
		}

		protected override void dotick()
		{
			if (seeking != null)
			{
				float dist = (float)Math.Sqrt(Math.Abs(x - seeking.x) * Math.Abs(x - seeking.x)
					+ Math.Abs(y - seeking.y) * Math.Abs(y - seeking.y));

				float ang;
				ang = (float)Math.Atan((seeking.y - y) / (seeking.x - x));
				if (seeking.x < x)
					ang += (float)Math.PI;
				xspeed = (float)Math.Cos(ang) * shotSpeed;
				yspeed = (float)Math.Sin(ang) * shotSpeed;

				xdest = seeking.x;
				ydest = seeking.y;
			}

			if (Math.Abs(x - xdest) < Math.Abs(xspeed * factor) && Math.Abs(y - ydest) < Math.Abs(yspeed * factor) && !destroyed)
			{
				setPortal();
				return;
			}

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
			if ((Math.Abs(x) > 999) || (Math.Abs(y) > 999))
				destroy();
		}

		private void setPortal()
		{
			if (!destroyed)
			{
				if (vz.node != null)
					vz.node.vz = null;

				vz.x = xdest; vz.y = ydest;
				vz.rescale(scale);
				vz.vFactor = vFactor;
				vz.vGravFactor = vGravFactor;
				vz.type = type;

				destroy();
			}
		}

		private void setPortal(VelocityNode o)
		{
			if (vz.node != null)
				vz.node.vz = null;
			o.vz = vz;
			vz.node = o;

			vz.x = xdest; vz.y = ydest;
			vz.rescale(scale);
			vz.vFactor = vFactor;
			vz.vGravFactor = vGravFactor;
			vz.type = type;

			destroy();
		}

		protected override void destroy()
		{
			plr.velocityShot = null;
			base.destroy();
		}
	}
}
