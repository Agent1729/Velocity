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
	public static class FontManager
	{
		public static ContentManager content;
		public static List<MySpriteFont> spriteFonts;

		public static void init(ContentManager _content)
		{
			content = _content;
			spriteFonts = new List<MySpriteFont>();
		}

		public static SpriteFont getFont(string name)
		{
			foreach (MySpriteFont s in spriteFonts)
				if (s.name == name)
					return s.spriteFont;
			
			//Not loaded
			MySpriteFont newS = new MySpriteFont();
			newS.init(content, name);
			return newS.spriteFont;
		}

		public class MySpriteFont
		{
			public string name;
			public SpriteFont spriteFont;

			public void init(ContentManager content, string _name)
			{
				name = _name;
				spriteFont = content.Load<SpriteFont>(name);
			}
		}
	}
}
