using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SwitchWeapons
{
	[StaticConstructorOnStartup]
	public static class TextureResources
	{
		public static readonly Texture2D Background = ContentFinder<Texture2D>.Get("sssw_background", true);

		public static readonly Texture2D ForceRanged = ContentFinder<Texture2D>.Get("sssw_ranged", true);
		public static readonly Texture2D ForceMelee = ContentFinder<Texture2D>.Get("sssw_melee", true);
		public static readonly Texture2D ForceUnarmed = ContentFinder<Texture2D>.Get("sssw_unarmed", true);
		public static readonly Texture2D Disable = ContentFinder<Texture2D>.Get("sssw_disable", true);

		public static readonly Texture2D LongRange = ContentFinder<Texture2D>.Get("sssw_longrange", true);
		public static readonly Texture2D MediumRange = ContentFinder<Texture2D>.Get("sssw_mediumrange", true);
		public static readonly Texture2D ShortRange = ContentFinder<Texture2D>.Get("sssw_shortrange", true);

		public static readonly Texture2D Dangerous = ContentFinder<Texture2D>.Get("sssw_dangerous", true);
		public static readonly Texture2D EMP = ContentFinder<Texture2D>.Get("sssw_emp", true);

		public static readonly Texture2D Next = ContentFinder<Texture2D>.Get("sssw_next", true);
		public static readonly Texture2D Previous = ContentFinder<Texture2D>.Get("sssw_previous", true);
	}
}
