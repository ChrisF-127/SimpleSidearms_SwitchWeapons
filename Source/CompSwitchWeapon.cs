using PeteTimesSix.SimpleSidearms;
using PeteTimesSix.SimpleSidearms.Utilities;
using RimWorld;
using SimpleSidearms.rimworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SwitchWeapons
{
	public class CompSwitchWeapon : ThingComp
	{
		public bool ForceSwitchedToRanged = false;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (parent is Pawn pawn && pawn.RaceProps?.Humanlike != true)
				pawn.AllComps.Remove(this);
		}

		public override void PostExposeData()
		{
			Scribe_Values.Look(ref ForceSwitchedToRanged, nameof(ForceSwitchedToRanged));
		}

		public static bool IsDraftedMeleeForcedRanged(Pawn pawn)
		{
			CompSwitchWeapon compSwitch = null;

			// Must be drafted
			if (pawn?.Drafted != true)
				goto FALSE;

			// Get SwitchWeapon comp
			compSwitch = pawn.TryGetComp<CompSwitchWeapon>();

			// Must be force switched to ranged
			if (compSwitch?.ForceSwitchedToRanged != true)
				goto FALSE;

			// Must be carrying ranged weapon
			if (pawn.equipment?.Primary?.def?.IsRangedWeapon != true)
				goto FALSE;

			// Get Memory
			var compMemory = pawn.TryGetComp<CompSidearmMemory>();
			if (compMemory == null)
				goto FALSE;
			// Must default to melee or by-skill and prefer melee
			if (!(compMemory.primaryWeaponMode == Enums.PrimaryWeaponMode.Melee
				|| compMemory.primaryWeaponMode == Enums.PrimaryWeaponMode.BySkill && pawn.getSkillWeaponPreference() == Enums.PrimaryWeaponMode.Melee))
				goto FALSE;

			//Log.Message($"{pawn.Name} true");
			return true;

FALSE:
			if (compSwitch != null)
				compSwitch.ForceSwitchedToRanged = false;
			//Log.Message($"{pawn.Name} false");
			return false;
		}
	}


	public class CompProperties_SwitchWeapon : CompProperties
	{
		public CompProperties_SwitchWeapon()
		{
			compClass = typeof(CompSwitchWeapon);
		}
	}
}
