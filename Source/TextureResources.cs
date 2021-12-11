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
		public static readonly Texture2D Background = ContentFinder<Texture2D>.Get("background", true);

		public static readonly Texture2D ForceRanged = ContentFinder<Texture2D>.Get("ranged", true);
		public static readonly Texture2D ForceMelee = ContentFinder<Texture2D>.Get("melee", true);
		public static readonly Texture2D ForceUnarmed = ContentFinder<Texture2D>.Get("unarmed", true);
		public static readonly Texture2D Disable = ContentFinder<Texture2D>.Get("disable", true);

		public static readonly Texture2D Next = ContentFinder<Texture2D>.Get("next", true);
		public static readonly Texture2D Previous = ContentFinder<Texture2D>.Get("previous", true);
	}
}
