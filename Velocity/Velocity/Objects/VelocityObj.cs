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
	public class VelocityObj : obj
	{
		public float factor, newFactor = 1;
		public bool factorSet = false;
		public float gravFactor, newGravFactor = 1;
		public bool factorAffectsGrav = false;
		public float paintedTime = 0;
		public Color paintedColor = Color.White;

		public float gravity = .3f;
		public bool hasGravity;
		public float terminalVelocity = 20;

		public VelocityObj(float x, float y) : base(x, y) { }

		protected override void init()
		{
			//base.init();

			factor = 1;

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

			//x += xspeed * factor;
			//y += yspeed * factor;
			if (!hasMoved)
			{
				if (xspeed != 0 || yspeed != 0)
					Move(xspeed * factor, yspeed * factor, true);
			}

			factorSet = false;
			newFactor = 1;
			newGravFactor = 1;
		}

		protected override void doCollision(obj other, bool isPrimary)
		{
			if (other.objType == "VelocityZone")
				vZoneCollide(other, isPrimary);
		}

		protected virtual void vZoneCollide(obj other, bool isPrimary)
		{
			VelocityZone vz = other as VelocityZone;
			newFactor = vz.vFactor;
			newGravFactor = vz.vGravFactor;
			factorSet = true;
			factorAffectsGrav = vz.timeDoesntAffectGrav;
			paintedTime = vz.paintTime;
		}

		public override bool isGrounded() { return false; }

		public bool canPush(obj o, float spd)
		{
			VelocityObj other = (VelocityObj)o;
			if (!other.canBePushed) return false;
			if (pushForce >= other.inertia * other.gravFactor)
				return true;
			if (Math.Abs(mass * spd) >= other.inertia * other.gravFactor)
				return true;
			return false;
		}

		//protected override bool moveToOther(obj _b, float axspeed, float ayspeed)
		//{
		//    float xsd, ysd;
		//    float i;
		//    float inc1 = .1f;
		//    float ix = x, iy = y;
		//    bool ret = true;

		//    VelocityObj b = (VelocityObj)_b;

		//    //if (level.collidesSolid(this))
		//    //{
		//    //    y -= 1;
		//    //    if (!level.collidesSolid(this))
		//    //    {
		//    //        yspeed = 0;
		//    //        return true;
		//    //    }
		//    //    y += 1;
		//    //}

		//    { xsd = axspeed * factor; ysd = ayspeed * factor; }

		//    x -= xsd; y -= ysd;
		//    bool needsPushed = false;
		//    needsPushed = level.collidesSolid(this);
		//    if (b.collisionStatic)
		//        needsPushed = false;
		//    if (needsPushed)
		//    {
		//        x += xsd; y += ysd;

		//        if (b.xspeed * b.factor == 0)
		//        {
		//            ret = getPushedByOther(b, 0, ayspeed);
		//            //umoving = false; dmoving = false;
		//        }
		//        if (b.yspeed * b.factor == 0)
		//        {
		//            ret = getPushedByOther(b, axspeed, 0);
		//            //lmoving = false; rmoving = false;
		//        }
		//        else
		//        {
		//            ret = true;
		//        }
		//        return ret;
		//        /*
		//        { xsd = xspeed * factor - b.xspeed * b.factor; ysd = yspeed * factor - b.yspeed * b.factor; }
		//        x -= xsd;
		//        y -= ysd;
		//        //moveToOther(b);
		//        //return;

		//        x += xsd; bool xPushing = level.collidesSolid(this); x -= xsd; if (!xPushing) xsd = 0;
		//        y += xsd; bool yPushing = level.collidesSolid(this); y -= xsd; if (!yPushing) ysd = 0;

		//        if (xPushing) { x -= xsd; if (level.collidesSolid(this)) { return true; } x += xsd; }
		//        if (yPushing) { y -= ysd; if (level.collidesSolid(this)) { return true; } y += ysd; }
		//        */
		//    }



		//    for (i = 0f; i < 1f; i += inc1)
		//    {
		//        x += inc1 * xsd; y += inc1 * ysd;
		//        if (level.collidesSolid(this))
		//            break;
		//    }

		//    x -= inc1 * xsd; y -= inc1 * ysd;
		//    i -= inc1;

		//    char xy = ' ';
		//    x += inc1 * xsd;
		//    if (level.collidesSolid(this))
		//    {
		//        xy = 'y';
		//        x -= inc1 * xsd;
		//        if (!needsPushed)
		//        { xspeed = 0; lmoving = false; rmoving = false; }
		//    }
		//    y += inc1 * ysd;
		//    if (level.collidesSolid(this))
		//    {
		//        xy = 'x';
		//        y -= inc1 * ysd;
		//        if(!needsPushed)
		//        { yspeed = 0; umoving = false; dmoving = false; }
		//    }

		//    if (xy == ' ')
		//    {
		//        //WTF DAVID BLAINE
		//        xy = ' ';
		//    }

		//    x -= inc1 * xsd;
		//    for (float r = i * inc1; r < 1f; r += inc1)
		//    {
		//        if (xy == 'x')
		//            x += inc1 * xsd;
		//        else if (xy == 'y')
		//            y += inc1 * ysd;
		//        if (level.collidesSolid(this))
		//        {
		//            if (xy == 'x')
		//            {
		//                x -= inc1 * xsd;
		//                xspeed = 0; lmoving = false; rmoving = false;
		//            }
		//            else if (xy == 'y')
		//            {
		//                y -= inc1 * ysd;
		//                yspeed = 0; umoving = false; dmoving = false;
		//            }
		//            break;
		//        }
		//    }

		//    if (level.collidesSolid(this))
		//        return false;
		//    return true;
		//}

		//protected override bool getPushedByOther(obj _b, float axspeed, float ayspeed)
		//{
		//    float xsd, ysd;
		//    float i;
		//    float inc1 = .1f;
		//    float ix = x, iy = y;
		//    bool ret = true;

		//    VelocityObj b = (VelocityObj)_b;

		//    //xsd = axspeed * factor - b.xspeed * b.factor; ysd = ayspeed * factor - b.yspeed * b.factor;
		//    xsd = Math.Abs(axspeed * factor - b.xspeed * b.factor); ysd = Math.Abs(ayspeed * factor - b.yspeed * b.factor);
		//    if (x > b.x) xsd *= -1;
		//    if (y > b.y) ysd *= -1;


		//    while (level.collidesSolid(this))
		//    {
		//        x -= inc1 * xsd;
		//        y -= inc1 * ysd;
		//    }

		//    if (Math.Abs(x - ix) + Math.Abs(y - iy) > Math.Abs(xsd) + Math.Abs(ysd))
		//    {
		//        x = ix; y = iy;
		//        y -= 2;

		//        while (level.collidesSolid(this))
		//        {
		//            x -= inc1 * xsd;
		//            y -= inc1 * ysd;
		//        }
		//        if (Math.Abs(x - ix) + Math.Abs(y - iy) > Math.Abs(xsd) + Math.Abs(ysd))
		//        {
		//            //Moved farther than it should
		//            //Aka got squished?
		//            x += 0;
		//        }
		//    }


		//    return ret;
		//}

		//protected override bool moveFromOthers()
		//{
		//    List<obj> colls = level.collisionList(this, true);

		//    if (colls.Count == 0)
		//        return true;

		//    float distL = 0, distR = 0, distU = 0, distD = 0;
		//    float dL, dR, dU, dD;
		//    float safeDist = 100f;
		//    float ix = x, iy = y;

		//    if (objType == "Player")
		//    {
		//        safeDist += 0;
		//    }

		//    //TRY #1: One at a time
		//    bool otherGrounded = false;
		//    foreach (obj j in colls)
		//    {
		//        distL = this.bb.r - j.bb.l;
		//        distR = j.bb.r - this.bb.l;
		//        distU = this.bb.d - j.bb.u;
		//        distD = j.bb.d - this.bb.u;
		//        if (j.isGrounded())
		//            otherGrounded = true;

		//        if ((distD <= distL) && (distD <= distR) && (distD <= distU) && (otherGrounded == false))
		//        {
		//            y += distD;
		//            if (yspeed < 0) yspeed = 0;
		//            umoving = false;
		//        }
		//        else if ((distL <= distR) && (distL <= distU) && (distL <= distD))
		//        {
		//            x -= distL;
		//            if (xspeed > 0) xspeed = 0;
		//            rmoving = false;
		//        }
		//        else if ((distR <= distU) && (distR <= distL) && (distR <= distD))
		//        {
		//            x += distR;
		//            if (xspeed < 0) xspeed = 0;
		//            lmoving = false;
		//        }
		//        else if ((distU <= distL) && (distU <= distR) && (distU <= distD))
		//        {
		//            y -= distU;
		//            if (yspeed > 0) yspeed = 0;
		//            dmoving = false;
		//            //umoving = false;
		//        }
		//        else if (otherGrounded && (j.y > y))
		//        {
		//            y -= distU;
		//            if (yspeed > 0) yspeed = 0;
		//            dmoving = false;
		//            //umoving = false;
		//        }
		//    }
		//    colls = level.collisionList(this, true);
		//    if (colls.Count == 0)
		//        return true;


		//    //TRY #2: All at once
		//    distL = 0; distR = 0; distU = 0; distD = 0;
		//    otherGrounded = false;
		//    foreach (obj j in colls)
		//    {
		//        dL = this.bb.r - j.bb.l;
		//        dR = j.bb.r - this.bb.l;
		//        dU = this.bb.d - j.bb.u;
		//        dD = j.bb.d - this.bb.u;
		//        if (dL > distL) distL = dL;
		//        if (dR > distR) distR = dR;
		//        if (dU > distU) distU = dU;
		//        if (dD > distD) distD = dD;
		//        if (j.isGrounded())
		//            otherGrounded = true;
		//    }

		//    if ((distD <= distL) && (distD <= distR) && (distD <= distU) && (otherGrounded == false))
		//    {
		//        y += distD;
		//        if (yspeed < 0) yspeed = 0;
		//        umoving = false;
		//    }
		//    else if ((distL <= distR) && (distL <= distU) && (distL <= distD))
		//    {
		//        x -= distL;
		//        if (xspeed > 0) xspeed = 0;
		//        rmoving = false;
		//    }
		//    else if ((distR <= distU) && (distR <= distL) && (distR <= distD))
		//    {
		//        x += distR;
		//        if (xspeed < 0) xspeed = 0;
		//        lmoving = false;
		//    }
		//    else if ((distU <= distL) && (distU <= distR) && (distU <= distD))
		//    {
		//        y -= distU;
		//        if (yspeed > 0) yspeed = 0;
		//        dmoving = false;
		//        //umoving = false;
		//    }
		//    /*else if (otherGrounded)
		//    {
		//        y -= distU;
		//        if (yspeed > 0) yspeed = 0;
		//        dmoving = false;
		//        //umoving = false;
		//    }//*/

		//    colls = level.collisionList(this, true);
		//    if (colls.Count == 0)
		//        return true;


		//    //TRY #3: One at a time, last ditch effort
		//    otherGrounded = false;
		//    foreach (obj j in colls)
		//    {
		//        distL = this.bb.r - j.bb.l;
		//        distR = j.bb.r - this.bb.l;
		//        distU = this.bb.d - j.bb.u;
		//        distD = j.bb.d - this.bb.u;
		//        if (j.isGrounded())
		//            otherGrounded = true;

		//        if ((distD <= distL) && (distD <= distR) && (distD <= distU) && (otherGrounded == false))
		//        {
		//            y += distD;
		//            if (yspeed < 0) yspeed = 0;
		//            umoving = false;
		//        }
		//        else if ((distL <= distR) && (distL <= distU) && (distL <= distD))
		//        {
		//            x -= distL;
		//            if (xspeed > 0) xspeed = 0;
		//            rmoving = false;
		//        }
		//        else if ((distR <= distU) && (distR <= distL) && (distR <= distD))
		//        {
		//            x += distR;
		//            if (xspeed < 0) xspeed = 0;
		//            lmoving = false;
		//        }
		//        else if ((distU <= distL) && (distU <= distR) && (distU <= distD))
		//        {
		//            y -= distU;
		//            if (yspeed > 0) yspeed = 0;
		//            dmoving = false;
		//            //umoving = false;
		//        }
		//        else if (otherGrounded && (j.y > y))
		//        {
		//            y -= distU;
		//            if (yspeed > 0) yspeed = 0;
		//            dmoving = false;
		//            //umoving = false;
		//        }
		//    }
		//    colls = level.collisionList(this, true);
		//    if (colls.Count == 0)
		//        return true;


		//    //Can't properly uncollide
		//    //Probably being squished?
		//    {
		//        int asdf = 0;
		//        //x = ix; y = iy;
		//    }
		//    return false;
		//}

		protected void ActuallyMove(float mx, float my)
		{
			List<obj> collsTEST = level.collisionListAtRelative(this, mx, 0, true);
			if (collsTEST.Count > 0)
			{
				int asdf = 0;
			}
			//x+=mx;
			x = (float)Math.Round(x + mx, 5);

			collsTEST = level.collisionListAtRelative(this, 0, my, true);
			if (collsTEST.Count > 0)
			{
				int asdf = 0;
			}
			//y+=my;
			y = (float)Math.Round(y + my, 5);

			setRegions();
		}

		protected static float makeDistSafe(float dist)
		{
			float safeDist = .01f;
			float ans = dist - safeDist * obj.Sign(dist);
			if (obj.Sign(ans) != obj.Sign(dist))
				ans = 0;
			return ans;
		}

		public override Vector2 MoveOld(float mx, float my, bool actuallyMove)
		{
			isMoving = true;
			float ix = x, iy = y;
			List<obj> colls;
			List<obj> colls2 = new List<obj>();
			List<obj> colls3 = new List<obj>();
			List<obj> colls4 = new List<obj>();
			List<obj> colls5 = new List<obj>();
			List<obj> collsTEST = new List<obj>();
			float dx = 9999, px = 9999;
			float dy = 9999, py = 9999;
			float movedx = mx, movedy = my;
			float safeDist = .01f;
			float tooMuch = 20;
			float safePushDist = 1;

			colls = level.collisionList(this, true);
			while (colls.Count != 0)
			{
				//Pre colliding, GLITCH!!!
				int asdf = 0;
				//As a temp fix, just move up above it
				y -= 1;
				setRegions();

				colls = level.collisionList(this, true);
			}

			colls4 = level.collisionListAtRelative(this, 0, -1, true);
			colls5 = level.collisionListAtRelative(this, 0, 1, true);
			//x += mx;
			colls = level.collisionListAtRelative(this, mx, 0, true);
			if (colls.Count != 0)
			{
				//x -= mx;
				//Check furthest can move
				foreach (obj o in colls)
				{
					if ((mx > 0) && (o.bb.l - bb.r <= dx))
						dx = o.bb.l - bb.r;
					if ((mx < 0) && (bb.l - o.bb.r <= dx))
						dx = bb.l - o.bb.r;
				}

				//Move!
				if (mx > 0)
				{
					if (Math.Abs(mx) < Math.Abs(dx))
						movedx = mx;
					else
						movedx = dx;
				}
				if (mx < 0)
				{
					if (Math.Abs(mx) < Math.Abs(dx))
						movedx = mx;
					else
						movedx = -dx;
				}
				movedx = makeDistSafe(movedx);
				ActuallyMove(movedx, 0);

				//Check who got collid
				foreach (obj o in colls)
				{
					if ((mx > 0) && (o.bb.l - bb.r == dx))
						colls2.Add(o);
					if ((mx < 0) && (bb.l - o.bb.r == dx))
						colls2.Add(o);
				}

				//Try to push them
				foreach (obj o in colls2)
				{
					if (o.canBePushed)
					{
						colls3.Add(o);
						float tx = o.Move(mx - movedx, 0, false).X;
						if (Math.Abs(tx) <= Math.Abs(px))
							px = tx;
					}
					else
						px = 0;
				}
				if (Math.Abs(px) < safeDist * 2)
					px = 0;
				foreach (obj o in colls3)
				{
					o.Move(px, 0, actuallyMove||true);
					if (colls4.Contains(o))
						colls4.Remove(o);
					if (colls5.Contains(o))
						colls5.Remove(o);
				}
				if ((px != 9999) && (px != 0))
					//{ movedx += px; x += px; }
					movedx += Move(px, 0, true).X;

				//Collide with them
				foreach (obj o in colls2)
				{
					//Collide it
				}

			}
			else
				ActuallyMove(makeDistSafe(mx), 0);

			//Friction things above/below
			if (actuallyMove)
			{
				if (Math.Abs(movedx) > 0)
				{
					foreach (obj j in colls4)
					{
						VelocityObj o = j as VelocityObj;
						if (objType == "Player")
						{
							int asdf = 0;
						}
						if ((o.canBePushed) && (!o.hasBeenFrictioned) && (o.gravFactor > 0) && (!o.isMoving))
						{
							//o.hasBeenFrictioned = true;
							o.Move(movedx, 0, true);
						}
					}
				}
				if (Math.Abs(movedx) > 0)
				{
					foreach (obj j in colls5)
					{
						VelocityObj o = j as VelocityObj;
						if ((o.canBePushed) && (!o.hasBeenFrictioned) && (o.gravFactor < 0) && (!o.isMoving))
						{
							//o.hasBeenFrictioned = true;
							o.Move(movedx, 0, true);
						}
					}
				}
			}
			if (Math.Abs(movedx) > tooMuch)
			{
				int asdf = 0;
			}
			


			colls2.Clear();
			colls3.Clear();
			colls4.Clear();
			colls5.Clear();
			collsTEST.Clear();
			colls4 = level.collisionListAtRelative(this, 0, -1, true);
			colls5 = level.collisionListAtRelative(this, 0, 1, true);
			//y += my;
			colls = level.collisionListAtRelative(this, 0, my, true);
			if (colls.Count != 0)
			{
				//y -= my;
				//Check furthest can move
				foreach (obj o in colls)
				{
					if ((my > 0) && (o.bb.u - bb.d <= dy))
						dy = o.bb.u - bb.d;
					if ((my < 0) && (bb.u - o.bb.d <= dy))
						dy = bb.u - o.bb.d;
				}

				//Move!
				if (my > 0)
				{
					if (Math.Abs(my) < Math.Abs(dy))
						movedy = my;
					else
						movedy = dy;
				}
				if (my < 0)
				{
					if (Math.Abs(my) < Math.Abs(dy))
						movedy = my;
					else
						movedy = -dy;
				}
				movedy = makeDistSafe(movedy);
				ActuallyMove(0, movedy);

				//Check who got collid
				foreach (obj o in colls)
				{
					if ((my > 0) && (o.bb.u - bb.d == dy))
						colls2.Add(o);
					if ((my < 0) && (bb.u - o.bb.d == dy))
						colls2.Add(o);
				}

				//Try to push them
				foreach (obj o in colls2)
				{
					if (o.canBePushed)
					{
						colls3.Add(o);
						float ty = o.Move(0, my - movedy, false).Y;
						if (Math.Abs(ty) <= Math.Abs(py))
							py = ty;
					}
					else
						py = 0;
				}
				if (Math.Abs(py) < safeDist * 2)
					py = 0;
				foreach (obj o in colls3)
				{
					o.Move(0, py, actuallyMove || true);
					if (colls4.Contains(o))
						colls4.Remove(o);
					if (colls5.Contains(o))
						colls5.Remove(o);
				}
				if ((py != 9999) && (py != 0))
					//{ movedy += py; y += py; }
					movedy += Move(0, py, true).Y;

				//Collide with them
				foreach (obj o in colls2)
				{
					//Collide it
				}
			}
			else
				ActuallyMove(0, makeDistSafe(my));

			//Friction things above/below
			if (actuallyMove)
			{
				if ((Math.Abs(movedy) > 0) && (movedy > 0))
				{
					foreach (obj j in colls4)
					{
						VelocityObj o = j as VelocityObj;
						if ((o.canBePushed) && (!o.hasBeenFrictioned) && (o.gravFactor > 0) && (!o.isMoving))
						{
							//o.hasBeenFrictioned = true;
							o.Move(0, movedy, true);
						}
					}
				}
				if ((Math.Abs(movedy) > 0) && (movedy < 0))
				{
					foreach (obj j in colls5)
					{
						VelocityObj o = j as VelocityObj;
						if ((o.canBePushed) && (!o.hasBeenFrictioned) && (o.gravFactor < 0) && (!o.isMoving))
						{
							//o.hasBeenFrictioned = true;
							o.Move(0, movedy, true);
						}
					}
				}
			}
			if (objType == "Player")
			{
				int asdf = 0;
			}
			if (Math.Abs(movedy) > tooMuch)
			{
				int asdf = 0;
			}



			if (actuallyMove)
			{
				if ((mx < 0) && (movedx > mx))
				{
					if (xspeed * factor < 0)
						xspeed = 0;
					lmoving = false;
				}
				if ((mx > 0) && (movedx < mx))
				{
					if (xspeed * factor > 0)
						xspeed = 0;
					rmoving = false;
				}
				if ((my < 0) && (movedy > my))
				{
					if (yspeed * factor < 0)
						yspeed = 0;
					umoving = false;
					isGrounded();
				}
				if ((my > 0) && (movedy < my))
				{
					if (yspeed * factor > 0)
						yspeed = 0;
					dmoving = false;
					isGrounded();
				}
			}
			if (!actuallyMove)
			{ x = ix; y = iy; }

			//if (level.collidesSolid(this))
			//{ x = ix; y = iy; }

			isMoving = false;
			setRegions();
			return new Vector2(movedx, movedy);
		}

		public override Vector2 Move(float mx, float my, bool setHasMovedTrue)
		{
			return MoveHelped(mx, my, setHasMovedTrue, 0, 0);
		}

		public override Vector2 Move(float mx, float my, bool setHasMovedTrue, int layers, int moveLayers)
		{
			return MoveHelped(mx, my, setHasMovedTrue, layers, moveLayers);
		}

		public override Vector2 MoveHelped(float mx, float my, bool setHasMovedTrue, int layers, int moveLayers)
		{
			float dampen = .95f;
			float prex = x;
			float prey = y;
			setRegions();
			//public Vector2 Move(movex, movey)
			//{
			//	do
			//	{
			//		1. Test if anything is in path 
			//			a. Initial BB, final BB, 2 lines
			//				i. Lines are tangents to BBi BBf
			//				ii. How to find them?
			//			b. Better solution: Make 6 line BB, check for total area hitbox vs path
			//				i. Actually use new BB with all BBi, BBf, BBi->BBf lines?
			//		2. If not, move
			//			a. Collide with all nonsolids enroute
			//			b. Return
			//		3. Else, check which solid collid with first
			//			a. Foreach solid s collid with
			//				i. Foreach corner c of self BB
			//					A. Check line BBi_c -> BBf_c
			//					B. To see which lines of BBother it collid with
			//					C. Break with first(closest) collision point
			//				ii. Or use robust function: SEE BELOW FOR DETAILS
			//			b. Break with first(closest) collision point of solids
			//		4. Stop at the closest point
			//			a. Set speeds according to slope
			//			b. Keep remainder of distance
			//			c. Collide with all nonsolids enroute
			//		5. Push obj stopped at: movegroup(this+other, velocity=remainingAfterSlope)
			//	} while(dist obj was pushed>0)
			//	return dist moved
			//}
			//
			//setRegions() at end of everything?

			if (layers > 2)
			{
				x += 0;
				return new Vector2(0, 0);
			}
			if(moveLayers > 2)
			{
				x += 0;
				return new Vector2(0, 0);
			}
			if(objType == "Box")
			{
				if(my<0)
				{
					int i = 0;
				}
			}

			List<obj> preColls = level.collisionList(this, true);

			if (objType == "Player")
			{
				int i = 0;
			}
			if (preColls.Count > 0 && !(preColls.Count == 1 && preColls[0] == this))
			{
				int j = 0;
			}
			List<obj> colls;
			List<obj> collsDontTrustUnion;
			colls = level.collisionList(this, true);
			collsDontTrustUnion = level.collisionListAtRelative(this, mx, my, true);
			colls = colls.Union(level.collisionListAtRelative(this, mx, my, true)).ToList();
			colls = colls.Union(level.collisionListAlongLine(bb.l, bb.u, bb.l + mx, bb.u + my, true)).ToList();
			colls = colls.Union(level.collisionListAlongLine(bb.l, bb.d, bb.l + mx, bb.d + my, true)).ToList();
			colls = colls.Union(level.collisionListAlongLine(bb.r, bb.u, bb.r + mx, bb.u + my, true)).ToList();
			colls = colls.Union(level.collisionListAlongLine(bb.r, bb.d, bb.r + mx, bb.d + my, true)).ToList();
			//colls now contains EVERYTHING we could pass through enroute

			if (noColls(colls) && noColls(collsDontTrustUnion))	//Found nothing!
			{
				//Find all nonsolids
				//Collide all nonsolids
				if (objType == "Player")
				{
					int i = 0;
				}
				x += mx;
				y += my;
				setRegions();
				List<obj> postColls = level.collisionList(this, true);
				if (!noColls(postColls))
					x += 0;
				if (setHasMovedTrue) hasMoved = true;
				return new Vector2(mx, my);
			}

			//Check which solid is closest, and at which point
			if (objType == "Player")
			{
				int i = 0;
			}
			float pct = 2;
			float ang = -1;
			obj bestO = this;
			CollisionInfo bestCI = new CollisionInfo(false);
			foreach(obj o in colls)
			{
				if (o == this) continue;
				//Vector2 result = findClosestCollisionsPercent(mx, my, o);
				CollisionInfo result = findClosestCollisionsPercent(mx, my, o);
				//float percentMoved = result.X;
				float percentMoved = result.pct;
				if (percentMoved >= 0 && percentMoved < pct)
				{
					pct = percentMoved;
					ang = result.getSlope();
					bestO = o;
					bestCI = result;
				}
				//if (percentMoved >= 0)
				//{
				//	pct = Math.Min(pct, percentMoved);
				//	//ang = result.Y;
				//	ang = result.getSlope();
				//	bestO = o;
				//	bestCI = result;
				//}
			}
			if (pct == 2)
				pct = 0;

			{
				//Find all nonsolids
				//Make list of all nonsolids collid
				//pct *= dampen;

				//x += mx * pct * dampen;
				//y += my * pct * dampen;
				Vector2 movedtoci;
				if (bestCI.pct > .001f)
					movedtoci = MoveToCI(mx, my, bestCI);
				else
					movedtoci = new Vector2(0, 0);
				if (mx != 0)
					pct = movedtoci.X / mx;
				else
					pct = movedtoci.Y / my;
				//pct = bestCI.pct;
				ang = bestCI.getSlope();
				if (pct > 1.05f)
					x += 0;
				setRegions();
				List<obj> postColls = level.collisionList(this, true);
				if (!noColls(postColls))
				{
					x += 0;
					//x -= mx * pct;
					//y -= my * pct;
					float postx = x;
					float posty = y;
					x = prex;
					y = prey;
					setRegions();
					findClosestCollisionsPercent(mx, my, bestO);
					movedtoci = MoveToCI(mx, my, bestCI);
					//x += mx * pct;
					//y += my * pct;
					x = postx;
					y = posty;
					setRegions();
				}


				//Don't return, keep going remainder of distance perpendicular to slope, then return
				float mx2 = 0;
				float my2 = 0;
				Vector2 moved2 = new Vector2(0, 0);
				float spd = 0;
				if (ang == 0) spd = yspeed;
				if (ang == 1) spd = xspeed;
				if (!canPush(bestO, spd))
				{
					if (ang == 0)	//Horizontal hit
					{
						yspeed = 0;
						dmoving = false;
						umoving = false;
						isGrounded();
						mx2 = mx * (1.0f - pct);
					}
					if (ang == 1)	//Vertical hit
					{
						xspeed = 0;
						lmoving = false;
						rmoving = false;
						my2 = my * (1.0f - pct);
					}
					if ((mx2 != 0 || my2 != 0))
						moved2 = MoveHelped(mx2, my2, false, layers + 1, moveLayers);
					List<obj> postColls2 = level.collisionList(this, true);
					if (!noColls(postColls2))
						x += 0;
				}
				else	//Other can be pushed
				{
					if (!objsPushed.Contains(bestO))
					{
						objsPushed.Add(bestO);
						if (bestO.objType == "Box")
							doCollision(bestO, true);
						Vector2 pushed = new Vector2(0, 0);
						if (ang == 0)
						{
							if (!objsPushedLast.Contains(bestO))
							{
								//Didn't push last time, use inelastic collision with momentum
								/*
								mx2 = mx * (1.0f - pct) * dampen;
								my2 = my * (1.0f - pct) * dampen;
								pushed = bestO.MoveHelped(0, my2, false, layers, moveLayers + 1);
								moved2 = MoveHelped(mx2, pushed.Y, false, layers, moveLayers + 1);//*/
								VelocityObj vBestO = (VelocityObj)bestO;
								float totalMomentum = mass * yspeed + vBestO.mass * vBestO.yspeed;
								float totalMass = mass + vBestO.mass;
								float finalSpeed = totalMomentum / totalMass;
								if (canBePushed)
									yspeed = finalSpeed;
								vBestO.yspeed = finalSpeed;
								pushed = bestO.MoveHelped(vBestO.xspeed * factor, vBestO.yspeed * factor, true, layers, moveLayers + 1);
								bestO.ticksSincePushed = 0;

								//if (mx != 0) mx2 = xspeed / mx * (1.0f - pct) * dampen;
								//if (my != 0) my2 = yspeed / my * (1.0f - pct) * dampen;
								//moved2 = MoveHelped(mx2, my2, true, layers, moveLayers + 1);

								//Method 2
								CollisionInfo pushedCI = findClosestCollisionsPercent(xspeed, yspeed, vBestO);
								moved2 = MoveToCI(xspeed, yspeed, pushedCI);

								//Method 3
								//if (mx != 0) mx2 = xspeed / mx * (1.0f - pct);
								//if (my != 0) my2 = yspeed / my * (1.0f - pct);
								//moved2 = Move(mx2 * factor, my2 * factor, true, layers, moveLayers + 1);

								if (Math.Abs(pushed.Y) < Math.Abs(my2 * dampen))
								{
									yspeed = 0;
									dmoving = false;
									umoving = false;
									isGrounded();
								}
							}
							else
							{
								//We pushed this last round too, use F=ma
								//TODO (copy paste from below)
							}
							//Now move the rest of the x distance if applicable
							//TODO
						}
						if (ang == 1)
						{
							VelocityObj vBestO = (VelocityObj)bestO;
							if (!objsPushedLast.Contains(bestO))
							{
								float totalMomentum = mass * xspeed + vBestO.mass * vBestO.xspeed;
								float totalMass = mass + vBestO.mass;
								float finalSpeed = totalMomentum / totalMass;
								if (canBePushed)
									xspeed = finalSpeed;
								vBestO.xspeed = finalSpeed;
								pushed = bestO.MoveHelped(vBestO.xspeed * vBestO.factor, vBestO.yspeed * vBestO.factor, true, layers, moveLayers + 1);
								bestO.ticksSincePushed = 0;

								//if (mx != 0) mx2 = xspeed / mx * (1.0f - pct) * dampen;
								//if (my != 0) my2 = yspeed / my * (1.0f - pct) * dampen;
								//moved2 = MoveHelped(mx2, my2, true, layers, moveLayers + 1);

								//Method 2
								CollisionInfo pushedCI = findClosestCollisionsPercent(xspeed, yspeed, vBestO);
								moved2 = MoveToCI(xspeed, yspeed, pushedCI);

								//Method 3
								//if (mx != 0) mx2 = xspeed / mx * (1.0f - pct);
								//if (my != 0) my2 = yspeed / my * (1.0f - pct);
								//moved2 = Move(mx2 * factor, my2 * factor, true, layers, moveLayers + 1);

								/*
								mx2 = mx * (1.0f - pct) * dampen;
								my2 = my * (1.0f - pct) * dampen;
								pushed = bestO.MoveHelped(mx2, 0, false, layers, moveLayers + 1);
								moved2 = MoveHelped(pushed.X, my2, false, layers, moveLayers + 1);//*/
								if (Math.Abs(pushed.X) < Math.Abs(mx2 * dampen))
								{
									xspeed = 0;
									lmoving = false;
									rmoving = false;
								}
							}
							else
							{
								//We pushed this last round too, use F=ma
								float accel = pushForce * fmaPushMod / bestO.mass;
								if (mx > 0)
								{
									bestO.xspeed += accel * factor;
									if (bestO.xspeed > xspeed) bestO.xspeed = xspeed;
								}
								if (mx < 0)
								{
									bestO.xspeed -= accel * factor;
									if (bestO.xspeed < xspeed) bestO.xspeed = xspeed;
								}
								pushed = bestO.MoveHelped(vBestO.xspeed * vBestO.factor, vBestO.yspeed * vBestO.factor, true, layers, moveLayers + 1);
								bestO.ticksSincePushed = 0;
								//Method 2
								//CollisionInfo pushedCI = findClosestCollisionsPercent(xspeed, yspeed, vBestO);
								//moved2 = MoveToCI(xspeed, yspeed, pushedCI);
								//Method 3
								if (mx != 0) mx2 = xspeed / mx * (1.0f - pct);
								if (my != 0) my2 = yspeed / my * (1.0f - pct);
								moved2 = Move(mx2 * factor, my2 * factor, true, layers, moveLayers + 1);
							}
							//Now move the rest of the y distance if applicable
							//TODO
						}
						List<obj> postColls2 = level.collisionList(this, true);
						if (!noColls(postColls2))
							x += 0;
					}
					else
					{
						//We already pushed this object this tick...
						//Probably in some glitched infinite loop
						x += 0;
					}
				}
				if (setHasMovedTrue) hasMoved = true;
				return (new Vector2(mx * pct, my * pct) + moved2) * dampen;
			}


			setRegions();
			if (setHasMovedTrue) hasMoved = true;
			return new Vector2();
		}

		public Vector2 MoveToCI(float mx, float my, CollisionInfo ci)
		{
			float movetocidampen = .95f;
			if (ci.pct == 0) return new Vector2(0, 0);
			float pct;
			float mx2 = 0;
			float my2 = 0;
			if (ci.side == "l")
			{
				mx2 = ci.other.bb.l - bb.r;
				pct = mx2 / mx;
				my2 = pct * my;
			}
			if (ci.side == "r")
			{
				mx2 = ci.other.bb.r - bb.l;
				pct = mx2 / mx;
				my2 = pct * my;
			}
			if (ci.side == "u")
			{
				my2 = ci.other.bb.u - bb.d;
				pct = my2 / my;
				mx2 = pct * mx;
			}
			if (ci.side == "d")
			{
				my2 = ci.other.bb.d - bb.u;
				pct = my2 / my;
				mx2 = pct * mx;
			}

			if (ci.side == " " && ci.pct == 1)
			{
				mx2 = mx;
				my2 = my;
				x += mx2;
				y += my2;
			}
			else
			{
				x += mx2 * movetocidampen;
				y += my2 * movetocidampen;
			}
			return new Vector2(mx2, my2);
		}

		public bool noColls(List<obj> colls)
		{
			if(colls.Count==0) return true;
			if(colls.Count==1 && colls[0]==this) return true;
			return false;
		}

		//public Vector2 findClosestCollisionsPercent(float mx, float my, obj o)
		public CollisionInfo findClosestCollisionsPercent(float mx, float my, obj o)
		{
			//public float FindClosestCollisionsPercent(HyperBB, BBSolid)
			//{
			//	//HyperBB is BBi+BBf+pathLines
			//	1. Check each pathLine vs BBS, return closest collision's dist%
			//		a. pathLine collides with Rectangle: CASE RED
			//			i. Foreach BBline of Rectangle:
			//				A. Find collision point of pathLine and BBLine
			//				B. Get dist% along pathLine
			//			ii. Return dist% of min
			//		b. BBf collides with Rectangle: CASE BROWN
			//			i. Find where BB would be if adjacent to each side of BBS
			//				A. Along path, put HBBr=BBSl, HBBd=BBSu, etc
			//				B. Get dist% for these
			//				C. Return the one that has a common line segment and < 100%
			//		c. pathLine collides with Circle: CASE YELLOW (Not implemented)
			//		d. BBf collides with Circle: CASE ORANGE (Not implemented)
			//		e. pathLine collides with Line: CASE PURPLE (Not implemented)
			//		f. BBf collides with Line: CASE PINK (Not implemented)
			//		g. Something collides with Polyline: Special case of e or f (Not implemented)
			//}

			//First check if a boundary is coincident with another boundary, can't move that way then...
			//if (bb.l == o.bb.r && mx < 0) return new Vector2(0, 1);
			//if (bb.r == o.bb.l && mx > 0) return new Vector2(0, 1);
			//if (bb.u == o.bb.d && my < 0) return new Vector2(0, 0);
			//if (bb.d == o.bb.u && my > 0) return new Vector2(0, 0);
			if (bb.l == o.bb.r && mx < 0) return new CollisionInfo(true, o, "r", new Vector2(-1, -1), 0);
			if (bb.r == o.bb.l && mx > 0) return new CollisionInfo(true, o, "l", new Vector2(-1, -1), 0);
			if (bb.u == o.bb.d && my < 0) return new CollisionInfo(true, o, "d", new Vector2(-1, -1), 0);
			if (bb.d == o.bb.u && my > 0) return new CollisionInfo(true, o, "u", new Vector2(-1, -1), 0);

			List<obj> collsPathlines = new List<obj>();
			collsPathlines = collsPathlines.Union(level.collisionListAlongLine(bb.l, bb.u, bb.l + mx, bb.u + my, true)).ToList();
			collsPathlines = collsPathlines.Union(level.collisionListAlongLine(bb.l, bb.d, bb.l + mx, bb.d + my, true)).ToList();
			collsPathlines = collsPathlines.Union(level.collisionListAlongLine(bb.r, bb.u, bb.r + mx, bb.u + my, true)).ToList();
			collsPathlines = collsPathlines.Union(level.collisionListAlongLine(bb.r, bb.d, bb.r + mx, bb.d + my, true)).ToList();
			if(collsPathlines.Contains(o))
			{
				//Case Red:
				//float dist = 0;
				CollisionInfo cilu = findClosestCollisionsPercentLine(bb.l, bb.u, bb.l + mx, bb.u + my, o);
				CollisionInfo cild = findClosestCollisionsPercentLine(bb.l, bb.d, bb.l + mx, bb.d + my, o);
				CollisionInfo ciru = findClosestCollisionsPercentLine(bb.r, bb.u, bb.r + mx, bb.u + my, o);
				CollisionInfo cird = findClosestCollisionsPercentLine(bb.r, bb.d, bb.r + mx, bb.d + my, o);
				Vector2 vlu = new Vector2(cilu.pct, cilu.getSlope());
				Vector2 vld = new Vector2(cild.pct, cild.getSlope());
				Vector2 vru = new Vector2(ciru.pct, ciru.getSlope());
				Vector2 vrd = new Vector2(cird.pct, cird.getSlope());
				//Vector2 vlu = findClosestCollisionsPercentLine(bb.l, bb.u, bb.l + mx, bb.u + my, o);
				//Vector2 vld = findClosestCollisionsPercentLine(bb.l, bb.d, bb.l + mx, bb.d + my, o);
				//Vector2 vru = findClosestCollisionsPercentLine(bb.r, bb.u, bb.r + mx, bb.u + my, o);
				//Vector2 vrd = findClosestCollisionsPercentLine(bb.r, bb.d, bb.r + mx, bb.d + my, o);
				float distlu = vlu.X;
				float distld = vld.X;
				float distru = vru.X;
				float distrd = vrd.X;
				if (distlu > 1) distlu = -1;
				if (distld > 1) distld = -1;
				if (distru > 1) distru = -1;
				if (distrd > 1) distrd = -1;

				float index = smallestPositiveIndex(new float[] { distlu, distld, distru, distrd });
				
				//if (index >= 0)
				//	return new Vector2[] { vlu, vld, vru, vrd }[(int)index];
				//else
				//	return new Vector2(-1, -1);
				if (index >= 0)
					return new CollisionInfo[] { cilu, cild, ciru, cird }[(int)index];
				else return new CollisionInfo(false);
			}

			List<obj> collsBBf = level.collisionListAtRelative(this, mx, my, true);
			if (collsBBf.Contains(o))
			{
				//Case Brown:
				//if(BB.linesCoincide(bb.l, bb.u, bb.l, bb.d, o.bb.l, o.bb.u, o.bb.l, o.bb.d))
				float m = my / mx;
				float distl = (o.bb.r - bb.l) / mx;
				float distr = (o.bb.l - bb.r) / mx;
				float distu = (o.bb.d - bb.u) / my;
				float distd = (o.bb.u - bb.d) / my;
				//float distl = (bb.l - o.bb.r) * (float)Math.Sqrt(m * m + 1);
				//float distr = (o.bb.l - bb.r) * (float)Math.Sqrt(m * m + 1); float distrp = distr / mx;

				//Make each noncoincident line <0, make each >1 <0
				if (distl > 1 || !linesCoincideBrown(bb.u, bb.d, bb.l, o.bb.r, m, true, o.bb.r, o.bb.u, o.bb.r, o.bb.d)) distl = -1;
				if (distr > 1 || !linesCoincideBrown(bb.u, bb.d, bb.r, o.bb.l, m, true, o.bb.l, o.bb.u, o.bb.l, o.bb.d)) distr = -1;
				if (distu > 1 || !linesCoincideBrown(bb.l, bb.r, bb.u, o.bb.d, m, false, o.bb.l, o.bb.d, o.bb.r, o.bb.d)) distu = -1;
				if (distd > 1 || !linesCoincideBrown(bb.l, bb.r, bb.d, o.bb.u, m, false, o.bb.l, o.bb.u, o.bb.r, o.bb.u)) distd = -1;

				//Find smallest positive
				float index = smallestPositiveIndex(new float[] { distd, distu, distr, distl });
				//if (index == 0) return new Vector2(distd, 0);
				//if (index == 1) return new Vector2(distu, 0);
				//if (index == 2) return new Vector2(distr, 1);
				//if (index == 3) return new Vector2(distl, 1);
				//if (index == -1) return new Vector2(-1, -1);
				//return new Vector2(-1, -1);

				if (index == 0) return new CollisionInfo(true, o, "u", new Vector2(-1, -1), distd);
				if (index == 1) return new CollisionInfo(true, o, "d", new Vector2(-1, -1), distu);
				if (index == 2) return new CollisionInfo(true, o, "l", new Vector2(-1, -1), distr);
				if (index == 3) return new CollisionInfo(true, o, "r", new Vector2(-1, -1), distl);
				if (index == -1) return new CollisionInfo(false);
				return new CollisionInfo(false);
			}

			//Unimplemented? Else it's a bug :/
			//For now assume no collision
			return new CollisionInfo(false, o, " ", new Vector2(-1, -1), 1);

			//return new Vector2(-1, -1);
			return new CollisionInfo(false);
		}

		public CollisionInfo findClosestCollisionsPercentLine(float x1, float y1, float x2, float y2, obj o)
		//public Vector2 findClosestCollisionsPercentLine(float x1, float y1, float x2, float y2, obj o)
		{
			Vector2 collPointl = BB.lineCollisionPoint(x1, y1, x2, y2, o.bb.l, o.bb.u, o.bb.l, o.bb.d);
			Vector2 collPointr = BB.lineCollisionPoint(x1, y1, x2, y2, o.bb.r, o.bb.u, o.bb.r, o.bb.d);
			Vector2 collPointu = BB.lineCollisionPoint(x1, y1, x2, y2, o.bb.l, o.bb.u, o.bb.r, o.bb.u);
			Vector2 collPointd = BB.lineCollisionPoint(x1, y1, x2, y2, o.bb.l, o.bb.d, o.bb.r, o.bb.d);
			float distl = BB.dist(x1, y1, collPointl.X, collPointl.Y) / BB.dist(x1, y1, x2, y2);
			float distr = BB.dist(x1, y1, collPointr.X, collPointr.Y) / BB.dist(x1, y1, x2, y2);
			float distu = BB.dist(x1, y1, collPointu.X, collPointu.Y) / BB.dist(x1, y1, x2, y2);
			float distd = BB.dist(x1, y1, collPointd.X, collPointd.Y) / BB.dist(x1, y1, x2, y2);
			if (collPointl.X == -1 || distl > 1) distl = -1;
			if (collPointr.X == -1 || distr > 1) distr = -1;
			if (collPointu.X == -1 || distu > 1) distu = -1;
			if (collPointd.X == -1 || distd > 1) distd = -1;

			float index = smallestPositiveIndex(new float[] { distd, distu, distr, distl });
			//if (index == 0) return new Vector2(distd, 0);
			//if (index == 1) return new Vector2(distu, 0);
			//if (index == 2) return new Vector2(distr, 1);
			//if (index == 3) return new Vector2(distl, 1);
			//if (index == -1) return new Vector2(-1, -1);
			if (index == 0) return new CollisionInfo(true, o, "d", collPointd, distd);
			if (index == 1) return new CollisionInfo(true, o, "u", collPointu, distu);
			if (index == 2) return new CollisionInfo(true, o, "r", collPointr, distr);
			if (index == 3) return new CollisionInfo(true, o, "l", collPointl, distl);
			if (index == -1) return new CollisionInfo(false, o, "", new Vector2(-1, -1), -1);
			//return dist;
			//Error?
			//return new Vector2();
			return new CollisionInfo(false);
		}

		public bool linesCoincideBrown(float p1, float p2, float from, float to, float m, bool givenXFromTo, float bx1, float by1, float bx2, float by2)
		{
			//y-y1=m(x-x1)
			//(y-y1)/m+x1=x
			//y=m(x-x1)+y1
			if(givenXFromTo)
			{
				float ax1 = to;
				float ay1 = m * (to - from) + p1;
				float ax2 = to;
				float ay2 = m * (to - from) + p2;
				return linesCoincideHelped(ax1, ay1, ax2, ay2, bx1, by1, bx2, by2, m);
			}
			else
			{
				float ax1 = p1 + (to - from) / m;
				float ay1 = to;
				float ax2 = p2 + (to - from) / m;
				float ay2 = to;
				return linesCoincideHelped(ax1, ay1, ax2, ay2, bx1, by1, bx2, by2, m);
			}
		}

		public bool linesCoincideHelped(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2, float m)
		{
			if (m == 0 && ay1 == by1)
			{
				if (BB.isBetween(ax1, bx1, bx2) || BB.isBetween(ax2, bx1, bx2) || BB.isBetween(bx1, ax1, ax2) || BB.isBetween(bx2, ax1, ax2))
					return true;
			}
			if (float.IsInfinity(m) && ax1 == bx1)
			{
				if (BB.isBetween(ay1, by1, by2) || BB.isBetween(ay2, by1, by2) || BB.isBetween(by1, ay1, ay2) || BB.isBetween(by2, ay1, ay2))
					return true;
			}
			if ((ax1 == bx1 && ay1 == by1)
				|| (ax1 == bx2 && ay1 == by2)
				|| (ax2 == bx1 && ay2 == by1)
				|| (ax2 == bx2 && ay2 == by2))
				return true;
			return false;
		}

		public float smallestPositive(float[] lst)
		{
			float smallest = -1;
			foreach (float f in lst)
			{
				if (f <= 0) continue;
				bool failed = false;
				foreach (float f2 in lst)
				{
					if (f > f2 && f2 > 0)
					{
						failed = true;
						break;
					}
				}
				if (!failed)
					smallest = f;
			}

			return smallest;
		}

		public float smallestPositiveIndex(float[] lst)
		{
			float smallest = -1;
			float smallesti = -1;
			for (int i = 0; i < lst.Length; i++)
			{
				float f = lst[i];
				if (f <= 0) continue;
				bool failed = false;
				foreach (float f2 in lst)
				{
					if (f > f2 && f2 > 0)
					{
						failed = true;
						break;
					}
				}
				if (!failed)
				{
					smallest = f;
					smallesti = i;
				}
			}

			return smallesti;
		}

		protected override void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			getPaintColor();
			drawSprite(spriteBatch, c, mainSprite, drawP, paintedColor, 1);
		}

		protected override void getPaintColor()
		{
			float tintAmt = .5f;
			if ((factor < 1) && (factor > 0))				//Blue
				paintedColor = tintColorWhite(87, 242, 242, 255, tintAmt);
			else if (factor > 1)							//Orange
				paintedColor = tintColorWhite(255, 201, 14, 255, tintAmt);
			else if (gravFactor > 1)						//Purple
				paintedColor = tintColorWhite(210, 91, 255, 255, tintAmt);
			else if ((gravFactor < 1) && (gravFactor > 0))	//Light Green
				paintedColor = tintColorWhite(128, 255, 0, 255, tintAmt);
			else if (factor < 0)							//Red
				paintedColor = tintColorWhite(182, 14, 22, 255, tintAmt);
			else if (gravFactor < 0)						//Green
				paintedColor = tintColorWhite(0, 125, 0, 255, tintAmt);
			else
				paintedColor = Color.White;
		}

		protected Color tintColorWhite(int r, int g, int b, int a, float amt)
		{
			r += (int)((256 - r) * amt);
			g += (int)((256 - g) * amt);
			b += (int)((256 - b) * amt);
			return new Color(r, g, b, a);
		}
	}
}
