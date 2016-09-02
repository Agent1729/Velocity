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
	public class Box : VelocityObj
	{
		//public float moveSpeed;
		public bool grounded = false;
		public float friction = .25f;
		public float maxXSpeed = 3;

		public List<float> xspeeds = new List<float>();
		public List<float> yspeeds = new List<float>();
		public List<float> xdists = new List<float>();
		public List<float> ydists = new List<float>();
		public float xpre;
		public float ypre;
		public int logsToHold = 10;

		public Box(float x, float y)
			: base(x, y)
		{
		}

		protected override void init()
		{
			objType = "Box";
			mainSpriteName = "Box";
			base_width = 16; base_height = 16;
			width = base_width; height = base_height;
			depth = 5;
			mass = 20;
			pushForce = 1;
			inertia = 10;
			collisionStatic = false;
			isSolid = true;
			takesControls = false;
			canBePushed = true;
			hasGravity = true;
			canAbsorb = false;
			hasBeenFrictioned = false;

			base.init();
		}

		protected override void doleftPressed(object lvl)
		{
			xspeed = -5;
		}

		protected override void dorightPressed(object lvl)
		{
			xspeed = 5;
		}

		protected override void doBeginTick()
		{
			base.doBeginTick();

			xpre = x;
			ypre = y;
		}

		protected override void doEndTick()
		{
			base.doEndTick();

			xspeeds.Add(xspeed);
			yspeeds.Add(yspeed);
			xdists.Add(x - xpre);
			ydists.Add(y - ypre);
			if (xspeeds.Count > logsToHold) xspeeds.RemoveAt(0);
			if (yspeeds.Count > logsToHold) yspeeds.RemoveAt(0);
			if (xdists.Count > logsToHold) xdists.RemoveAt(0);
			if (ydists.Count > logsToHold) ydists.RemoveAt(0);
		}

		protected override void dotick()
		{
			hasBeenFrictioned = false;
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


			if (hasGravity && !isGrounded())
			{
				if (!factorAffectsGrav)
					yspeed += gravity * factor * gravFactor;
				else
					yspeed += gravity * gravFactor;
			}

			capSpeed(terminalVelocity);
			if (!hasMoved)
			{
				Move(xspeed * factor, yspeed * factor, true);
				setRegions();
			}

			frictionSelf();
			capHorizontalSpeed(maxXSpeed);

			factorSet = false;
			newFactor = 1;
			newGravFactor = 1;
		}

		public void frictionSelf()
		{
			//if (!isGrounded()) return;
			if (ticksSincePushed >= 0)
				return;
			if(xspeed>0)
			{
				if (xspeed > friction * factor * gravFactor)
				{
					xspeed -= friction * factor * gravFactor;
				}
				else
				{
					xspeed = 0;
				}
			}
			else if(xspeed<0)
			{
				if (xspeed < -friction * factor * gravFactor)
				{
					xspeed += friction * factor * gravFactor;
				}
				else
				{
					xspeed = 0;
				}
			}
		}

		public void capHorizontalSpeed(float max)
		{
			if (xspeed > max) xspeed = max;
			if (xspeed < -max) xspeed = -max;
		}

		protected override void doCollision(obj other, bool isPrimary)
		{
			if (other.objType == "VelocityZone")
				vZoneCollide(other, isPrimary);
		}

		public override bool isGrounded()
		{
			List<obj> colls = level.collisionListAtRelative(this, 0, obj.Sign(gravFactor), true);

			if (!noColls(colls))
			{
				if (yspeed * gravFactor > 0)
					yspeed = 0;
				return true;
			}
			return false;
		}
	}
}