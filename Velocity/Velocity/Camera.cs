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
