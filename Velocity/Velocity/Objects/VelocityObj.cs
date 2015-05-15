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
			Move(xspeed * factor, yspeed * factor, true);

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
		}

		protected static float makeDistSafe(float dist)
		{
			float safeDist = .01f;
			float ans = dist - safeDist * obj.Sign(dist);
			if (obj.Sign(ans) != obj.Sign(dist))
				ans = 0;
			return ans;
		}

		public override Vector2 Move(float mx, float my, bool actuallyMove)
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
			if (colls.Count != 0)
			{
				//Pre colliding, GLITCH!!!
				int asdf = 0;
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
			return new Vector2(movedx, movedy);
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
