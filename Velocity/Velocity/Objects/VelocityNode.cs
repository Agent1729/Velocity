using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Velocity.Objects
{
	public class VelocityNode : VelocityObj
	{
		public VelocityZone vz;
		public obj stuckTo;
		protected Player plr;
		protected float shotSpeed = 10;
		protected float resize = 10;

		public VelocityNode(float _x, float _y)
			: base(_x, _y)
		{ }

		public virtual void unStick()
		{
			stuckTo = null;
			if (vz != null)
				vz.Clear();
			vz = null;
		}
	}
}
