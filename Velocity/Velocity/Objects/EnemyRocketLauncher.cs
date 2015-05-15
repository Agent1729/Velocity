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
	public class EnemyRocketLauncher : VelocityObj
	{
		public float gunx, guny;
		private float cooldownTime = 0, cooldownResetTime = 120;
		private float chargeTime = 0, chargeUpTime = 30;
		private float fireSpeed = 8;
		private obj fireAt = null;

		public EnemyRocketLauncher(float _x, float _y)
			: base(_x, _y)
		{
			gunx = x;
			guny = y;
		}

		protected override void init()
		{
			objType = "EnemyRocketLauncher";
			mainSpriteName = "RocketLauncher";
			base_width = 16; base_height = 16;
			width = base_width; height = base_height;
			depth = 5;
			collisionStatic = true;
			isSolid = true;
			takesControls = false;
			canBePushed = false;
			hasGravity = false;
			canAbsorb = false;
			hasBeenFrictioned = false;

			base.init();
		}

		protected override void dotick()
		{
			base.dotick();

			if (cooldownTime > 0)
				cooldownTime -= factor;
			else
				if (fireAt == null)
					tryFire();

			if (fireAt != null)
			{
				if (chargeTime > 0)
					chargeTime -= factor;
				else
					fire();
			}

			if (cooldownTime > cooldownResetTime)
				cooldownTime = cooldownResetTime;
			if (chargeTime > chargeUpTime)
				chargeTime = chargeUpTime;
		}

		protected void tryFire()
		{
			List<obj> players = level.getPlayers();
			if (players.Count == 0)
				return;

			List<obj> playerCanFireAt = new List<obj>();
			foreach (obj p in players)
			{
				List<obj> colls = level.collisionListAlongLine(gunx, guny, p.x, p.y, true);
				bool collFound = false;
				foreach (obj o in colls)
				{
					if ((o.objType != "Player")&&(o.objType!="EnemyRocketLauncher"))
					{
						collFound = true;
						break;
					}
				}
				if (!collFound)
					playerCanFireAt.Add(p);
			}
			if (playerCanFireAt.Count == 0)
				return;

			Random r = new Random();
			int R = r.Next(playerCanFireAt.Count);
			fireAt = playerCanFireAt[R];
			chargeTime = chargeUpTime;
		}

		protected void fire()
		{
			float dist = BB.dist(gunx, guny, fireAt.x, fireAt.y);
			float scale = dist / fireSpeed;
			float xspd = (fireAt.x - gunx) / scale;
			float yspd = (fireAt.y - guny) / scale;

			//Create new rocket to fire at fireAt
			Rocket rocket = new Rocket(gunx, guny, xspd, yspd, fireAt);
			level.addNewObj(rocket);
			cooldownTime = cooldownResetTime;
			fireAt = null;
		}
	}
}