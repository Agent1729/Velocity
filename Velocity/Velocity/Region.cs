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
	public class Region
	{
		public List<obj> objs;
		public List<Region_objAddRem> objsChanges;
		public float l, r, u, d;

		public Region(float x, float y, float w, float h)
		{
			objs = new List<obj>();
			objsChanges = new List<Region_objAddRem>();
			l = x;
			u = y;
			r = x + w;
			d = y + h;
		}

		public void add(obj o)
		{
			objsChanges.Add(new Region_objAddRem(o, false));
		}

		public void remove(obj o)
		{
			objsChanges.Add(new Region_objAddRem(o, true));
		}

		public void tick()
		{
			foreach (Region_objAddRem ro in objsChanges)
			{
				if(ro.toRemove)
				{
					if (objs.Contains(ro.o))
						objs.Remove(ro.o);
				}
				else
				{
					objs.Add(ro.o);
				}
			}
			objsChanges.Clear();
		}

		public struct Region_objAddRem
		{
			public obj o;
			public bool toRemove;

			public Region_objAddRem(obj _o, bool _remove)
			{
				o = _o;
				toRemove = _remove;
			}
		}
	}

	/*		//Backup
	public class Region
	{
		public List<obj> objs;
		public List<obj> objsToAdd;
		public List<obj> objsToRemove;
		public float l, r, u, d;

		public Region(float x, float y, float w, float h)
		{
			objs = new List<obj>();
			objsToAdd = new List<obj>();
			objsToRemove = new List<obj>();
			l = x;
			u = y;
			r = x + w;
			d = y + h;
		}

		public void add(obj o)
		{
			if (!objs.Contains(o))
				objs.Add(o);
			if (!objsToAdd.Contains(o))
				objsToAdd.Add(o);
		}

		public void remove(obj o)
		{
			if (!objsToRemove.Contains(o))
				objsToRemove.Add(o);
		}

		public void tick()
		{
			foreach (obj o in objsToAdd)
			{
				if (objsToRemove.Contains(o))
					objsToRemove.Remove(o);
			}
			foreach (obj o in objsToRemove)
			{
				if (objs.Contains(o))
					objs.Remove(o);
			}
			objsToAdd.Clear();
			objsToRemove.Clear();
		}
	}//*/
}
