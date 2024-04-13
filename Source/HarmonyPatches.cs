using HarmonyLib;
using PeteTimesSix.SimpleSidearms;
using PeteTimesSix.SimpleSidearms.Intercepts;
using PeteTimesSix.SimpleSidearms.Utilities;
using RimWorld;
using SimpleSidearms.rimworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static PeteTimesSix.SimpleSidearms.Utilities.Enums;

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
				&& SwitchWeaponsUtility.IsSwitchable(pawn)
				&& !pawn.WorkTagIsDisabled(WorkTags.Violent))
			{
				return new Gizmo_SwitchWeapon(pawn)
				{
					Disabled = pawn.Downed,
					disabledReason = "SSSW_Downed".Translate(),
				};
			}
			return null;
		}
	}

	[HarmonyPatch(typeof(AutoUndrafter_AutoUndraftTick_Postfix), "AutoUndraftTick")]
	static class AutoUndrafter_AutoUndraftTick_Postfix_AutoUndraftTick
	{
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator generator)
		{
			var patched = false;
			var list = codeInstructions.ToList();
			//Log.Message($"mmmmmmmmmm BEFORE\n{string.Join("\n", list)}");
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i + 0].opcode == OpCodes.Callvirt && list[i].operand is MethodInfo mi && mi == AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Map))
					&& list[i + 1].opcode == OpCodes.Brfalse_S)
				{
					var label = list[i + 1].operand;
					list.Insert(i++, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SwitchWeaponsUtility), nameof(SwitchWeaponsUtility.PreventUndraftTickWeaponSwitch))));
					list.Insert(i++, new CodeInstruction(OpCodes.Brtrue_S, label));
					list.Insert(i++, new CodeInstruction(OpCodes.Ldloc_0));
					
					patched = true;
					break;
				}
			}
			//Log.Message($"mmmmmmmmmm AFTER\n{string.Join("\n", list)}");
			if (!patched)
				Log.Error($"{nameof(SwitchWeapons)}: {nameof(AutoUndrafter_AutoUndraftTick_Postfix_AutoUndraftTick)} patch failed");
			return list;
		}
	}

	[HarmonyPatch(typeof(JobGiver_QuicklySwitchWeapons), "TryGiveJobStatic")]
	static class JobGiver_QuicklySwitchWeapons_TryGiveJobStatic
	{
		public static bool Prefix(Pawn pawn)
		{
			// prevent switching to prefered when in guard mode
			return !SwitchWeaponsUtility.IsGuard(pawn);
		}
	}
}
