using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SwitchWeapons
{
	[DefOf]
	internal static class SWKeyBindingDefOf
	{
#pragma warning disable CS0649 // "never assigned to"
		public static KeyBindingDef SSSW_Ranged;
		public static KeyBindingDef SSSW_Melee;
		public static KeyBindingDef SSSW_Disable;
		public static KeyBindingDef SSSW_Unarmed;

		public static KeyBindingDef SSSW_LongRange;
		public static KeyBindingDef SSSW_MediumRange;
		public static KeyBindingDef SSSW_ShortRange;

		public static KeyBindingDef SSSW_Dangerous;
		public static KeyBindingDef SSSW_EMP;

		public static KeyBindingDef SSSW_Next;
		public static KeyBindingDef SSSW_Previous;
#pragma warning restore CS0649
	}
}
