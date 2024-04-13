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
		public ThingDefStuffDefPair? SelectedWeapon = null;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (parent is Pawn pawn && pawn.RaceProps?.Humanlike != true)
				pawn.AllComps.Remove(this);
		}

		public override void PostExposeData()
		{
			Scribe_Deep.Look(ref SelectedWeapon, nameof(SelectedWeapon));
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
