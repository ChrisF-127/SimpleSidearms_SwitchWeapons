using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SwitchWeapons
{
	public static class Utility
	{
		public static bool IsNonLethal(this ThingWithComps weapon) =>
			weapon?.def?.weaponTags?.Any(s => string.Compare(s, "NonLethal", true) == 0) == true;
	}
}
