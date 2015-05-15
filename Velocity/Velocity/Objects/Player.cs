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
	public class Player : VelocityObj
	{
		public int num;
		public float moveSpeed;

		public bool dead = false;
		public bool canBeControlled = true;
		public float cantBeControlledTimer = 0;

		public bool isJumping = false;
		public bool grounded = false;
		public float jumpSpeed = 8;

		private string cursorSpriteName;
		private SpriteFont font;
		private Texture2D cursorSprite;
		private float plrScale = 7;

		public VelocityZone vz;
		private string velocityZoneSpriteName;
		private Texture2D velocityZoneSprite;

		private Vector2 clickStarted = new Vector2();
		private Vector2 clickEnded = new Vector2();
		private int isShooting = 0;
		private char lrMousePressed;
		private VelocityNode clickedOn = null;
		//private bool shiftIsDown = false;

		private float minVSize = 15;
		private float maxVSize = 200;

		private int gunSelected = 1;
		private bool hasGun1A = false, hasGun1B = false, hasGun2A = false, hasGun2B = false;
		private bool hasGun3A = false, hasGun3B = false, hasGun4 = false, hasGun5 = false;
		private float shotSpeed = 10;
		public VelocityShot velocityShot = null;
		public VelocityNode nodeShot = null;

		public SoundEffect se;
		public SoundEffectInstance sei;

		public Player(float x, float y, int _num) : base(x, y)
		{
			num = _num;
			moveSpeed = 5.0f;
		}

		public Player(float x, float y, int _num, VelocityZone _vz) : base(x, y)
		{
			num = _num;
			moveSpeed = 5.0f;
			vz = _vz;
		}

		protected override void init()
		{
			objType = "Player";
			//spriteName = "2048";
			mainSpriteName = "Player";
			cursorSpriteName = "cursor1";
			velocityZoneSpriteName = "BlueCircle";
			base_width = 70 / plrScale; base_height = 70 / plrScale;
			width = base_width; height = base_height;
			depth = 1;
			collisionStatic = false;
			isSolid = true;
			takesControls = true;
			canBePushed = true;
			hasGravity = true;
			canAbsorb = true;
			hasBeenFrictioned = false;

			base.init();
		}

		protected override void doLoadTexture()
		{
			mainSprite = SpriteManager.getSprite(mainSpriteName);
			cursorSprite = SpriteManager.getSprite(cursorSpriteName);
			velocityZoneSprite = SpriteManager.getSprite(velocityZoneSpriteName);
			blackPixel = SpriteManager.getSprite("BlackPixel");

			font = FontManager.getFont("Font1");

			se = SoundManager.getSound("sound110");
		}

		#region controls

		protected override void doleftDown(object lvl)
		{
			if (canBeControlled)
			if (!lmoving)
			{
				xspeed -= moveSpeed;
				lmoving = true;
			}
		}
		protected override void dorightDown(object lvl)
		{
			if (canBeControlled)
			if (!rmoving)
			{
				xspeed += moveSpeed;
				rmoving = true;
			}
		}
		protected override void doupDown(object lvl)
		{
			//if ((!umoving)&&(isGrounded()))
			if (canBeControlled)
			if(isGrounded()&&(yspeed==0))
			{
				if (gravFactor > 0)
				{
					yspeed -= jumpSpeed;
					umoving = true;
				}
				if (gravFactor < 0)
				{
					yspeed += jumpSpeed;
					dmoving = true;
				}
			}
		}
		protected override void dodownDown(object lvl)
		{
			/*if (!dmoving)
			{
				yspeed += moveSpeed;
				dmoving = true;
			}*/
		}
		protected override void doleftReleased(object lvl)
		{
			if (canBeControlled)
			if (lmoving)
			{
				xspeed += moveSpeed;
				lmoving = false;
			}
		}
		protected override void dorightReleased(object lvl)
		{
			if (canBeControlled)
			if (rmoving)
			{
				xspeed -= moveSpeed;
				rmoving = false;
			}
		}
		protected override void doupReleased(object lvl)
		{
			/*if (umoving)
			{
				yspeed += moveSpeed;
				umoving = false;
			}*/
		}
		protected override void dodownReleased(object lvl)
		{
			/*if (dmoving)
			{
				yspeed -= moveSpeed;
				dmoving = false;
			}*/
		}

		protected override void doclearPressed(object lvl)
		{
			if (!dead)
			{
				if (isShooting == 0)
					vz.Clear();
				else
					isShooting = 0;
			}
		}

		protected override void don1Down(object lvl) { if ((isShooting == 0) && (hasGun1A || hasGun1B) && (canBeControlled)) gunSelected = 1; }
		protected override void don2Down(object lvl) { if ((isShooting == 0) && (hasGun2A || hasGun2B) && (canBeControlled)) gunSelected = 2; }
		protected override void don3Down(object lvl) { if ((isShooting == 0) && (hasGun3A || hasGun3B) && (canBeControlled)) gunSelected = 3; }
		protected override void don4Down(object lvl) { if ((isShooting == 0) && (hasGun4) && (canBeControlled)) gunSelected = 4; }
		protected override void don5Down(object lvl) { if ((isShooting == 0) && (hasGun5) && (canBeControlled)) gunSelected = 5; }

		//protected override void doshiftPressed(object lvl) { shiftIsDown = true; }
		//protected override void doshiftReleased(object lvl) { shiftIsDown = false; }

		#endregion

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

			isGrounded();
			if (hasGravity)
			{
				if (!factorAffectsGrav)
					yspeed += gravity * factor * gravFactor;
				else
					yspeed += gravity * gravFactor;
			}

			capSpeed(terminalVelocity);
			Move(xspeed * factor, yspeed * factor, true);

			if (!dead)
			{
				if (cantBeControlledTimer > 0)
					cantBeControlledTimer--;
				else
					canBeControlled = true;
				if (factor < 0)
					LoseControl();
			}

			factorSet = false;
			newFactor = 1;
			newGravFactor = 1;
		}

		protected override void doCollision(obj other, bool isPrimary)
		{/*
			if (other.objType == "Wall")
				wallCollide(other, isPrimary, 0);
			if (other.objType == "MovingPlatform")
				wallCollide(other, isPrimary, 1);
			if (other.objType == "Box")
				wallCollide(other, isPrimary, 0);//*/
			if (other.objType == "VelocityZone")
				vZoneCollide(other, isPrimary);
		}

		public override bool isGrounded()
		{
			List<obj> colls = level.collisionListAtRelative(this, 0, obj.Sign(gravFactor), true);
			
			if (colls.Count > 0)
			{
				if (yspeed * gravFactor > 0)
					yspeed = 0;
				return true;
			}
			return false;
		}

		protected override void doDraw(SpriteBatch spriteBatch, Camera c)
		{
			//drawText(spriteBatch, c, font, "HELLO WORLD!", XY, Color.White);

			//Cursor stats
			MouseState ms = Mouse.GetState();
			Vector2 cs = new Vector2(clickStarted.X, clickStarted.Y); cs *= c.zoom; cs += c.XY;
			Vector2 ce = new Vector2(ms.X, ms.Y); ce *= c.zoom; ce += c.XY;
			if (isShooting == 3)
			{ cs.X = clickedOn.x; cs.Y = clickedOn.y; }
			float dist = (float)Math.Sqrt(Math.Abs(ce.X - cs.X) * Math.Abs(ce.X - cs.X) + Math.Abs(ce.Y - cs.Y) * Math.Abs(ce.Y - cs.Y));
			if (dist >= maxVSize) dist = maxVSize; if (dist <= minVSize) dist = minVSize;
			float scale = dist / vz.base_height;

			//Draw vzone preview
			if (hasAGun())
			{
				if (((isShooting == 1) || (isShooting == 3)) && (gunSelected < 4))
				{ drawSprite(spriteBatch, c, velocityZoneSprite, new Vector2(cs.X - dist, cs.Y - dist), new Color(0, 0, 0, 150), scale); }
				else if (((isShooting == 2) || (isShooting == 3)) && (gunSelected < 4))
				{ drawSprite(spriteBatch, c, velocityZoneSprite, new Vector2(cs.X - dist, cs.Y - dist), new Color(0, 0, 0, 150), scale); }
			}

			//Aimer line
			if (hasAGun() && canBeControlled)
			{
				float endX = 0, endY = 0;
				if ((isShooting == 0) || (gunSelected >= 4))
				{ endX = ce.X; endY = ce.Y; }
				else if ((isShooting == 1) || (isShooting == 2))
				{ endX = cs.X; endY = cs.Y; }
				else if (isShooting == 3)
				{ endX = cs.X; endY = cs.Y; }

				//Check if end collides anything, and reset it here
				float endX1 = endX, endY1 = endY;
				float maxDist = BB.dist(x, y, endX, endY);
				float dist2 = maxDist;
				obj furthestObj = null;
				List<obj> colls = level.collisionListAlongLine(x, y, endX, endY, true);
				foreach (obj o in colls)
				{
					if (o == this)
						continue;
					dist2 = BB.dist(x, y, o.x, o.y);
					if (dist2 < maxDist)
					{
						maxDist = dist2;
						furthestObj = o;
					}
				}
				if (furthestObj != null)
				{
					endX = furthestObj.x;
					endY = furthestObj.y;
				}

				//drawLine(spriteBatch, c, blackPixel, new Vector2(x, y), (new Vector2(endX, endY)));
				drawLine(spriteBatch, c, blackPixel, new Vector2(x, y), (new Vector2(endX1, endY1)));
			}

			//Self
			getPaintColor();
			drawSprite(spriteBatch, c, mainSprite, drawP, paintedColor, 1f / plrScale);

			//Cursor
			drawSpriteNoZoom(spriteBatch, c, cursorSprite, new Vector2(ce.X, ce.Y));
		}

		#region Gun Control

		protected override void dolmPressed(object lvl)
		{
			if (canBeControlled)
			{
				if (gunSelected >= 4)
				{
					Camera c = level.camera;
					clickEnded.X = Mouse.GetState().X; clickEnded.Y = Mouse.GetState().Y;
					Vector2 cs = new Vector2(clickStarted.X, clickStarted.Y); cs *= c.zoom; cs += c.XY;
					Vector2 ce = new Vector2(clickEnded.X, clickEnded.Y); ce *= c.zoom; ce += c.XY;

					if (isShooting == 3)
					{ cs.X = clickedOn.x; cs.Y = clickedOn.y; }

					float dist = (float)Math.Sqrt(Math.Abs(ce.X - cs.X) * Math.Abs(ce.X - cs.X)
						+ Math.Abs(ce.Y - cs.Y) * Math.Abs(ce.Y - cs.Y));
					if (dist >= maxVSize) dist = maxVSize; if (dist <= minVSize) dist = minVSize;

					float ang;
					if (gunSelected < 4)
					{
						ang = (float)Math.Atan((cs.Y - y) / (cs.X - x));
						if (cs.X < x)
							ang += (float)Math.PI;
					}
					else
					{
						ang = (float)Math.Atan((ce.Y - y) / (ce.X - x));
						if (ce.X < x)
							ang += (float)Math.PI;
					}
					float shotxspd = (float)Math.Cos(ang) * shotSpeed;
					float shotyspd = (float)Math.Sin(ang) * shotSpeed;
					float scale = dist / vz.base_height;
					//vz.x = cs.X; vz.y = cs.Y;
					//vz.rescale(scale);
					if ((gunSelected == 4) && (hasGun4) && (nodeShot == null))
					{
						StickyNode newShot = (StickyNode)level.addNewObj(new StickyNode(x, y, shotxspd, shotyspd));
						newShot.setStats(this);
						nodeShot = newShot;
					}
					else if ((gunSelected == 5) && (hasGun5) && (nodeShot == null))
					{
						PaintNode newShot = (PaintNode)level.addNewObj(new PaintNode(x, y, ce.X, ce.Y, shotxspd, shotyspd));
						newShot.setStats(this);
						nodeShot = newShot;
					}
				}
				else if (isShooting == 0)
				{
					lrMousePressed = 'l';
					clickStarted.X = Mouse.GetState().X; clickStarted.Y = Mouse.GetState().Y;
					isShooting = 1;
					Camera c = level.camera;
					Vector2 cs = new Vector2(clickStarted.X, clickStarted.Y); cs *= c.zoom; cs += c.XY;
					List<obj> colls = level.collisionListAtPoint(cs.X, cs.Y, false);
					foreach (obj o in colls)
					{
						VelocityNode node = o as VelocityNode;
						if (node != null)
						{
							isShooting = 3;
							clickedOn = node;
						}
					}
				}
			}
		}

		protected override void dormPressed(object lvl)
		{
			if (canBeControlled)
			{
				if (gunSelected >= 4)
				{
					if (nodeShot != null)
						nodeShot.unStick();
				}
				else if (isShooting == 0)
				{
					lrMousePressed = 'r';
					clickStarted.X = Mouse.GetState().X; clickStarted.Y = Mouse.GetState().Y;
					isShooting = 2;
					Camera c = level.camera;
					Vector2 cs = new Vector2(clickStarted.X, clickStarted.Y); cs *= c.zoom; cs += c.XY;
					List<obj> colls = level.collisionListAtPoint(cs.X, cs.Y, false);
					foreach (obj o in colls)
					{
						VelocityNode node = o as VelocityNode;
						if (node != null)
						{
							isShooting = 3;
							clickedOn = node;
						}
					}
				}
			}
		}

		protected override void dolmReleased(object lvl)
		{
			if (canBeControlled)
			{
				if ((isShooting != 0) && (lrMousePressed == 'l'))
				{
					Camera c = level.camera;
					clickEnded.X = Mouse.GetState().X; clickEnded.Y = Mouse.GetState().Y;
					Vector2 cs = new Vector2(clickStarted.X, clickStarted.Y); cs *= c.zoom; cs += c.XY;
					Vector2 ce = new Vector2(clickEnded.X, clickEnded.Y); ce *= c.zoom; ce += c.XY;

					if (isShooting == 3)
					{ cs.X = clickedOn.x; cs.Y = clickedOn.y; }

					float dist = (float)Math.Sqrt(Math.Abs(ce.X - cs.X) * Math.Abs(ce.X - cs.X)
						+ Math.Abs(ce.Y - cs.Y) * Math.Abs(ce.Y - cs.Y));
					if (dist >= maxVSize) dist = maxVSize; if (dist <= minVSize) dist = minVSize;

					float ang;
					if (gunSelected < 4)
					{
						ang = (float)Math.Atan((cs.Y - y) / (cs.X - x));
						if (cs.X < x)
							ang += (float)Math.PI;
					}
					else
					{
						ang = (float)Math.Atan((ce.Y - y) / (ce.X - x));
						if (ce.X < x)
							ang += (float)Math.PI;
					}
					float shotxspd = (float)Math.Cos(ang) * shotSpeed;
					float shotyspd = (float)Math.Sin(ang) * shotSpeed;
					float scale = dist / vz.base_height;
					//vz.x = cs.X; vz.y = cs.Y;
					//vz.rescale(scale);
					if ((gunSelected == 1) && (hasGun1A) && (velocityShot == null))
					{
						//vz.vFactor = (.3f * scale);
						//vz.vGravFactor = 1f;
						velocityShot = shootVelocity(shotxspd, shotyspd, cs.X, cs.Y, scale, .3f * scale, 1, 1);
					}
					else if ((gunSelected == 2) && (hasGun2A) && (velocityShot == null))
					{
						//vz.vFactor = 1f;
						//vz.vGravFactor = (-.444f * scale + 2.65f);
						velocityShot = shootVelocity(shotxspd, shotyspd, cs.X, cs.Y, scale, 1, -.444f * scale + 2.65f, 3);
					}
					else if ((gunSelected == 3) && (velocityShot == null))
					{
						//vz.vFactor = -1f;
						//vz.vGravFactor = 1f;
						velocityShot = shootVelocity(shotxspd, shotyspd, cs.X, cs.Y, scale, -1, 1, 5);
					}
					isShooting = 0;
				}
			}
		}

		protected override void dormReleased(object lvl)
		{
			if (canBeControlled)
			{
				if ((isShooting != 0) && (lrMousePressed == 'r'))
				{
					Camera c = level.camera;
					clickEnded.X = Mouse.GetState().X; clickEnded.Y = Mouse.GetState().Y;
					Vector2 cs = new Vector2(clickStarted.X, clickStarted.Y); cs *= c.zoom; cs += c.XY;
					Vector2 ce = new Vector2(clickEnded.X, clickEnded.Y); ce *= c.zoom; ce += c.XY;

					if (isShooting == 3)
					{ cs.X = clickedOn.x; cs.Y = clickedOn.y; }

					float dist = (float)Math.Sqrt(Math.Abs(ce.X - cs.X) * Math.Abs(ce.X - cs.X)
						+ Math.Abs(ce.Y - cs.Y) * Math.Abs(ce.Y - cs.Y));
					if (dist >= maxVSize) dist = maxVSize; if (dist <= minVSize) dist = minVSize;

					float ang;
					if (gunSelected < 4)
					{
						ang = (float)Math.Atan((cs.Y - y) / (cs.X - x));
						if (cs.X < x)
							ang += (float)Math.PI;
					}
					else
					{
						ang = (float)Math.Atan((ce.Y - y) / (ce.X - x));
						if (ce.X < x)
							ang += (float)Math.PI;
					}
					float shotxspd = (float)Math.Cos(ang) * shotSpeed;
					float shotyspd = (float)Math.Sin(ang) * shotSpeed;
					float scale = dist / vz.base_height;
					//vz.x = cs.X; vz.y = cs.Y;
					//vz.rescale(scale);
					if ((gunSelected == 1) && (hasGun1B) && (velocityShot == null))
					{
						//vz.vFactor = (-.444f * scale + 2.65f);
						//vz.vGravFactor = 1f;
						velocityShot = shootVelocity(shotxspd, shotyspd, cs.X, cs.Y, scale, -.444f * scale + 2.65f, 1, 2);
					}
					else if ((gunSelected == 2) && (hasGun2B) && (velocityShot == null))
					{
						//vz.vFactor = 1f;
						//vz.vGravFactor = (.3f * scale);
						velocityShot = shootVelocity(shotxspd, shotyspd, cs.X, cs.Y, scale, 1, .3f * scale, 4);
					}
					else if ((gunSelected == 3) && (hasGun3B) && (velocityShot == null))
					{
						//vz.vFactor = 1f;
						//vz.vGravFactor = -1f;
						velocityShot = shootVelocity(shotxspd, shotyspd, cs.X, cs.Y, scale, 1, -1, 6);
					}
					isShooting = 0;
				}
			}
		}

		public void setGunsOwned(bool g1A, bool g1B, bool g2A, bool g2B, bool g3A, bool g3B, bool g4, bool g5)
		{
			hasGun1A = g1A; hasGun1B = g1B; hasGun2A = g2A; hasGun2B = g2B;
			hasGun3A = g3A; hasGun3B = g3B; hasGun4 = g4; hasGun5 = g5;
		}
		public void setGunsOwned(bool has1, bool has2, bool has3, bool has4, bool has5)
		{
			hasGun1A = has1; hasGun1B = has1; hasGun2A = has2; hasGun2B = has2;
			hasGun3A = has3; hasGun3B = has3; hasGun4 = has4; hasGun5 = has5;
		}
		public void setGunsOwned(bool hasAll)
		{
			hasGun1A = hasAll; hasGun1B = hasAll; hasGun2A = hasAll; hasGun2B = hasAll;
			hasGun3A = hasAll; hasGun3B = hasAll; hasGun4 = hasAll; hasGun5 = hasAll;
		}
		public bool hasAGun()
		{ return (hasGun1A || hasGun1B || hasGun2A || hasGun2B || hasGun3A || hasGun3B || hasGun4 || hasGun5); }

		#endregion Gun Control

		protected VelocityShot shootVelocity(float shotxspd, float shotyspd, float _x, float _y, float _scale, float _vFac, float _vGFac, int _type)
		{
			//SoundEffectInstance sei2 = se.CreateInstance();
			/*
			if (sei == null)
			{
				sei = se.CreateInstance();
				sei.IsLooped = true;
				sei.Play();
			}
			else
			{
				sei.Stop();
				sei = null;
			}//*/

			VelocityShot newShot = (VelocityShot)level.addNewObj(new VelocityShot(x, y, shotxspd, shotyspd));
			newShot.setVelocityStats(_x, _y, _scale, _vFac, _vGFac, _type, vz, this);
			if (isShooting == 3)
				newShot.seeking = clickedOn;
			return newShot;
		}

		public void Die()
		{
			if (!dead)
			{
				dead = true;
				LoseControl();
				level.WaitThenRestart(100);
			}
		}

		public void LoseControl()
		{
			canBeControlled = false;
			cantBeControlledTimer = 2;
			if (lmoving) { lmoving = false; xspeed += moveSpeed; }
			if (rmoving) { rmoving = false; xspeed -= moveSpeed; }
			if (umoving) { umoving = false; }
			if (dmoving) { dmoving = false; }
			if (isShooting > 0) isShooting = 0;
		}
	}
}
