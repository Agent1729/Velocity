﻿using System;
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

		public Texture2D sprite;
		protected Texture2D blackPixel;
		public string spriteName;
		protected Vector2 XY;
		protected Vector2 WH;
		public float width, height;
		protected Vector2 speed;
		public BB bb;
		public string objType;
		public float depth;
		public bool collisionStatic;
		public bool isSolid;
		public bool takesControls;
		public bool canBePushed;
		public bool canAbsorb;
		public bool hasBeenFrictioned;
		public bool isMoving;
		public bool destroyed;

		public float base_width, base_height;

		public bool lmoving, rmoving, umoving, dmoving;

		public obj(float _x, float _y)
		{
			XY.X = _x; XY.Y = _y; speed.X = 0; speed.Y = 0;
			lmoving = false; rmoving = false; umoving = false; dmoving = false;
			isMoving = false;
			destroyed = false;
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
		}

		protected virtual void destroy()
		{
			destroyed = true;
			level.removeNewObj(this);
		}

		public void loadTexture(ContentManager Content) { doLoadTexture(Content); }
		protected virtual void doLoadTexture(ContentManager Content)
		{
			sprite = Content.Load<Texture2D>(spriteName);
			blackPixel = Content.Load<Texture2D>("BlackPixel");
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

		public void tick() { dotick(); }
		protected virtual void dotick()
		{
			//x += xspeed;
			//y += yspeed;
			Move(xspeed, yspeed, true);
		}

		public virtual bool isGrounded() { return false; }

		public virtual Vector2 Move(float mx, float my, bool actuallyMove)
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

			return new Vector2(movedx, movedy);
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

		public void Collision(obj other, bool isPrimary) { doCollision(other, isPrimary); }
		protected virtual void doCollision(obj other, bool isPrimary) { }

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
			drawSprite(spriteBatch, c, sprite, drawP);
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
			#region 1st Try
			//float m1=9999, c1, m2=9999, c2;

			//if (ax1 != ax2)
			//    m1 = (ay2 - ay1) / (ax2 - ax1);
			//c1 = m1 * ax1 - ay1;
			//if (bx1 != bx2)
			//    m2 = (by2 - by1) / (bx2 - bx1);
			//c2 = m2 * bx1 - by1;

			//if (m1 == m2)
			//    return false;

			//if ((m1 != 9999) && (m2 != 9999))
			//{
			//    float x3 = (c2 - c1) / (m1 - m2);
			//    if (((x3 > ax1) && (x3 < ax2)) || ((x3 > ax2) && (x3 < ax1)))
			//        return true;
			//}
			//else if (m1 == 9999)	//m1 is vertical, m2 is not
			//{
			//    float y3 = m2 * ax1 + c2;
			//    if (((y3 > ay1) && (y3 < ay2)) || ((y3 > ay2) && (y3 < ay1)))
			//        return true;
			//}
			//else if (m2 == 9999)	//m2 is vertical, m1 is not
			//{
			//    float y3 = m1 * bx1 + c1;
			//    if (((y3 > by1) && (y3 < by2)) || ((y3 > by2) && (y3 < by1)))
			//        return true;
			//}

			//return false;
			#endregion 1st Try

			float denominator = ((ax2 - ax1) * (by2 - by1)) - ((ay2 - ay1) * (bx2 - bx1));
			float numerator1 = ((ay1 - by1) * (bx2 - bx1)) - ((ax1 - bx1) * (by2 - by1));
			float numerator2 = ((ay1 - by1) * (ax2 - ax1)) - ((ax1 - bx1) * (ay2 - ay1));
			//float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
			//float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
			//float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

			// Detect coincident lines (has a problem, read below)
			// (Doesn't work if lines are coincident but don't overlap?)
			if (denominator == 0)
				return ((numerator1 == 0) && (numerator2 == 0));

			float r = numerator1 / denominator;
			float s = numerator2 / denominator;

			return ((r >= 0 && r <= 1) && (s >= 0 && s <= 1));

			//return false;


			/*
			public static bool lineIntersect(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
			{
				var x=((x1*y2-y1*x2)*(x3-x4)-(x1-x2)*(x3*y4-y3*x4))/((x1-x2)*(y3-y4)-(y1-y2)*(x3-x4));
				var y=((x1*y2-y1*x2)*(y3-y4)-(y1-y2)*(x3*y4-y3*x4))/((x1-x2)*(y3-y4)-(y1-y2)*(x3-x4));
				if (isNaN(x)||isNaN(y))
					return false;
				else
				{
					if (x1>=x2)
					{
						if (!(x2<=x&&x<=x1)) {return false;}
					}
					else
					{
						if (!(x1<=x&&x<=x2)) {return false;}
					}
					if (y1>=y2)
					{
						if (!(y2<=y&&y<=y1)) {return false;}
					}
					else
					{
						if (!(y1<=y&&y<=y2)) {return false;}
					}
					if (x3>=x4)
					{
						if (!(x4<=x&&x<=x3)) {return false;}
					}
					else
					{
						if (!(x3<=x&&x<=x4)) {return false;}
					}
					if (y3>=y4)
					{
						if (!(y4<=y&&y<=y3)) {return false;}
					}
					else
					{
						if (!(y3<=y&&y<=y4)) {return false;}
					}
				}
				return true;
			}
			//*/
		}
		
		public static Dir collisionDirection(BB a, BB b, float xs, float ys)
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
		}
	}

	public enum Dir { Left, Right, Up, Down };
}