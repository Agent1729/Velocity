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
	public class MovingPlatform : VelocityObj
	{
		private bool horizontal;
		private float moveDist;
		private float startCoord;
		private float rxspeed, ryspeed;

		public List<obj> objsToMove = new List<obj>();

		public MovingPlatform(float x, float y, bool _horizontal, float _moveSpeed, float _dist) : base(x, y)
		{
			moveDist = _dist;
			horizontal = _horizontal;
			if (_horizontal)
			{
				startCoord = x;
				xspeed = _moveSpeed;
				rxspeed = _moveSpeed;
			}
			else
			{
				startCoord = y;
				yspeed = _moveSpeed;
				ryspeed = _moveSpeed;
			}
		}

		protected override void init()
		{
			objType = "MovingPlatform";
			spriteName = "wall_block";
			base_width = 16; base_height = 16;
			width = base_width; height = base_height;
			depth = -5;
			collisionStatic = false;
			isSolid = true;
			//isSolid = false;
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
				factor = newFactor;

			if (hasGravity)
				yspeed += gravity * Math.Abs(factor);

			capSpeed(terminalVelocity);

			/*
			y -= 1;
			List<obj> colls = level.collisionList(this, true);
			foreach (obj o in colls)
				if (o.canBePushed)
					objsToMove.Add(o);
			y += 1;
			//*/

			//x += xspeed * factor;
			//y += yspeed * factor;
			xspeed = rxspeed;
			yspeed = ryspeed;
			if (Move(rxspeed * factor, ryspeed * factor, true).Length() < 1)
			{
				//rxspeed *= -1;
				//ryspeed *= -1;
			}
			/*
			foreach (obj o in objsToMove)
			{
				if (ryspeed >= 0)
					o.Move(rxspeed * factor, ryspeed * factor, true);
			}//*/

			if (horizontal)
			{
				float x2 = x;
				if (x <= startCoord)
				{
					//x = startCoord;
					Move(startCoord - x, 0, true);
					foreach (obj o in objsToMove)
					{
						if (ryspeed >= 0)
							o.Move(startCoord - x2, 0, true);
					}
					rxspeed *= -1;
				}
				else if (x >= startCoord + moveDist)
				{
					//x = startCoord + moveDist;
					Move(startCoord + moveDist - x, 0, true);
					foreach (obj o in objsToMove)
					{
						if (ryspeed >= 0)
							o.Move(startCoord + moveDist - x2, 0, true);
					}
					rxspeed *= -1;
				}
			}
			else
			{
				float y2 = y;
				if (y <= startCoord)
				{
					//y = startCoord;
					Move(0, startCoord - y, true);
					foreach (obj o in objsToMove)
					{
						//if (ryspeed >= 0)
							o.Move(0, startCoord - y2, true);
					}
					ryspeed *= -1;
				}
				else if (y >= startCoord + moveDist)
				{
					//y = startCoord + moveDist;
					Move(0, startCoord + moveDist - y, true);
					//*
					foreach (obj o in objsToMove)
					{
						//if (ryspeed >= 0)
							o.Move(0, startCoord + moveDist - y2, true);
					}//*/
					ryspeed *= -1;
				}
			}
			objsToMove.Clear();

			factorSet = false;
			newFactor = 1;
		}

		protected override void doCollision(obj other, bool isPrimary)
		{
			if (other.objType == "VelocityZone")
				vZoneCollide(other, isPrimary);
			if (other.objType == "Wall")
			{
				rxspeed *= -1;
				ryspeed *= -1;
			}
			if (other.objType == "MovingPlatform")
			{
				rxspeed *= -1;
				ryspeed *= -1;
			}
			/*if (other.objType == "Box")
			{
				xspeed *= -1;
				yspeed *= -1;
			}//*/
		}
	}
}
