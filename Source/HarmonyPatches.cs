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
	[HarmonyPatch(typeof(Pawn_DraftController), "GetGizmos")]
	static class Pawn_DraftController_GetGizmos
	{
		[HarmonyPriority(Priority.Last)]
		public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn_DraftController __instance)
		{
			var switchWeaponGizmo = CreateSwitchWeaponGizmo(__instance?.pawn);

			// no gizmo to show
			if (switchWeaponGizmo == null)
			{
				foreach (var gizmo in __result)
					yield return gizmo;
			}
			// show gizmo after SimpleSidearms (hopefully)
			else if (!SwitchWeapons.ShowAfterDraftToggle)
			{
				yield return switchWeaponGizmo;

				foreach (var gizmo in __result)
					yield return gizmo;
			}
			// show gizmo after draft-toggle
			else
			{
				foreach (var gizmo in __result)
				{
					yield return gizmo;

					// check groupKey for draft/undraft; see Pawn_DraftController.GetGizmos
					//  could also use "defaultDesc = "CommandToggleDraftDesc".Translate()" or "icon = TexCommand.Draft"
					if (switchWeaponGizmo != null
						&& gizmo is Command_Toggle cmd
						&& cmd.groupKeyIgnoreContent == 81729172)
					{
						yield return switchWeaponGizmo;
						switchWeaponGizmo = null;
					}
				}

				// make sure the gizmo shows if for some reason it didn't show beforehand (will show last)
				if (switchWeaponGizmo != null)
					yield return switchWeaponGizmo;
			}
		}

		private static Gizmo_SwitchWeapon CreateSwitchWeaponGizmo(Pawn pawn)
		{
			if (pawn != null
				&& pawn.Faction?.IsPlayer == true
				&& pawn.Drafted
				&& !pawn.WorkTagIsDisabled(WorkTags.Violent))
			{
				return new Gizmo_SwitchWeapon(pawn)
				{
					disabled = pawn.Downed,
					disabledReason = "SSSW_Downed".Translate(),
				};
			}
			return null;
		}
	}
}
