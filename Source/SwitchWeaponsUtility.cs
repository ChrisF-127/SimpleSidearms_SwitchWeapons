using aRandomKiwi.GFM;
using PeteTimesSix.SimpleSidearms.Utilities;
using PeteTimesSix.SimpleSidearms;
using SimpleSidearms.rimworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static PeteTimesSix.SimpleSidearms.Utilities.Enums;

namespace SwitchWeapons
{
	internal static class SwitchWeaponsUtility
	{
		internal static bool IsDrafted(this Pawn pawn) =>
			pawn?.Drafted == true;
		internal static bool IsGuard(this Pawn pawn)
		{
			Log.Message($"{pawn} IsGuard {pawn?.TryGetComp<Comp_Guard>()?.isActiveGuard() == true}");
#warning TODO check for dll
			return pawn?.TryGetComp<Comp_Guard>()?.isActiveGuard() == true;
		}

		internal static bool IsSwitchable(this Pawn pawn) =>
			IsDrafted(pawn) || IsGuard(pawn);


		internal static bool SwitchToRanged(this Pawn pawn, CompSidearmMemory memory)
		{
			// Switch to default weapon
			if (memory.DefaultRangedWeapon is ThingDefStuffDefPair pair
				&& pawn.SwitchTo(pair))
				return true;

			// Switch to best ranged weapon
			if (GettersFilters.findBestRangedWeapon(pawn, null, true, PeteTimesSix.SimpleSidearms.SimpleSidearms.Settings.SkipDangerousWeapons, true).weapon is ThingWithComps thing
				&& pawn.SwitchTo(thing))
				return true;

			// Switch failed
			return false;
		}
		internal static bool SwitchToMelee(this Pawn pawn, CompSidearmMemory memory)
		{
			// Switch to preferred melee weapon
			if (memory.PreferredMeleeWeapon is ThingDefStuffDefPair pair
				&& pawn.SwitchTo(pair))
				return true;

			// Switch to best melee weapon
			if (GettersFilters.findBestMeleeWeapon(pawn, out var thing, pawn.equipment.Primary?.def.IsRangedWeapon != true, false)
				&& pawn.SwitchTo(thing))
				return true;

			// Switch failed
			return false;
		}

		internal static bool SwitchToSelected(this Pawn pawn)
		{
			if (pawn.TryGetComp<CompSwitchWeapon>() is CompSwitchWeapon switchWeapon 
				&& switchWeapon.SelectedWeapon is ThingDefStuffDefPair pair)
			{
				if (!pawn.IsSwitchable())
				{
					Log.Message($"{pawn} Clear");
					switchWeapon.SelectedWeapon = null;
					return false;
				}
				Log.Message($"{pawn} {pawn.equipment.Primary} {pair}");
				return pawn.equipment.Primary?.toThingDefStuffDefPair() == pair
					|| WeaponAssingment.equipSpecificWeaponTypeFromInventory(pawn, pair, false, false);
			}
			return false;
		}

		internal static bool SwitchTo(this Pawn pawn, ThingDefStuffDefPair pair)
		{
			var output = pawn.equipment.Primary?.toThingDefStuffDefPair() == pair
				|| WeaponAssingment.equipSpecificWeaponTypeFromInventory(pawn, pair, pawn.ShouldFumbleDrop(), false);
			if (output && pawn.TryGetComp<CompSwitchWeapon>() is CompSwitchWeapon switchWeapon)
				switchWeapon.SelectedWeapon = pair;
			return output;
		}
		internal static bool SwitchTo(this Pawn pawn, ThingWithComps thing)
		{
			var output = pawn.equipment.Primary == thing
				|| WeaponAssingment.equipSpecificWeapon(pawn, thing, pawn.ShouldFumbleDrop(), false);
			if (output && thing != null && pawn.TryGetComp<CompSwitchWeapon>() is CompSwitchWeapon switchWeapon)
				switchWeapon.SelectedWeapon = thing.toThingDefStuffDefPair();
			return output;
		}

		internal static bool ShouldFumbleDrop(this Pawn pawn) =>
			MiscUtils.shouldDrop(pawn, DroppingModeEnum.Combat, false);


		internal static bool PreventUndraftTickWeaponSwitch(Pawn pawn)
		{
			// Get SwitchWeapon comp
			var compSwitch = pawn.TryGetComp<CompSwitchWeapon>();
			if (compSwitch == null)
				return false;

			// Only allow switching if no selected weapon
#warning TODO only switch if "idle"
			return pawn.SwitchToSelected();
		}
	}
}
