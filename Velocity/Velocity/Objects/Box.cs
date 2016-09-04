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
			collisionStatic = false;
			isSolid = true;
			takesControls = false;
			canBePushed = true;
			hasGravity = true;
			canAbsorb = false;
			hasBeenFrictioned = false;

			base.init();
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
			Move(xspeed * factor, yspeed * factor, true);
			setRegions();

			factorSet = false;
			newFactor = 1;
			newGravFactor = 1;
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