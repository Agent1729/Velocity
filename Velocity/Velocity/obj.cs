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

namespace Velocity
{
	public class obj
	{
		public Level level;

		public Texture2D mainSprite;
		protected Texture2D blackPixel;
		public string mainSpriteName;
		protected Vector2 XY;
		protected Vector2 WH;
		public float width, height;
		public float mass = 1;
		public float pushForce = 1;
		public float fmaPushMod = .25f;
		public float inertia = 1;
		protected Vector2 speed;
		public BB bb;
		public string objType;
		public float depth;
		public bool collisionStatic;
		public bool isSolid;
		public bool takesControls;
		public bool canAbsorb;
		public bool hasBeenFrictioned;
		public bool hasMoved;
		public bool isMoving;
		public bool destroyed;
		public List<Region> myRegions;
		public List<obj> collidThisTick;

		public bool canBePushed;
		public int ticksSincePushed = -1;
		public List<obj> objsPushed;
		public List<obj> objsPushedLast;

		public float base_width, base_height;

		public bool lmoving, rmoving, umoving, dmoving;

		public obj(float _x, float _y)
		{
			XY.X = _x; XY.Y = _y; speed.X = 0; speed.Y = 0;
			lmoving = false; rmoving = false; umoving = false; dmoving = false;
			isMoving = false;
			destroyed = false;
			myRegions = new List<Region>();
			collidThisTick = new List<obj>();
			objsPushed = new List<obj>();
			objsPushedLast = new List<obj>();
			init();
			bb = new BB(_x, _y, width, height);
			WH.X = width; WH.Y = height;
		}

		protected virtual void init()
		{
			/*
			objType = "";
			spriteName = "";
			base_width = 0; base_height = 0;
			width = base_width; height = base_height;
			depth = 0;
			collisionStatic = true;
			isSolid = true;
			takesControls = false;
			canBePushed = false;*/
			//setRegions();
		}

		protected virtual void destroy()
		{
			destroyed = true;
			level.removeNewObj(this);
			setRegions();
			foreach (Region r in myRegions)
				if(r.objs.Contains(this))
					r.remove(this);
			myRegions.Clear();
		}

		public void loadTexture() { doLoadTexture(); }
		protected virtual void doLoadTexture()
		{
			mainSprite = SpriteManager.getSprite(mainSpriteName);
			blackPixel = SpriteManager.getSprite("BlackPixel");
		}

		public float x
		{
			get { return XY.X; }
			set
			{
				XY.X = value;
				bb.x = value;
			}
		}
		public float y
		{
			get { return XY.Y; }
			set
			{
				XY.Y = value;
				bb.y = value;
			}
		}
		public Vector2 p
		{
			get { return XY; }
		}
		public Vector2 drawP
		{
			get { return XY - WH; }
		}

		public float xspeed
		{
			get { return speed.X; }
			set { speed.X = value; }
		}
		public float yspeed
		{
			get { return speed.Y; }
			set { speed.Y = value; }
		}
		public Vector2 s
		{
			get { return speed; }
		}

		public void beginTick() { doBeginTick(); }
		protected virtual void doBeginTick()
		{
			hasMoved = false;
			if (ticksSincePushed >= 0) ticksSincePushed++;
			if (ticksSincePushed >= 2) ticksSincePushed = -1;
		}

		public void tick() { dotick(); }
		protected virtual void dotick()
		{
			//x += xspeed;
			//y += yspeed;
			if (!hasMoved)
			{
				if (xspeed != 0 || yspeed != 0)
					Move(xspeed, yspeed, true);
			}
		}

		public void endTick() { doEndTick(); }
		protected virtual void doEndTick()
		{
			collidThisTick.Clear();
			objsPushedLast = objsPushed;
			objsPushed = new List<obj>();
		}

		public void verifyCoords() { doVerifyCoords(); }
		protected virtual void doVerifyCoords() { }

		public virtual bool isGrounded() { return false; }

		public virtual Vector2 MoveOld(float mx, float my, bool actuallyMove)
		{
			float ix = x, iy = y;
			List<obj> colls;
			List<obj> colls2 = new List<obj>();
			List<obj> colls3 = new List<obj>();
			float dx = 9999, px = 9999;
			float dy = 9999, py = 9999;
			float movedx = mx, movedy = my;

			x += mx;
			colls = level.collisionList(this, true);
			if (colls.Count != 0)
			{
				x -= mx;
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
					if (mx < dx)
						movedx = mx;
					else
						movedx = dx;
				}
				if (mx < 0)
				{
					if (Math.Abs(mx) < dx)
						movedx = mx;
					else
						movedx = -dx;
				}
				x += movedx;

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
				foreach (obj o in colls3)
					o.Move(px, 0, true);
				if (px != 9999)
					{ movedx += px; x += px; }

				//Collide with them
				foreach (obj o in colls2)
				{
					//Collide it
				}
			}
				
			colls2.Clear();
			colls3.Clear();
			y += my;
			colls = level.collisionList(this, true);
			if (colls.Count != 0)
			{
				y -= my;
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
					if (my < dy)
						movedy = my;
					else
						movedy = dy;
				}
				if (my < 0)
				{
					if (Math.Abs(my) < dy)
						movedy = my;
					else
						movedy = -dy;
				}
				y += movedy;

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
				foreach (obj o in colls3)
					o.Move(0, py, true);
				if (py != 9999)
				{ movedy += py; y += py; }

				//Collide with them
				foreach (obj o in colls2)
				{
					//Collide it
				}
			}

			if (!actuallyMove)
				{ x = ix; y = iy; }
			if (actuallyMove)
			{
				if ((mx < 0) && (movedx > mx))
				{
					if (xspeed < 0)
						xspeed = 0;
					lmoving = false;
				}
				if ((mx > 0) && (movedx < mx))
				{
					if (xspeed > 0)
						xspeed = 0;
					rmoving = false;
				}
				if ((my < 0) && (movedy > my))
				{
					if (yspeed < 0)
						yspeed = 0;
					umoving = false;
				}
				if ((my > 0) && (movedy < my))
				{
					if (yspeed > 0)
						yspeed = 0;
					dmoving = false;
					isGrounded();
				}
			}

			setRegions();
			return new Vector2(movedx, movedy);
		}

		public virtual Vector2 Move(float mx, float my, bool setHasMovedTrue)
		{
			return new Vector2();
		}

		public virtual Vector2 Move(float mx, float my, bool setHasMovedTrue, int layers, int movedLayers)
		{
			return new Vector2();
		}

		public virtual Vector2 Move2(float mx, float my, bool setHasMovedTrue)
		{
			return new Vector2();
		}

		public virtual Vector2 MoveHelped(float mx, float my, bool setHasMovedTrue, int layers, int movedLayers)
		{
			return new Vector2();
		}

		#region Controls
		public void leftDown(object lvl) { doleftDown(lvl); } protected virtual void doleftDown(object lvl) { }
		public void leftPressed(object lvl) { doleftPressed(lvl); } protected virtual void doleftPressed(object lvl) { }
		public void leftReleased(object lvl) { doleftReleased(lvl); } protected virtual void doleftReleased(object lvl) { }
		public void rightDown(object lvl) { dorightDown(lvl); } protected virtual void dorightDown(object lvl) { }
		public void rightPressed(object lvl) { dorightPressed(lvl); } protected virtual void dorightPressed(object lvl) { }
		public void rightReleased(object lvl) { dorightReleased(lvl); } protected virtual void dorightReleased(object lvl) { }
		public void upDown(object lvl) { doupDown(lvl); } protected virtual void doupDown(object lvl) { }
		public void upPressed(object lvl) { doupPressed(lvl); } protected virtual void doupPressed(object lvl) { }
		public void upReleased(object lvl) { doupReleased(lvl); } protected virtual void doupReleased(object lvl) { }
		public void downDown(object lvl) { dodownDown(lvl); } protected virtual void dodownDown(object lvl) { }
		public void downPressed(object lvl) { dodownPressed(lvl); } protected virtual void dodownPressed(object lvl) { }
		public void downReleased(object lvl) { dodownReleased(lvl); } protected virtual void dodownReleased(object lvl) { }

		public void shiftDown(object lvl) { doshiftDown(lvl); } protected virtual void doshiftDown(object lvl) { }
		public void shiftPressed(object lvl) { doshiftPressed(lvl); } protected virtual void doshiftPressed(object lvl) { }
		public void shiftReleased(object lvl) { doshiftReleased(lvl); } protected virtual void doshiftReleased(object lvl) { }
		public void clearDown(object lvl) { doclearDown(lvl); } protected virtual void doclearDown(object lvl) { }
		public void clearPressed(object lvl) { doclearPressed(lvl); } protected virtual void doclearPressed(object lvl) { }
		public void clearReleased(object lvl) { doclearReleased(lvl); } protected virtual void doclearReleased(object lvl) { }

		public void n1Down(object lvl) { don1Down(lvl); } protected virtual void don1Down(object lvl) { }
		public void n1Pressed(object lvl) { don1Pressed(lvl); } protected virtual void don1Pressed(object lvl) { }
		public void n1Released(object lvl) { don1Released(lvl); } protected virtual void don1Released(object lvl) { }
		public void n2Down(object lvl) { don2Down(lvl); } protected virtual void don2Down(object lvl) { }
		public void n2Pressed(object lvl) { don2Pressed(lvl); } protected virtual void don2Pressed(object lvl) { }
		public void n2Released(object lvl) { don2Released(lvl); } protected virtual void don2Released(object lvl) { }
		public void n3Down(object lvl) { don3Down(lvl); } protected virtual void don3Down(object lvl) { }
		public void n3Pressed(object lvl) { don3Pressed(lvl); } protected virtual void don3Pressed(object lvl) { }
		public void n3Released(object lvl) { don3Released(lvl); } protected virtual void don3Released(object lvl) { }
		public void n4Down(object lvl) { don4Down(lvl); } protected virtual void don4Down(object lvl) { }
		public void n4Pressed(object lvl) { don4Pressed(lvl); } protected virtual void don4Pressed(object lvl) { }
		public void n4Released(object lvl) { don4Released(lvl); } protected virtual void don4Released(object lvl) { }
		public void n5Down(object lvl) { don5Down(lvl); } protected virtual void don5Down(object lvl) { }
		public void n5Pressed(object lvl) { don5Pressed(lvl); } protected virtual void don5Pressed(object lvl) { }
		public void n5Released(object lvl) { don5Released(lvl); } protected virtual void don5Released(object lvl) { }

		public void lmDown(object lvl) { dolmDown(lvl); } protected virtual void dolmDown(object lvl) { }
		public void lmPressed(object lvl) { dolmPressed(lvl); } protected virtual void dolmPressed(object lvl) { }
		public void lmReleased(object lvl) { dolmReleased(lvl); } protected virtual void dolmReleased(object lvl) { }
		public void rmDown(object lvl) { dormDown(lvl); } protected virtual void dormDown(object lvl) { }
		public void rmPressed(object lvl) { dormPressed(lvl); } protected virtual void dormPressed(object lvl) { }
		public void rmReleased(object lvl) { dormReleased(lvl); } protected virtual void dormReleased(object lvl) { }
		#endregion

		public void Collision(obj other, bool isPrimary)
		{
			if (collidThisTick.Contains(other))
				return;
			collidThisTick.Add(other);
			doCollision(other, isPrimary);
		}
		protected virtual void doCollision(obj other, bool isPrimary) { }

		public void setRegions()
		{
			if (destroyed) return;
			foreach (Region r in myRegions)
				r.remove(this);
			List<Region> rgns = getRegions();
			foreach (Region r in rgns)
				r.add(this);
			myRegions = rgns;
		}

		public List<Region> getRegions()
		{
			List<Region> regions = new List<Region>();
			int rsize = level.regionSize;
			int rnumx = level.regionsX;
			int rnumy = level.regionsY;
			Region r = null;

			Vector4 v = getRegionNumbersOf(bb.l, bb.u, bb.r, bb.d, rsize, rnumx, rnumy);
			if (v.X == -1 || v.Y == -1 || v.Z == -1 || v.W == -1) return regions;

			for (int i = (int)v.X; i <= (int)v.Z; i++)
				for (int j = (int)v.Y; j <= (int)v.W; j++)
				{
					r = level.regions[j * rnumx + i];
					regions.Add(r);
				}

			//For each corner, get its region. If the region is not in regions, add it
			/*r = getRegionOf(bb.l, bb.u, rsize, rnum);
			if (r != null && !regions.Contains(r))
				regions.Add(r);
			r = getRegionOf(bb.r, bb.u, rsize, rnum);
			if (r != null && !regions.Contains(r))
				regions.Add(r);
			r = getRegionOf(bb.r, bb.d, rsize, rnum);
			if (r != null && !regions.Contains(r))
				regions.Add(r);
			r = getRegionOf(bb.l, bb.d, rsize, rnum);
			if (r != null && !regions.Contains(r))
				regions.Add(r);//*/

			return regions;
		}

		private Vector4 getRegionNumbersOf(float ax, float ay, float bx, float by, int _rsize, int _rnumx, int _rnumy)
		{
			int x1, y1, x2, y2;
			x1 = (int)Math.Floor((double)ax / _rsize);
			y1 = (int)Math.Floor((double)ay / _rsize);
			x2 = (int)Math.Floor((double)bx / _rsize);
			y2 = (int)Math.Floor((double)by / _rsize);
			if (x1 < 0) x1 = 0;
			if (x1 >= _rnumx) return new Vector4(-1);
			if (y1 < 0) y1 = 0;
			if (y1 >= _rnumy) return new Vector4(-1);
			if (x2 < 0) return new Vector4(-1);
			if (x2 >= _rnumx) x2 = _rnumx - 1;
			if (y2 < 0) return new Vector4(-1);
			if (y2 >= _rnumy) y2 = _rnumy - 1;
			return new Vector4(x1, y1, x2, y2);
		}

		private Region getRegionOf(float _x, float _y, int _rsize, int _rnum)
		{
			int xr, yr;
			xr = (int)Math.Floor((double)_x / _rsize);
			yr = (int)Math.Floor((double)_y / _rsize);
			if ((xr >= 0) && (xr < _rnum) && (yr >= 0) && (yr < _rnum))
				return level.regions[yr * _rnum + xr];
			return null;
		}

		//protected virtual bool moveToOther(obj b, float axspeed, float ayspeed)
		//{
		//    float xsd, ysd;
		//    float i;
		//    float inc1 = .1f;

		//    xsd = xspeed - b.xspeed; ysd = yspeed - b.yspeed;
		//    x -= xspeed; y -= yspeed;
		//    for (i = 0f; i < 1f; i += inc1)
		//    {
		//        x += inc1 * xspeed;
		//        y += inc1 * yspeed;
		//        if (level.collidesSolid(this))
		//            break;
		//    }

		//    x -= inc1 * xspeed;
		//    y -= inc1 * yspeed;
		//    i -= inc1;

		//    char xy = 'x';
		//    x += inc1 * xspeed;
		//    if (level.collidesSolid(this))
		//    {
		//        xy = 'y';
		//        x -= inc1 * xspeed;
		//        xspeed = 0; lmoving = false; rmoving = false;
		//    }
		//    else
		//    {
		//        x -= inc1 * xspeed;
		//        yspeed = 0; umoving = false; dmoving = false;
		//    }
		//    x -= inc1 * xspeed;
		//    for (float r = i * inc1; r < 1f; r += inc1)
		//    {
		//        if (xy == 'x')
		//            x += inc1 * xspeed;
		//        else if (xy == 'y')
		//            y += inc1 * yspeed;
		//        if (level.collidesSolid(this))
		//        {
		//            if (xy == 'x')
		//            {
		//                x -= inc1 * xspeed;
		//                xspeed = 0; lmoving = false; rmoving = false;
		//            }
		//            else if (xy == 'y')
		//            {
		//                y -= inc1 * yspeed;
		//                yspeed = 0; umoving = false; dmoving = false;
		//            }
		//            break;
		//        }
		//    }

		//    return true;
		//}

		//protected virtual bool getPushedByOther(obj b, float axspeed, float ayspeed)
		//{
		//    return true;
		//}

		//protected virtual bool moveFromOthers() { return true; }

		public void capSpeed(float max)
		{
			if (xspeed > max)
				xspeed = max;
			if (xspeed < -max)
				xspeed = -max;
			if (yspeed > max)
				yspeed = max;
			if (yspeed < -max)
				yspeed = -max;
		}

		public void draw(SpriteBatch s, Camera c) { doDraw(s, c); }

		protected virtual void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			drawSprite(spriteBatch, c, mainSprite, drawP);
		}

		#region drawSprite
		protected void drawSprite(SpriteBatch spriteBatch, Camera c, Texture2D _sprite, Vector2 position, Color color, float scale)
		{
			//spriteBatch.Draw(sprite, position - c.XY, null, color, 0, new Vector2(0, 0), scale * c.zoom, SpriteEffects.None, 0);
			spriteBatch.Draw(_sprite, (position - c.XY) / c.zoom, null, color, 0, new Vector2(0, 0), scale / c.zoom, SpriteEffects.None, 0);
		}

		protected void drawSprite(SpriteBatch spriteBatch, Camera c, Texture2D _sprite, Vector2 position)
		{
			spriteBatch.Draw(_sprite, (position - c.XY) / c.zoom, null, Color.White, 0, new Vector2(0, 0), 1f/c.zoom, SpriteEffects.None, 0);
		}

		protected void drawSpriteNoZoom(SpriteBatch spriteBatch, Camera c, Texture2D _sprite, Vector2 position)
		{
			spriteBatch.Draw(_sprite, (position - c.XY) / c.zoom, null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
		}

		protected void drawSpriteNoZoomOnGui(SpriteBatch spriteBatch, Camera c, Texture2D _sprite, Vector2 position)
		{
			spriteBatch.Draw(_sprite, position, null, Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
		}

		protected void drawLine(SpriteBatch spriteBatch, Camera c, Texture2D pxl, Vector2 p1, Vector2 p2)
		{
			float ang, dist;
			dist = BB.dist(p1.X, p1.Y, p2.X, p2.Y);
			ang = (float)Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));
			if (p2.X < p1.X)
				ang += (float)Math.PI;

			//spriteBatch.Draw(pxl, new Rectangle((int)p1.X, (int)p1.Y, (int)dist, 1), null, Color.Black,
			//	(float)(ang * Math.PI / 4), new Vector2(0f, 0f), SpriteEffects.None, 1f);
			//spriteBatch.Draw(pxl, (p1 - c.XY) / c.zoom, null, Color.Black,
			//	(float)(2 * Math.PI / 4), new Vector2(0f, 0f), new Vector2((int)dist, 1), SpriteEffects.None, 1f);
			spriteBatch.Draw(pxl, (p1 - c.XY) / c.zoom, null, Color.Black,
				ang, new Vector2(0f, 0f), new Vector2((int)dist/c.zoom, 1), SpriteEffects.None, 1f);
		}

		protected void drawText(SpriteBatch spriteBatch, Camera c, SpriteFont sf, string s, Vector2 position, Color color)
		{
			spriteBatch.DrawString(sf, s, (position - c.XY) / c.zoom, color, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
		}

		protected void drawTextOnGui(SpriteBatch spriteBatch, Camera c, SpriteFont sf, string s, Vector2 position, Color color)
		{
			spriteBatch.DrawString(sf, s, position, color, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
		}

		protected virtual void getPaintColor() { }
		#endregion

		public static float Sign(float n)
		{
			if (n < 0)
				return -1;
			else if (n == 0)
				return 0;
			else
				return 1;
		}

		public static float calcAng(float xd, float yd)
		{
			float ang = (float)Math.Atan(yd / xd);
			if (xd < 0)
				ang += (float)Math.PI;
			return ang;
		}
	}

	public struct BB
	{
		public float l, r, u, d;
		private float width, height;
		private float X, Y;
		public float rad;
		public string type;

		private float x1, y1, x2, y2;

		public BB(float _x, float _y, float _width, float _height)
		{
			X = _x; Y = _y; width = _width; height = _height;
			l = X - width; r = X + width;
			u = Y - height; d = Y + height;
			rad = BB.dist(X, Y, l, u);
			type = "square";

			x1 = 0; y1 = 0; x2 = 0; y2 = 0;
		}
		public BB(float _x, float _y, float _radius)
		{
			X = _x; Y = _y; width = _radius; height = _radius;
			l = X - width; r = X + width;
			u = Y - height; d = Y + height;
			//rad = BB.dist(X, Y, l, u);
			rad = _radius;
			type = "circle";

			x1 = 0; y1 = 0; x2 = 0; y2 = 0;
		}
		public BB(float _x1, float _y1, float _x2, float _y2, string _line)
		{
			X = (_x1 + _x2) / 2; Y = (_y1 + _y2) / 2;
			width = Math.Abs(_x1 - _x2) / 2; height = Math.Abs(_y1 - _y2) / 2;
			l = X - width; r = X + width;
			u = Y - height; d = Y + height;
			rad = BB.dist(X, Y, l, u);
			type = "line";

			x1 = _x1; y1 = _y1; x2 = _x2; y2 = _y2;
		}

		public float x
		{
			get { return X; }
			set
			{
				X = value;
				l = x - width; r = x + width;
			}
		}
		public float y
		{
			get { return Y; }
			set
			{
				Y = value;
				u = y - height; d = y + height;
			}
		}

		public static BB operator +(BB bb, Vector2 v)
		{
			return new BB(bb.x + v.X, bb.y + v.Y, bb.width, bb.height);
		}

		public static float dist(float x1, float y1, float x2, float y2)
		{ return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)); }

		public bool pointCollide(float _x, float _y)
		{
			/*	//With equals
			if (type == "square")
				return ((_x >= l) && (_x <= r) && (_y <= d) && (_y >= u));
			else if (type == "circle")
				return (BB.dist(_x, _y, x, y) <= rad);
			return false;//*/
			//*	//No equals
			if (type == "square")
				return ((_x > l) && (_x < r) && (_y < d) && (_y > u));
			else if (type == "circle")
				return (BB.dist(_x, _y, x, y) <= rad);
			return false;//*/
		}

		public static bool collides(BB a, BB b)
		{
			//If too far away, no
			if (BB.dist(a.x, a.y, b.x, b.y) > (a.rad + b.rad))
				if ((a.type != "line") && (b.type != "line"))
					return false;

			//Select collision checking type
			if ((a.type == "square") && (b.type == "square"))
				return collidesSquares(a, b);
			if ((a.type == "square") && (b.type == "circle"))
				return collidesSquareCircle(a, b);
			if ((a.type == "circle") && (b.type == "square"))
				return collidesSquareCircle(b, a);
			if ((a.type == "circle") && (b.type == "circle"))
				return collidesCircles(a, b);
			if ((a.type == "square") && (b.type == "line"))
				return collidesSquareLine(a, b);
			if ((a.type == "line") && (b.type == "square"))
				return collidesSquareLine(b, a);

			return false;	//Shouldn't get here
		}

		private static bool collidesSquares(BB a, BB b)
		{
			//If a corner of b is in a
			if (a.pointCollide(b.l, b.u))
				return true;
			if (a.pointCollide(b.r, b.u))
				return true;
			if (a.pointCollide(b.l, b.d))
				return true;
			if (a.pointCollide(b.r, b.d))
				return true;

			//If center of b is in a
			if (a.pointCollide(b.x, b.y))
				return true;

			//If an edge point of b is in a
			if (a.pointCollide(b.l, b.y))
				return true;
			if (a.pointCollide(b.r, b.y))
				return true;
			if (a.pointCollide(b.x, b.u))
				return true;
			if (a.pointCollide(b.x, b.d))
				return true;

			//If a corner of a is in b
			if (b.pointCollide(a.l, a.u))
				return true;
			if (b.pointCollide(a.r, a.u))
				return true;
			if (b.pointCollide(a.l, a.d))
				return true;
			if (b.pointCollide(a.r, a.d))
				return true;

			//If both are long, and perpendicularly form a cross
			/*	//With equals
			if ((a.l <= b.l) && (a.r >= b.r) && (a.u >= b.u) && (a.d <= b.d))
				return true;
			if ((a.l >= b.l) && (a.r <= b.r) && (a.u <= b.u) && (a.d >= b.d))
				return true;//*/
			//*	//No equals
			if ((a.l < b.l) && (a.r > b.r) && (a.u > b.u) && (a.d < b.d))
				return true;
			if ((a.l > b.l) && (a.r < b.r) && (a.u < b.u) && (a.d > b.d))
				return true;//*/

			return false;
		}

		private static bool collidesSquareCircle(BB a, BB b)
		{
			//If a corner is in the circle
			if (BB.dist(a.l, a.u, b.x, b.y) <= b.rad)
				return true;
			if (BB.dist(a.l, a.d, b.x, b.y) <= b.rad)
				return true;
			if (BB.dist(a.r, a.u, b.x, b.y) <= b.rad)
				return true;
			if (BB.dist(a.r, a.d, b.x, b.y) <= b.rad)
				return true;
			//If the center is in the circle
			if (BB.dist(a.x, a.y, b.x, b.y) <= b.rad)
				return true;

			//If the circle's center is inside the square
			if (a.pointCollide(b.x, b.y))
				return true;

			//If a line of a's BB collides the circle
			//Assuming xy aligned BB
			if ((a.l <= b.x + b.rad) && (a.l >= b.x - b.rad))
				if ((a.d >= b.y) && (a.u <= b.y))
					return true;
			if ((a.r <= b.x + b.rad) && (a.r >= b.x - b.rad))
				if ((a.d >= b.y) && (a.u <= b.y))
					return true;
			if ((a.u <= b.y + b.rad) && (a.u >= b.y - b.rad))
				if ((a.r >= b.x) && (a.l <= b.x))
					return true;
			if ((a.d <= b.y + b.rad) && (a.d >= b.y - b.rad))
				if ((a.r >= b.x) && (a.l <= b.x))
					return true;

			return false;
		}

		private static bool collidesCircles(BB a, BB b)
		{
			if (BB.dist(a.x, a.y, b.x, b.y) <= (a.rad + b.rad))
				return true;

			return false;
		}

		private static bool collidesSquareLine(BB a, BB b)
		{
			//If a point is inside the square
			if (a.pointCollide(b.x1, b.y1))
				return true;
			if (a.pointCollide(b.x2, b.y2))
				return true;

			//If bb boundaries collide the line
			if (linesCollide(b.x1, b.y1, b.x2, b.y2, a.l, a.d, a.l, a.u))	//l
				return true;
			if (linesCollide(b.x1, b.y1, b.x2, b.y2, a.r, a.d, a.r, a.u))	//r
				return true;
			if (linesCollide(b.x1, b.y1, b.x2, b.y2, a.l, a.u, a.r, a.u))	//u
				return true;
			if (linesCollide(b.x1, b.y1, b.x2, b.y2, a.l, a.d, a.r, a.d))	//d
				return true;

			return false;
		}

		public static bool linesCollide(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2)
		{
			//float denominator = ((ax2 - ax1) * (by2 - by1)) - ((ay2 - ay1) * (bx2 - bx1));
			//float numerator1 = ((ay1 - by1) * (bx2 - bx1)) - ((ax1 - bx1) * (by2 - by1));
			//float numerator2 = ((ay1 - by1) * (ax2 - ax1)) - ((ax1 - bx1) * (ay2 - ay1));

			//// Detect coincident lines (has a problem, read below)
			//// (Doesn't work if lines are coincident but don't overlap?)
			//if (denominator == 0)
			//	return ((numerator1 == 0) && (numerator2 == 0));

			//float r = numerator1 / denominator;
			//float s = numerator2 / denominator;

			//return ((r >= 0 && r <= 1) && (s >= 0 && s <= 1));



			float slopea = (ay2 - ay1) / (ax2 - ax1);
			float slopeb = (by2 - by1) / (bx2 - bx1);
			//Discount coincident lines or points, only counting hard intersections at a single point
			if (slopea == slopeb)
			{
				return false;
				//Exception? Let's count horizontal and vertical coincident lines, as long as they're not just having one point in common
				if(slopea==0&&ay1==by1)
				{
					if (isBetween(ax1, bx1, bx2) || isBetween(ax2, bx1, bx2) || isBetween(bx1, ax1, ax2) || isBetween(bx2, ax1, ax2))
						return true;
				}
				if(float.IsInfinity(slopea)&&ax1==bx1)
				{
					if (isBetween(ay1, by1, by2) || isBetween(ay2, by1, by2) || isBetween(by1, ay1, ay2) || isBetween(by2, ay1, ay2))
						return true;
				}
				return false;
			}
			if ((ax1 == bx1 && ay1 == by1) ||
				(ax1 == bx2 && ay1 == by2) ||
				(ax2 == bx1 && ay2 == by1) ||
				(ax2 == bx2 && ay2 == by2))
				return false;
			double xintersection = -999999;
			double yintersection = -999999;
			if(float.IsInfinity(slopea))
			{
				xintersection = (double)ax1;
				yintersection = (double)slopeb * ((double)xintersection - (double)bx1) + (double)by1;
				if(slopeb==0)
					return isBetween((double)by1, (double)ay1, (double)ay2) && isBetween((double)ax1, (double)bx1, (double)bx2);
				return (isBetween(yintersection, (double)ay1, (double)ay2) && isBetween(yintersection, (double)by1, (double)by2));
			}
			else if (float.IsInfinity(slopeb))
			{
				xintersection = (double)bx1;
				yintersection = (double)slopea * ((double)xintersection - (double)ax1) + (double)ay1;
				if (slopea == 0)
					return isBetween((double)ay1, (double)by1, (double)by2) && isBetween((double)bx1, (double)ax1, (double)ax2);
				return (isBetween(yintersection, (double)ay1, (double)ay2) && isBetween(yintersection, (double)by1, (double)by2));
			}
			else
			{
				xintersection = ((double)slopea * (double)ax1 - (double)slopeb * (double)bx1 - (double)ay1 + (double)by1) / ((double)slopea - (double)slopeb);
				return isBetween(xintersection, (double)ax1, (double)ax2) && isBetween(xintersection, (double)bx1, (double)bx2);
			}

			return false;
		}

		public static bool isBetween(float n, float a, float b)
		{
			if (n < a && n > b) return true;
			if (n > a && n < b) return true;
			return false;
		}
		public static bool isBetween(double n, double a, double b)
		{
			if (n < a && n > b) return true;
			if (n > a && n < b) return true;
			return false;
		}

		public static Vector2 lineCollisionPoint(float ax1, float ay1, float ax2, float ay2, float bx1, float by1, float bx2, float by2)
		{
			if (!linesCollide(ax1, ay1, ax2, ay2, bx1, by1, bx2, by2))
				return new Vector2(-1, -1);

			bool line1Vert = (ax1 == ax2);
			float m1=0, b1=0;
			bool line2Vert = (bx1 == bx2);
			float m2=0, b2=0;

			if (!line1Vert)
			{
				m1 = (ay2 - ay1) / (ax2 - ax1);
				b1 = ay1 - m1 * ax1;
			}
			if (!line2Vert)
			{
				m2 = (by2 - by1) / (bx2 - bx1);
				b2 = by1 - m2 * bx1;
			}

			if (line1Vert && line2Vert)
			{
				if ((ay1 <= by1 && ay1 >= by2) || (ay1 <= by2 && ay1 >= by1))
					return new Vector2(ax1, ay1);
				if ((ay2 <= by1 && ay2 >= by2) || (ay2 <= by2 && ay2 >= by1))
					return new Vector2(ax2, ay2);
				if ((by1 <= ay1 && by1 >= ay2) || (by1 <= ay2 && by1 >= ay1))
					return new Vector2(bx1, by1);
				return new Vector2(bx2, by2);
			}

			if(!line1Vert && !line2Vert)
			{
				//Both diagonal or horizontal
				//Solve for x in: m1*x+b1=m2*x+b2
					//x(m1-m2)=b2-b1
					//x=(b2-b1)/(m1-m2)
				if (m1 == m2)	//Parallel
					return new Vector2(-1, -1);
				float ansx = (b2 - b1) / (m1 - m2);
				float ansy = m1 * ansx + b1;
				return new Vector2(ansx, ansy);
			}

			if (line1Vert)
			{
				float ansx = ax1;
				float ansy = m2 * ansx + b2;
				return new Vector2(ansx, ansy);
			}

			if (line2Vert)
			{
				float ansx = bx1;
				float ansy = m1 * ansx + b1;
				return new Vector2(ansx, ansy);
			}

			return new Vector2(-1, -1);	//Error
		}
		
		/*public static Dir collisionDirection(BB a, BB b, float xs, float ys)
		{
			if ((a.x <= b.x) && (xs > 0))
			{
				return Dir.Left;
			}
			if ((a.x >= b.x) && (xs < 0))
			{
				return Dir.Right;
			}

			if ((a.y <= b.y) && (ys > 0))
			{
				return Dir.Up;
			}
			if ((a.y >= b.y) && (ys < 0))
			{
				return Dir.Down;
			}

			return Dir.Left;
		}//*/
	}

	public struct CollisionInfo
	{
		public bool collides;
		public String side;
		public Vector2 point;
		public obj other;
		public float pct;

		public CollisionInfo(bool fals)
		{
			collides = false;
			other = null;
			side = "";
			point = new Vector2(-1, -1);
			pct = 0;
		}
		public CollisionInfo(bool _collides, obj _other, String _side, Vector2 _point, float _pct)
		{
			collides = _collides;
			other = _other;
			side = _side;
			point = _point;
			pct = _pct;
		}

		public float getSlope()
		{
			if (side == "l" || side == "r") return 1;
			if (side == "u" || side == "d") return 0;
			return 0;
		}
	}

	//public enum Dir { Left, Right, Up, Down };
}