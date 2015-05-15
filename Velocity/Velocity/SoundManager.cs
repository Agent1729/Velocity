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
	public static class SoundManager
	{
		public static ContentManager content;
		public static List<MySoundEffect> soundEffects;

		public static void init(ContentManager _content)
		{
			content = _content;
			soundEffects = new List<MySoundEffect>();
		}

		public static SoundEffect getSound(string name)
		{
			foreach (MySoundEffect s in soundEffects)
				if (s.name == name)
					return s.soundEffect;

			//Not loaded
			MySoundEffect newS = new MySoundEffect();
			newS.init(content, name);
			return newS.soundEffect;
		}

		public class MySoundEffect
		{
			public string name;
			public SoundEffect soundEffect;

			public void init(ContentManager content, string _name)
			{
				name = _name;
				soundEffect = content.Load<SoundEffect>(name);
			}
		}
	}
}
