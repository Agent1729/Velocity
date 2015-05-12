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
	public class Level
	{
		KeyboardState oldState;
		MouseState oldMouse;
		Color backColor = Color.CornflowerBlue;
		public Camera camera;
		public bool shouldQuit = false;
		VelocityGame myGame;
		protected int levelNum;
		public float gameTicks = 0;
		private float restartTime = -1;
		private char pauseResumeState = 'r';
		private char restartState = 'n';

		//Texture2D blackPixel;

		int debugTest = 0;

		List<obj> objs = new List<obj>();
		List<obj> objsToAdd = new List<obj>();
		List<obj> objsToRemove = new List<obj>();

		#region Control Events/Lists
		public delegate void KeyEventHandler(object sender);
		public event KeyEventHandler LeftDown, LeftPressed, LeftReleased;
		public event KeyEventHandler RightDown, RightPressed, RightReleased;
		public event KeyEventHandler UpDown, UpPressed, UpReleased;
		public event KeyEventHandler DownDown, DownPressed, DownReleased;

		public event KeyEventHandler ShiftDown, ShiftPressed, ShiftReleased;
		public event KeyEventHandler ClearDown, ClearPressed, ClearReleased;

		public event KeyEventHandler N1Down, N1Pressed, N1Released;
		public event KeyEventHandler N2Down, N2Pressed, N2Released;
		public event KeyEventHandler N3Down, N3Pressed, N3Released;
		public event KeyEventHandler N4Down, N4Pressed, N4Released;
		public event KeyEventHandler N5Down, N5Pressed, N5Released;

		public delegate void MouseEventHandler(object sender);
		public event MouseEventHandler LMDown, LMPressed, LMReleased;
		public event MouseEventHandler RMDown, RMPressed, RMReleased;

		private List<Keys> leftKeys = new List<Keys>();
		private List<Keys> rightKeys = new List<Keys>();
		private List<Keys> upKeys = new List<Keys>();
		private List<Keys> downKeys = new List<Keys>();

		private List<Keys> escKeys = new List<Keys>();
		private List<Keys> restartKeys = new List<Keys>();
		private List<Keys> pauseKeys = new List<Keys>();
		private List<Keys> shiftKeys = new List<Keys>();
		private List<Keys> clearKeys = new List<Keys>();

		private List<Keys> n1Keys = new List<Keys>();
		private List<Keys> n2Keys = new List<Keys>();
		private List<Keys> n3Keys = new List<Keys>();
		private List<Keys> n4Keys = new List<Keys>();
		private List<Keys> n5Keys = new List<Keys>();
		#endregion

		public void Initialize(VelocityGame game, int gameWidth, int gameHeight)
		{
			myGame = game;
			oldState = Keyboard.GetState();
			oldMouse = Mouse.GetState();

			//blackPixel = game.Content.Load<Texture2D>("BlackPixel");

			camera = new Camera(0, 0, 1, gameWidth, gameHeight);

			leftKeys.Add(Keys.Left); leftKeys.Add(Keys.A);
			rightKeys.Add(Keys.Right); rightKeys.Add(Keys.D);
			upKeys.Add(Keys.Up); upKeys.Add(Keys.W);
			downKeys.Add(Keys.Down); downKeys.Add(Keys.S);

			escKeys.Add(Keys.Escape);
			restartKeys.Add(Keys.R);
			pauseKeys.Add(Keys.P);
			shiftKeys.Add(Keys.LeftShift); shiftKeys.Add(Keys.RightShift);
			clearKeys.Add(Keys.C);

			n1Keys.Add(Keys.D1); n2Keys.Add(Keys.D2); n3Keys.Add(Keys.D3); n4Keys.Add(Keys.D4); n5Keys.Add(Keys.D5);

			doInitialize(game);
		}

		protected virtual void doInitialize(VelocityGame game) { }

		public void Update(VelocityGame game)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				Quit(game);
			}

			//Step
			if(!Paused())
			foreach (obj o in objs)
				o.tick();

			//Add new objects created
			if (objsToAdd.Count > 0)
			{
				foreach (obj o in objsToAdd)
				{
					o.tick();
					objs.Add(o);
				}
				objs.Sort(CompareDepths);
				objsToAdd.Clear();
			}

			//Remove objects
			foreach (obj o in objsToRemove)
				removeObj(o);
			objsToRemove.Clear();

			//Collision engine
			if (!Paused())
			for (int i = 0; i < objs.Count; i++)
			{
				if (objs[i].collisionStatic == true)
					continue;	//Ignore collision static collisions
				for (int j = 0; j < objs.Count; j++)
				{
					if (i == j)
						continue;	//Don't collide with self of course
					//if ((j > i) && (objs[i].collisionStatic == false))
					//	continue;	//Don't double collide 2 nonstatic objects, we already did this for i
					//Check for collision between these two
					if (BB.collides(objs[i].bb, objs[j].bb))
					{
						objs[i].Collision(objs[j], true);
						objs[j].Collision(objs[i], false);
					}
				}
			}

			UpdateInput(game);

			gameTicks++;

			//Wait-then-restart timers
			if (restartTime == gameTicks)
			{
				if (restartState == 'r')
				{
					restartTime += 100;
					restartState = 'q';
					pauseResumeState = 'P';
				}
				else if (restartState == 'q')
				{
					SwitchLevel(game, levelNum);
				}
			}

			if (shouldQuit)
				Quit(game);
		}

		public void Draw(VelocityGame game)
		{
			game.graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			//game.graphics.GraphicsDevice.Clear(new Color(100, 149, 237, 0));
			//game.graphics.GraphicsDevice.Clear(new Color(255, 255, 255, 0));

			//game.spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
			game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

			/*
			Texture2D SimpleTexture = new Texture2D(game.graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			UInt32[] pixel = { 0xFFFFFFFF }; SimpleTexture.SetData<UInt32>(pixel, 0, SimpleTexture.Width * SimpleTexture.Height);
			game.spriteBatch.Draw(SimpleTexture, new Rectangle(200, 200, 100, 10), null, Color.Black,
				(float)(2.5 * Math.PI / 4), new Vector2(0f, 0f), SpriteEffects.None, 1f);
			//*/

			foreach (obj o in objs)
			{
				o.draw(game.spriteBatch, camera);
			}

			game.spriteBatch.End();
		}

		public bool Paused()
		{
			return ((pauseResumeState == 'p') || (pauseResumeState == 'P') ||
				(pauseResumeState == 'q') || (pauseResumeState == 'Q'));
		}

		public void SwitchLevel(VelocityGame game, int num)
		{
			game.SwitchLevel(num);
		}



		protected void UpdateInput(VelocityGame game)
		{
			KeyboardState newState = Keyboard.GetState();
			MouseState newMouse = Mouse.GetState();

			#region Miscellaneous Controls
			//Esc
			if (isKeysetDown(newState, escKeys))
			{ game.Exit(); }

			//Restart
			if (isKeysetDown(newState, restartKeys))
				if (gameTicks > 15)
					{ game.SwitchLevel(levelNum); }

			//Pause
			if ((isKeysetDown(newState, pauseKeys))&&(!isKeysetDown(oldState, pauseKeys)))
			{
				if (pauseResumeState == 'p')
					pauseResumeState = 'r';
				else if (pauseResumeState == 'r')
					pauseResumeState = 'p';
			}

			//Skip Levels (Numpad 4/6)
			if (newState.IsKeyDown(Keys.NumPad4))
				if (gameTicks > 15)
					{ game.SwitchLevel(levelNum - 1); }
			if (newState.IsKeyDown(Keys.NumPad6))
				if (gameTicks > 15)
					{ game.SwitchLevel(levelNum + 1); }
			#endregion Miscellaneous Controls

			#region Camera Controls
			//J
			if (newState.IsKeyDown(Keys.J))
				camera.moveLeft(1);
			//L
			if (newState.IsKeyDown(Keys.L))
				camera.moveRight(1);
			//I
			if (newState.IsKeyDown(Keys.I))
				camera.moveUp(1);
			//K
			if (newState.IsKeyDown(Keys.K))
				camera.moveDown(1);
			//U
			if (newState.IsKeyDown(Keys.U))
			{
				if (!oldState.IsKeyDown(Keys.U))
					camera.Zoom(camera.zoomSpeed);
			}
			//O
			if (newState.IsKeyDown(Keys.O))
			{
				if (!oldState.IsKeyDown(Keys.O))
					camera.Zoom(1f / camera.zoomSpeed);
			}
			#endregion

			#region LRUD
				//Left
				if (isKeysetDown(newState, leftKeys))
				{
					KeyEventHandler h = LeftDown; if (h != null) h(this);
					if (!isKeysetDown(oldState, leftKeys))
					{ KeyEventHandler h2 = LeftPressed; if (h2 != null) h2(this); }
				}
				else if (isKeysetDown(oldState, leftKeys))
				{ KeyEventHandler h3 = LeftReleased; if (h3 != null) h3(this); }

				//Right
				if (isKeysetDown(newState, rightKeys))
				{
					KeyEventHandler h = RightDown; if (h != null) h(this);
					if (!isKeysetDown(oldState, rightKeys))
					{ KeyEventHandler h2 = RightPressed; if (h2 != null) h2(this); }
				}
				else if (isKeysetDown(oldState, rightKeys))
				{ KeyEventHandler h3 = RightReleased; if (h3 != null) h3(this); }

				//Up
				if (isKeysetDown(newState, upKeys))
				{
					KeyEventHandler h = UpDown; if (h != null) h(this);
					if (!isKeysetDown(oldState, upKeys))
					{ KeyEventHandler h2 = UpPressed; if (h2 != null) h2(this); }
				}
				else if (isKeysetDown(oldState, upKeys))
				{ KeyEventHandler h3 = UpReleased; if (h3 != null) h3(this); }

				//Down
				if (isKeysetDown(newState, downKeys))
				{
					KeyEventHandler h = DownDown; if (h != null) h(this);
					if (!isKeysetDown(oldState, downKeys))
					{ KeyEventHandler h2 = DownPressed; if (h2 != null) h2(this); }
				}
				else if (isKeysetDown(oldState, downKeys))
				{ KeyEventHandler h3 = DownReleased; if (h3 != null) h3(this); }
				#endregion

			#region Other Player Controls
			//Shift
			if (isKeysetDown(newState, shiftKeys))
			{
				KeyEventHandler h = ShiftDown; if (h != null) h(this);
				if (!isKeysetDown(oldState, shiftKeys))
				{ KeyEventHandler h2 = ShiftPressed; if (h2 != null) h2(this); }
			}
			else if (isKeysetDown(oldState, shiftKeys))
			{ KeyEventHandler h3 = ShiftReleased; if (h3 != null) h3(this); }

			//Clear
			if (isKeysetDown(newState, clearKeys))
			{
				KeyEventHandler h = ClearDown; if (h != null) h(this);
				if (!isKeysetDown(oldState, clearKeys))
				{ KeyEventHandler h2 = ClearPressed; if (h2 != null) h2(this); }
			}
			else if (isKeysetDown(oldState, clearKeys))
			{ KeyEventHandler h3 = ClearReleased; if (h3 != null) h3(this); }
			#endregion Other Player Controls

			#region Numbers

			//1
			if (isKeysetDown(newState, n1Keys))
			{
				KeyEventHandler h = N1Down; if (h != null) h(this);
				if (!isKeysetDown(oldState, n1Keys))
				{ KeyEventHandler h2 = N1Pressed; if (h2 != null) h2(this); }
			}
			else if (isKeysetDown(oldState, n1Keys))
			{ KeyEventHandler h3 = N1Released; if (h3 != null) h3(this); }

			//2
			if (isKeysetDown(newState, n2Keys))
			{
				KeyEventHandler h = N2Down; if (h != null) h(this);
				if (!isKeysetDown(oldState, n2Keys))
				{ KeyEventHandler h2 = N2Pressed; if (h2 != null) h2(this); }
			}
			else if (isKeysetDown(oldState, n2Keys))
			{ KeyEventHandler h3 = N2Released; if (h3 != null) h3(this); }

			//3
			if (isKeysetDown(newState, n3Keys))
			{
				KeyEventHandler h = N3Down; if (h != null) h(this);
				if (!isKeysetDown(oldState, n3Keys))
				{ KeyEventHandler h2 = N3Pressed; if (h2 != null) h2(this); }
			}
			else if (isKeysetDown(oldState, n3Keys))
			{ KeyEventHandler h3 = N3Released; if (h3 != null) h3(this); }

			//4
			if (isKeysetDown(newState, n4Keys))
			{
				KeyEventHandler h = N4Down; if (h != null) h(this);
				if (!isKeysetDown(oldState, n4Keys))
				{ KeyEventHandler h2 = N4Pressed; if (h2 != null) h2(this); }
			}
			else if (isKeysetDown(oldState, n4Keys))
			{ KeyEventHandler h3 = N4Released; if (h3 != null) h3(this); }

			//5
			if (isKeysetDown(newState, n5Keys))
			{
				KeyEventHandler h = N5Down; if (h != null) h(this);
				if (!isKeysetDown(oldState, n5Keys))
				{ KeyEventHandler h2 = N5Pressed; if (h2 != null) h2(this); }
			}
			else if (isKeysetDown(oldState, n5Keys))
			{ KeyEventHandler h3 = N5Released; if (h3 != null) h3(this); }

			#endregion

			#region Mouse Controls
			//Left Mouse
			if (newMouse.LeftButton == ButtonState.Pressed)
			{
				MouseEventHandler h = LMDown; if (h != null) h(this);
				if (!(oldMouse.LeftButton == ButtonState.Pressed))
				{ MouseEventHandler h2 = LMPressed; if (h2 != null) h2(this); }
			}
			else if (oldMouse.LeftButton == ButtonState.Pressed)
			{ MouseEventHandler h3 = LMReleased; if (h3 != null) h3(this); }

			//Right Mouse
			if (newMouse.RightButton == ButtonState.Pressed)
			{
				MouseEventHandler h = RMDown; if (h != null) h(this);
				if (!(oldMouse.RightButton == ButtonState.Pressed))
				{ MouseEventHandler h2 = RMPressed; if (h2 != null) h2(this); }
			}
			else if (oldMouse.RightButton == ButtonState.Pressed)
			{ MouseEventHandler h3 = RMReleased; if (h3 != null) h3(this); }
			#endregion

			//End
			oldState = newState;
			oldMouse = newMouse;

			//Debug Pause
			if (newState.IsKeyDown(Keys.H) && (debugTest == 0))
				{ debugTest = 1; }
			if (newState.IsKeyUp(Keys.H)) { debugTest = 0; }
		}

		public bool isKeysetDown(KeyboardState ks, List<Keys> keys)
		{
			foreach (Keys k in keys)
				if (ks.IsKeyDown(k))
					return true;
			return false;
		}

		#region Object Manipulation

		protected obj addObj(VelocityGame game, obj o)
		{
			objs.Add(o);
			objs.Sort(CompareDepths);
			return initializeObj(game, o);
		}

		public obj addNewObj(obj o)
		{
			objsToAdd.Add(o);
			return initializeObj(myGame, o);
		}

		protected obj initializeObj(VelocityGame game, obj o)
		{
			o.level = this;

			o.loadTexture(game.Content);

			if (o.takesControls)
			{
				LeftDown += new KeyEventHandler(o.leftDown);
				LeftPressed += new KeyEventHandler(o.leftPressed);
				LeftReleased += new KeyEventHandler(o.leftReleased);
				RightDown += new KeyEventHandler(o.rightDown);
				RightPressed += new KeyEventHandler(o.rightPressed);
				RightReleased += new KeyEventHandler(o.rightReleased);
				UpDown += new KeyEventHandler(o.upDown);
				UpPressed += new KeyEventHandler(o.upPressed);
				UpReleased += new KeyEventHandler(o.upReleased);
				DownDown += new KeyEventHandler(o.downDown);
				DownPressed += new KeyEventHandler(o.downPressed);
				DownReleased += new KeyEventHandler(o.downReleased);

				ShiftDown += new KeyEventHandler(o.shiftDown);
				ShiftPressed += new KeyEventHandler(o.shiftPressed);
				ShiftReleased += new KeyEventHandler(o.shiftReleased);
				ClearDown += new KeyEventHandler(o.clearDown);
				ClearPressed += new KeyEventHandler(o.clearPressed);
				ClearReleased += new KeyEventHandler(o.clearReleased);

				N1Down += new KeyEventHandler(o.n1Down); N1Pressed += new KeyEventHandler(o.n1Pressed); N1Released += new KeyEventHandler(o.n1Released);
				N2Down += new KeyEventHandler(o.n2Down); N2Pressed += new KeyEventHandler(o.n2Pressed); N2Released += new KeyEventHandler(o.n2Released);
				N3Down += new KeyEventHandler(o.n3Down); N3Pressed += new KeyEventHandler(o.n3Pressed); N3Released += new KeyEventHandler(o.n3Released);
				N4Down += new KeyEventHandler(o.n4Down); N4Pressed += new KeyEventHandler(o.n4Pressed); N4Released += new KeyEventHandler(o.n4Released);
				N5Down += new KeyEventHandler(o.n5Down); N5Pressed += new KeyEventHandler(o.n5Pressed); N5Released += new KeyEventHandler(o.n5Released);

				LMDown += new MouseEventHandler(o.lmDown);
				LMPressed += new MouseEventHandler(o.lmPressed);
				LMReleased += new MouseEventHandler(o.lmReleased);
				RMDown += new MouseEventHandler(o.rmDown);
				RMPressed += new MouseEventHandler(o.rmPressed);
				RMReleased += new MouseEventHandler(o.rmReleased);
			}
			return o;
		}

		protected obj removeObj(obj o)
		{
			objs.Remove(o);
			return deInitializeObj(o);
		}

		public obj removeNewObj(obj o)
		{
			objsToRemove.Add(o);
			return o;
		}

		protected obj deInitializeObj(obj o)
		{
			if (o.takesControls)
			{
				LeftDown -= new KeyEventHandler(o.leftDown);
				LeftPressed -= new KeyEventHandler(o.leftPressed);
				LeftReleased -= new KeyEventHandler(o.leftReleased);
				RightDown -= new KeyEventHandler(o.rightDown);
				RightPressed -= new KeyEventHandler(o.rightPressed);
				RightReleased -= new KeyEventHandler(o.rightReleased);
				UpDown -= new KeyEventHandler(o.upDown);
				UpPressed -= new KeyEventHandler(o.upPressed);
				UpReleased -= new KeyEventHandler(o.upReleased);
				DownDown -= new KeyEventHandler(o.downDown);
				DownPressed -= new KeyEventHandler(o.downPressed);
				DownReleased -= new KeyEventHandler(o.downReleased);

				ShiftDown -= new KeyEventHandler(o.shiftDown);
				ShiftPressed -= new KeyEventHandler(o.shiftPressed);
				ShiftReleased -= new KeyEventHandler(o.shiftReleased);
				ClearDown -= new KeyEventHandler(o.clearDown);
				ClearPressed -= new KeyEventHandler(o.clearPressed);
				ClearReleased -= new KeyEventHandler(o.clearReleased);

				N1Down -= new KeyEventHandler(o.n1Down); N1Pressed += new KeyEventHandler(o.n1Pressed); N1Released += new KeyEventHandler(o.n1Released);
				N2Down -= new KeyEventHandler(o.n2Down); N2Pressed += new KeyEventHandler(o.n2Pressed); N2Released += new KeyEventHandler(o.n2Released);
				N3Down -= new KeyEventHandler(o.n3Down); N3Pressed += new KeyEventHandler(o.n3Pressed); N3Released += new KeyEventHandler(o.n3Released);
				N4Down -= new KeyEventHandler(o.n4Down); N4Pressed += new KeyEventHandler(o.n4Pressed); N4Released += new KeyEventHandler(o.n4Released);
				N5Down -= new KeyEventHandler(o.n5Down); N5Pressed += new KeyEventHandler(o.n5Pressed); N5Released += new KeyEventHandler(o.n5Released);

				LMDown -= new MouseEventHandler(o.lmDown);
				LMPressed -= new MouseEventHandler(o.lmPressed);
				LMReleased -= new MouseEventHandler(o.lmReleased);
				RMDown -= new MouseEventHandler(o.rmDown);
				RMPressed -= new MouseEventHandler(o.rmPressed);
				RMReleased -= new MouseEventHandler(o.rmReleased);
			}
			return o;
		}

		#endregion Object Manipulation

		protected static int CompareDepths(obj a, obj b)
		{
			//Doesn't take null objs into account
			if (a.depth == b.depth)
				return 0;
			if (a.depth < b.depth)
				return 1;
			return -1;
		}

		public List<obj> getPlayers()
		{
			List<obj> players = new List<obj>();
			foreach (obj o in objs)
				if (o.objType == "Player")
					players.Add(o);
			return players;
		}

		#region Collision Functions

		public bool collidesSolid(obj o)
		{
			foreach (obj j in objs)
			{
				if (o == j)
					continue;	//Don't collide with self of course
				//Check for collision between these two
				if (BB.collides(o.bb, j.bb) && j.isSolid)
					return true;
			}
			return false;
		}

		public List<obj> collisionList(obj i, bool solid)
		{
			List<obj> colls = new List<obj>();

			foreach (obj j in objs)
			{
				if (i == j)
					continue;	//Don't collide with self
				if (solid && (!j.isSolid))
					continue;	//Ignore if only solids requested
				if (BB.collides(i.bb, j.bb))
					colls.Add(j);
			}

			return colls;
		}

		public List<obj> collisionListAtRelative(obj i, float atx, float aty, bool solid)
		{
			List<obj> colls = new List<obj>();

			BB bbAt = i.bb + new Vector2(atx, aty);

			foreach (obj j in objs)
			{
				if (i == j)
					continue;	//Don't collide with self
				if (solid && (!j.isSolid))
					continue;	//Ignore if only solids requested
				if (BB.collides(bbAt, j.bb))
					colls.Add(j);
			}

			return colls;
		}

		public List<obj> collisionListAlongLine(float x1, float y1, float x2, float y2, bool solid)
		{
			List<obj> colls = new List<obj>();

			BB bb = new BB(x1, y1, x2, y2, "line");

			foreach (obj o in objs)
			{
				if (solid && (!o.isSolid))
					continue;
				if (BB.collides(o.bb, bb))
					colls.Add(o);
			}

			return colls;
		}

		public List<obj> collisionListAtPoint(float atx, float aty, bool solid)
		{
			List<obj> colls = new List<obj>();

			BB bb = new BB(atx, aty, 1);

			foreach (obj j in objs)
			{
				if (solid && (!j.isSolid))
					continue;	//Ignore if only solids requested
				if (BB.collides(bb, j.bb))
					colls.Add(j);
			}

			return colls;
		}

		#endregion Collision Functions

		public void Quit(VelocityGame game)
		{
			game.Exit();
		}

		public void WaitThenRestart(float waitTime)
		{
			restartTime = gameTicks + waitTime;
			restartState = 'r';
		}
	}

	public class Camera
	{
		public Vector2 XY;
		public float zoom;	//Times zoomed OUT
		public float moveSpeed = 3f;
		public float zoomSpeed = .5f;
		public float base_width, base_height;

		public Camera(float _x, float _y, float _zoom, int gameWidth, int gameHeight)
		{
			XY = new Vector2(_x, _y);
			zoom = _zoom;
			base_width = gameWidth; base_height = gameHeight;
		}

		public void Zoom(float z)
		{
			float cx = XY.X + base_width * .5f * zoom;
			float cy = XY.Y + base_height * .5f * zoom;
			zoom *= z;
			XY.X = cx - base_width * .5f * zoom;
			XY.Y = cy - base_height * .5f * zoom;
		}

		public void moveLeft(float amt) { XY.X -= moveSpeed * zoom * amt; }
		public void moveRight(float amt) { XY.X += moveSpeed * zoom * amt; }
		public void moveUp(float amt) { XY.Y -= moveSpeed * zoom * amt; }
		public void moveDown(float amt) { XY.Y += moveSpeed * zoom * amt; }
	}
}
