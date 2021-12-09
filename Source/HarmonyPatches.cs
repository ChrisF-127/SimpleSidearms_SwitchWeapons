using HarmonyLib;
using PeteTimesSix.SimpleSidearms;
using PeteTimesSix.SimpleSidearms.Utilities;
using RimWorld;
using SimpleSidearms.rimworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SwitchWeapons
{
	[StaticConstructorOnStartup]
	public static class HarmonyPatches
	{
		static HarmonyPatches()
		{
			Harmony harmony = new Harmony("syrus.simplesidearms_switchweapons");

			harmony.PatchAll();
		}
	}

	[HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
	static class Pawn_DraftController_GetGizmos
	{
		[HarmonyPriority(0)]
		public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn_DraftController __instance)
		{
			var pawn = __instance?.pawn;
			if (pawn != null
				&& pawn.Faction?.IsPlayer == true
				&& pawn.Drafted
				&& !pawn.WorkTagIsDisabled(WorkTags.Violent))
			{
				yield return new Gizmo_SwitchWeapon(pawn)
				{
					disabled = pawn.Downed,
					disabledReason = "SSSW_Downed".Translate(),
				};
			}

			foreach (var gizmo in __result)
				yield return gizmo;
		}
	}
}
