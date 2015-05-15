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
	public static class SpriteManager
	{
		public static ContentManager content;
		public static List<MySprite> sprites;

		public static void init(ContentManager _content)
		{
			content = _content;
			sprites = new List<MySprite>();
		}

		public static Texture2D getSprite(string name)
		{
			foreach (MySprite s in sprites)
				if (s.name == name)
					return s.sprite;
			
			//Not loaded
			MySprite newS = new MySprite();
			newS.init(content, name);
			return newS.sprite;
		}

		public class MySprite
		{
			public string name;
			public Texture2D sprite;

			public void init(ContentManager content, string _name)
			{
				name = _name;
				sprite = content.Load<Texture2D>(name);
			}
		}
	}
}
