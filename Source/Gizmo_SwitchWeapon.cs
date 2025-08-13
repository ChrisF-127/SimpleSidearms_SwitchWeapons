using PeteTimesSix.SimpleSidearms;
using PeteTimesSix.SimpleSidearms.Utilities;
using RimWorld;
using SimpleSidearms.rimworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using static PeteTimesSix.SimpleSidearms.Utilities.Enums;

namespace SwitchWeapons
{
	public class Gizmo_SwitchWeapon : Command
	{
		private enum SwitchButtonEnum
		{
			None,

			Ranged,
			Melee,
			Disable,
			Unarmed,

			LongRange,
			MediumRange,
			ShortRange,

			Dangerous,
			EMP,

			Next,
			Previous,
		}

		private enum SortByEnum
		{
			MarketValue = 0,
			Range = 1,
		}

		#region FIELDS
		private const float _iconGap = 1f;
		private const float _iconSize = 32f;

		private Color _baseColor = new Color(0.5f, 0.5f, 0.5f);
		private Color _highlightColor = new Color(1.0f, 1.0f, 1.0f);

		private const float _hotKeyLabelRectMarginX = 3f;
		private const float _hotKeyLabelRectMarginY = 1f;

		private readonly Pawn _pawn = null;

		// must be static as the gizmo is constantly recreated
		private static float _columns = 1;
		#endregion

		#region CONSTRUCTORS
		public Gizmo_SwitchWeapon(Pawn pawn)
		{
			_pawn = pawn;
			defaultLabel = "SSSW_SwitchWeaponsLabel".Translate();
			defaultDesc = "SSSW_SwitchWeaponsDesc".Translate();
			shrinkable = false;
		}
		#endregion

		#region OVERRIDES
		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			if (_pawn == null)
			{
				Log.Error($"SimpleSidearms_SwitchWeapons: {nameof(Gizmo_SwitchWeapon)}.{nameof(GizmoOnGUI)}: {nameof(_pawn)} was null!");
				return new GizmoResult(GizmoState.Clear);
			}

			var gizmoRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
			Widgets.DrawWindowBackground(gizmoRect);

			var gridColumns = 0;
			var gridTopLeft = new Vector2(topLeft.x + 4, topLeft.y + 2);
			var interaction = SwitchButtonEnum.None;

			var oriFont = Text.Font;
			Text.Font = GameFont.Tiny;
			if (DrawButton(calcGridRect(0, 0), TextureResources.ForceRanged, new Color(0.4f, 0.8f, 0.4f), "SSSW_Ranged", SWKeyBindingDefOf.SSSW_Ranged))
				interaction = SwitchButtonEnum.Ranged;
			if (DrawButton(calcGridRect(0, 1), TextureResources.ForceMelee, new Color(0.8f, 0.4f, 0.4f), "SSSW_Melee", SWKeyBindingDefOf.SSSW_Melee))
				interaction = SwitchButtonEnum.Melee;
			if (DrawButton(calcGridRect(1, 1), TextureResources.ForceUnarmed, new Color(0.8f, 0.6f, 0.4f), "SSSW_Unarmed", SWKeyBindingDefOf.SSSW_Unarmed))
				interaction = SwitchButtonEnum.Unarmed;

			int rangeCount = 0;
			if (SwitchWeapons.Settings.ShowLongRangeSwitch)
				rangeCount++;
			if (SwitchWeapons.Settings.ShowMediumRangeSwitch)
				rangeCount++;
			if (SwitchWeapons.Settings.ShowShortRangeSwitch)
				rangeCount++;
			int deCount = 0;
			if (SwitchWeapons.Settings.ShowDangerousSwitch)
				deCount++;
			if (SwitchWeapons.Settings.ShowEMPSwitch)
				deCount++;

			var defaultOnRow1 = rangeCount > 1 && deCount == 0;
			var dangerOnRow0 = rangeCount == 0;

			var column = defaultOnRow1 ? 1 : 2;
			if (SwitchWeapons.Settings.ShowLongRangeSwitch)
			{
				if (DrawButton(calcGridRect(column++, 0), TextureResources.LongRange, new Color(0.8f, 0.8f, 0.4f), "SSSW_LongRange", SWKeyBindingDefOf.SSSW_LongRange))
					interaction = SwitchButtonEnum.LongRange;
			}
			if (SwitchWeapons.Settings.ShowMediumRangeSwitch)
			{
				if (DrawButton(calcGridRect(column++, 0), TextureResources.MediumRange, new Color(0.8f, 0.6f, 0.4f), "SSSW_MediumRange", SWKeyBindingDefOf.SSSW_MediumRange))
					interaction = SwitchButtonEnum.MediumRange;
			}
			if (SwitchWeapons.Settings.ShowShortRangeSwitch)
			{
				if (DrawButton(calcGridRect(column++, 0), TextureResources.ShortRange, new Color(0.8f, 0.4f, 0.4f), "SSSW_ShortRange", SWKeyBindingDefOf.SSSW_ShortRange))
					interaction = SwitchButtonEnum.ShortRange;
			}

			var row = dangerOnRow0 ? 0 : 1;
			if (SwitchWeapons.Settings.ShowDangerousSwitch)
			{
				if (DrawButton(calcGridRect(2, row), TextureResources.Dangerous, new Color(0.8f, 0.4f, 0.4f), "SSSW_Dangerous", SWKeyBindingDefOf.SSSW_Dangerous))
					interaction = SwitchButtonEnum.Dangerous;
			}
			column = !dangerOnRow0 && deCount > 1 ? 3 : 2;
			if (SwitchWeapons.Settings.ShowEMPSwitch)
			{
				if (DrawButton(calcGridRect(column, 1), TextureResources.EMP, new Color(0.4f, 0.8f, 0.8f), "SSSW_EMP", SWKeyBindingDefOf.SSSW_EMP))
					interaction = SwitchButtonEnum.EMP;
			}

			column = defaultOnRow1 ? 2 : 1;
			row = defaultOnRow1 ? 1 : 0;
			if (DrawButton(calcGridRect(column, row), TextureResources.Disable, new Color(0.8f, 0.8f, 0.8f), "SSSW_Disable", SWKeyBindingDefOf.SSSW_Disable))
				interaction = SwitchButtonEnum.Disable;

			if (SwitchWeapons.Settings.ShowPrevNextSwitch)
			{
				column = gridColumns;
				var nextPrevColor = new Color(0.8f, 0.8f, 0.8f);
				if (DrawButton(calcGridRect(column, 0), TextureResources.Next, nextPrevColor, "SSSW_Next", SWKeyBindingDefOf.SSSW_Next))
					interaction = SwitchButtonEnum.Next;
				if (DrawButton(calcGridRect(column, 1), TextureResources.Previous, nextPrevColor, "SSSW_Previous", SWKeyBindingDefOf.SSSW_Previous))
					interaction = SwitchButtonEnum.Previous;
			}

			DrawGizmoLabel("SSSW_Switch".Translate(), gizmoRect);

			Text.Font = oriFont;
			_columns = gridColumns;

			if (interaction != SwitchButtonEnum.None)
				return new GizmoResult(GizmoState.Interacted, new Event(Event.current) { button = (int)interaction });
			return new GizmoResult(GizmoState.Clear);

			Rect calcGridRect(int x, int y)
			{
				if (gridColumns <= x)
					gridColumns = x + 1; 
				return new Rect(
					gridTopLeft.x + (_iconGap + _iconSize) * x, 
					gridTopLeft.y + (_iconGap + _iconSize) * y, 
					_iconSize, 
					_iconSize);
			}
		}

		public override float GetWidth(float maxWidth) => 
			_iconSize * _columns + _iconGap * (_columns - 1) + 8;

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);

			var interaction = (SwitchButtonEnum)ev.button;
			if (interaction == SwitchButtonEnum.None)
				return;

			// Get Sidearm memory
			var memory = CompSidearmMemory.GetMemoryCompForPawn(_pawn);

			// Unset forced weapons
			memory.UnsetForcedWeapon(true);

			// Handle interaction
			switch (interaction)
			{
				case SwitchButtonEnum.Ranged:
					{
						// Disable out-of-combat forced weapon/unarmed
						var forcedWeapon = memory.ForcedWeapon;
						var forcedUnarmed = memory.ForcedUnarmed;
						if (forcedWeapon?.thing?.IsRangedWeapon == false)
							memory.ForcedWeapon = null;
						else if (forcedUnarmed)
							memory.ForcedUnarmed = false;

						// Switch to ranged
						if (!SwitchToRanged(memory))
							Log.Warning("Simple Sidearms - Switch Weapons: [Ranged] switching to melee failed");

						// Reenable out-of-combat forced weapon/unarmed
						memory.ForcedWeapon = forcedWeapon;
						memory.ForcedUnarmed = forcedUnarmed;

						// Remember that we forced a switch to ranged
						if (_pawn.TryGetComp<CompSwitchWeapon>() is CompSwitchWeapon switchWeapon)
							switchWeapon.ForceSwitchedToRanged = true;
					}
					break;
				case SwitchButtonEnum.Melee:
					{
						// Disable out-of-combat forced weapon/unarmed
						var forcedWeapon = memory.ForcedWeapon;
						if (forcedWeapon?.thing?.IsMeleeWeapon == false)
							memory.ForcedWeapon = null;

						// Switch to melee
						if (!SwitchToMelee(memory))
							Log.Warning("Simple Sidearms - Switch Weapons: [Melee] switching to melee failed");

						// Set the weapon as forced if it is a melee weapon
						var weapon = _pawn.equipment.Primary;
						if (weapon?.def?.IsMeleeWeapon == true)
							memory.SetWeaponAsForced(weapon.toThingDefStuffDefPair(), true);

						// Reenable out-of-combat forced weapon/unarmed
						memory.ForcedWeapon = forcedWeapon;
					}
					break;
				case SwitchButtonEnum.Disable:
					{
						// Switch according to preference
						// ranged
						if (UseRanged(memory))
						{
							if (!SwitchToRanged(memory))
								Log.Warning("Simple Sidearms - Switch Weapons: [Disable] switching to ranged failed");
						}
						// melee
						else if (!memory.PreferredUnarmed)
						{
							if (!SwitchToMelee(memory))
								Log.Warning("Simple Sidearms - Switch Weapons: [Disable] switching to melee failed");
						}
						// unarmed
						else
						{
							if (!SwitchTo(null))
								Log.Warning("Simple Sidearms - Switch Weapons: [Disable] switching to unarmed failed");
						}
					}
					break;
				case SwitchButtonEnum.Unarmed:
					{
						// Set unarmed as forced
						memory.SetUnarmedAsForced(true);

						// Switch to unarmed
						if (!SwitchTo(null))
							Log.Warning("Simple Sidearms - Switch Weapons: [Unarmed] switching to unarmed failed");
					}
					break;

				case SwitchButtonEnum.LongRange:
					{
						// Find highest DPS weapon for range
						var bestWeapon = SwitchWeapons.Settings.RangeUseLongestShortest ?
							GetLongestRangeWeapon() :
							GetBestDPSWeaponForRange(SwitchWeapons.Settings.LongRangeTarget);
						if (bestWeapon != null)
						{
							var pair = bestWeapon.toThingDefStuffDefPair();

							// Set weapon as forced so it sticks
							memory.SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [LongRange] switching to long range failed");
						}
					}
					break;
				case SwitchButtonEnum.MediumRange:
					{
						// Find highest DPS weapon for range
						var bestWeapon = SwitchWeapons.Settings.RangeUseLongestShortest ?
							GetClosestRangeWeapon(SwitchWeapons.Settings.MediumRangeTarget) : 
							GetBestDPSWeaponForRange(SwitchWeapons.Settings.MediumRangeTarget);
						if (bestWeapon != null)
						{
							var pair = bestWeapon.toThingDefStuffDefPair();

							// Set weapon as forced so it sticks
							memory.SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [MediumRange] switching to medium range failed");
						}
					}
					break;
				case SwitchButtonEnum.ShortRange:
					{
						// Find highest DPS weapon for range
						var bestWeapon = SwitchWeapons.Settings.RangeUseLongestShortest ?
							GetShortestRangeWeapon() : 
							GetBestDPSWeaponForRange(SwitchWeapons.Settings.ShortRangeTarget);
						if (bestWeapon != null)
						{
							var pair = bestWeapon.toThingDefStuffDefPair();

							// Set weapon as forced so it sticks
							memory.SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [ShortRange] switching to short range failed");
						}
					}
					break;

				case SwitchButtonEnum.Dangerous:
					{
						// Find next dangerous weapon
						var (current, carried) = GetCurrentAndCarriedWeapons(_pawn);

						// All weapons to list
						var weapons = new List<ThingWithComps>();
						foreach (var weapon in carried)
						{
							if (GettersFilters.isDangerousWeapon(weapon))
								weapons.Add(weapon);
						}

						// Find next
						if (GetPrevNextFromList(current, weapons, true, SwitchWeapons.Settings.PrevNextSortByRange ? SortByEnum.Range : SortByEnum.MarketValue) is ThingDefStuffDefPair pair)
						{
							// Set weapon as forced so it sticks
							memory.SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [Dangerous] switching to dangerous failed");
						}
					}
					break;
				case SwitchButtonEnum.EMP:
					{
						// Find next EMP weapon
						var (current, carried) = GetCurrentAndCarriedWeapons(_pawn);

						// All weapons to list
						var weapons = new List<ThingWithComps>();
						foreach (var weapon in carried)
						{
							if (GettersFilters.isEMPWeapon(weapon))
								weapons.Add(weapon);
						}

						// Find next
						if (GetPrevNextFromList(current, weapons, true, SwitchWeapons.Settings.PrevNextSortByRange ? SortByEnum.Range : SortByEnum.MarketValue) is ThingDefStuffDefPair pair)
						{
							// Set weapon as forced so it sticks
							memory.SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [EMP] switching to EMP failed");
						}
					}
					break;

				case SwitchButtonEnum.Next:
					{
						// Try find next weapon to switch to
						if (GetPrevNext(_pawn, true) is ThingDefStuffDefPair pair)
						{
							// Set weapon as forced so it sticks
							memory.SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [Next] switching to next weapon failed");
						}
					}
					break;
				case SwitchButtonEnum.Previous:
					{
						// Try find previous weapon to switch to
						if (GetPrevNext(_pawn, false) is ThingDefStuffDefPair pair)
						{
							// Set weapon as forced so it sticks
							memory.SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [Previous] switching to previous weapon failed");
						}
					}
					break;

				default:
					Log.Error($"Simple Sidearms - Switch Weapons: encountered unknown interaction: {interaction}");
					break;
			}
		}

		public override bool GroupsWith(Gizmo other) => 
			other is Gizmo_SwitchWeapon;
		#endregion

		#region PRIVATE METHODS
		private bool UseRanged(CompSidearmMemory memory) =>
			memory.primaryWeaponMode == PrimaryWeaponMode.Ranged 
			|| (memory.primaryWeaponMode == PrimaryWeaponMode.BySkill 
				&& _pawn.getSkillWeaponPreference() == PrimaryWeaponMode.Ranged);

		private bool SwitchToRanged(CompSidearmMemory memory)
		{
			// Switch to default weapon
			if (memory.DefaultRangedWeapon is ThingDefStuffDefPair pair 
				&& SwitchTo(pair))
				return true;

			// Switch to best ranged weapon
			if (GettersFilters.findBestRangedWeapon(_pawn, null, true, PeteTimesSix.SimpleSidearms.SimpleSidearms.Settings.SkipDangerousWeapons, true).weapon is ThingWithComps thing
				&& SwitchTo(thing))
				return true;

			// Switch failed
			return false;
		}
		private bool SwitchToMelee(CompSidearmMemory memory)
		{
			// Switch to preferred melee weapon
			if (memory.PreferredMeleeWeapon is ThingDefStuffDefPair pair
				&& SwitchTo(pair))
				return true;

			// Switch to best melee weapon
			if (GettersFilters.findBestMeleeWeapon(_pawn, out var thing, _pawn.equipment.Primary?.def.IsRangedWeapon != true, false)
				&& SwitchTo(thing))
				return true;

			// Switch failed
			return false;
		}

		private bool SwitchTo(ThingDefStuffDefPair pair) =>
			_pawn.equipment.Primary?.toThingDefStuffDefPair() == pair 
			|| WeaponAssingment.equipSpecificWeaponTypeFromInventory(_pawn, pair, IsFumbleDrop(), false);
		private bool SwitchTo(ThingWithComps thing) => 
			_pawn.equipment.Primary == thing 
			|| WeaponAssingment.equipSpecificWeapon(_pawn, thing, IsFumbleDrop(), false);

		private bool IsFumbleDrop() =>
			MiscUtils.shouldDrop(_pawn, DroppingModeEnum.Combat, false);

		private (ThingWithComps current, List<ThingWithComps> carried) GetCurrentAndCarriedWeapons(Pawn pawn)
		{
			// Shouldn't have a null-pawn, still better to check for it
			if (pawn == null)
				return (null, null);

			// Get currently equipped weapon
			var currentWeapon = pawn.equipment?.Primary;
			// Get carried weapons
			var carriedWeapons = pawn.GetCarriedWeapons(true, true);

			return (currentWeapon, carriedWeapons);
		}

		private ThingDefStuffDefPair? GetPrevNext(Pawn pawn, bool getNext)
		{
			var (current, carried) = GetCurrentAndCarriedWeapons(pawn);
			if (current == null || carried == null)
				return null;

			Func<ThingWithComps, bool> isValid;
			// If skipping dangerous is disabled
			if (!SwitchWeapons.Settings.PrevNextSkipDangerousAndEMP)
				isValid = thing => true;
			// ...otherwise if equipped weapon is dangerous, only dangerous weapons are valid
			else if (GettersFilters.isDangerousWeapon(current))
				isValid = thing => GettersFilters.isDangerousWeapon(thing);
			// ...otherwise if equipped weapon is emp, only emp weapons are valid
			else if (GettersFilters.isEMPWeapon(current))
				isValid = thing => GettersFilters.isEMPWeapon(thing);
			// ...otherwise any non-dangerous/emp weapon is valid
			else
				isValid = thing => !GettersFilters.isDangerousWeapon(thing) && !GettersFilters.isEMPWeapon(thing);

			var weapons = new List<ThingWithComps>();
			// If equipped weapon is ranged, find all carried ranged weapons
			if (current.def.IsRangedWeapon)
			{
				foreach (var carriedWeapon in carried)
					if (carriedWeapon?.def?.IsRangedWeapon == true && isValid(carriedWeapon))
						weapons.Add(carriedWeapon);
			}
			// ...otherwise find all carried melee weapons
			else
			{
				foreach (var carriedWeapon in carried)
					if (carriedWeapon?.def?.IsMeleeWeapon == true && isValid(carriedWeapon))
						weapons.Add(carriedWeapon);
			}

			// Get from list
			var sortBy = SwitchWeapons.Settings.PrevNextSortByRange && current.def.IsRangedWeapon ? SortByEnum.Range : SortByEnum.MarketValue;
			return GetPrevNextFromList(current, weapons, getNext, sortBy);
		}
		private ThingDefStuffDefPair? GetPrevNextFromList(ThingWithComps current, List<ThingWithComps> weapons, bool getNext, SortByEnum sortBy)
		{
			// Sort list
			switch (sortBy)
			{
				case SortByEnum.MarketValue:
					weapons.SortStable(SortByMarketValue);
					break;
				case SortByEnum.Range:
					weapons.SortStable(SortByRange);
					break;
			}

			// If none select, get first or last
			if (current == null)
				return (getNext ? weapons.FirstOrDefault() : weapons.LastOrDefault())?.toThingDefStuffDefPair();

			// Convert weapons to ThingDefStuffDefPairs
			var pairs = new List<ThingDefStuffDefPair>();
			foreach (var weapon in weapons)
			{
				var stuff = weapon.toThingDefStuffDefPair();
				if (!pairs.Contains(stuff))
					pairs.Add(stuff);
			}

			// Get index and pair of currently equipped weapon
			var currentPair = current.toThingDefStuffDefPair();
			var index = pairs.IndexOf(currentPair);

			// Return next or previous weapon
			index += getNext ? 1 : -1;
			if (index >= 0 && index < pairs.Count())
				return pairs[index];

			// No further weapon
			return currentPair;
		}

		private ThingWithComps GetBestDPSWeaponForRange(float desiredRange)
		{
			// try finding highest dps at range
			var output = GetRangedWeaponConditional((weapon, bestWeapon) =>
				GetDPSDifferenceAtRange(weapon, bestWeapon, desiredRange) > 0);
			// check for next highest range if no weapon of desired range found and setting is active
			if (output == null && SwitchWeapons.Settings.RangeUseHighestIfNotFound)
				output = GetRangedWeaponConditional((weapon, bestWeapon) =>
					GetWeaponRange(weapon) is float range && range > 0f && (!(GetWeaponRange(bestWeapon) is float bestRange) || range > bestRange));
			return output;
		}
		private ThingWithComps GetLongestRangeWeapon() =>
			GetRangedWeaponConditional((weapon, bestWeapon) =>
				GetWeaponRange(weapon) is float range 
				&& (!(GetWeaponRange(bestWeapon) is float bestRange)
					|| range > bestRange
					|| (range == bestRange && GetDPSDifferenceAtRange(weapon, bestWeapon, range) > 0))
			);
		private ThingWithComps GetShortestRangeWeapon() =>
			GetRangedWeaponConditional((weapon, bestWeapon) =>
				GetWeaponRange(weapon) is float range
				&& (!(GetWeaponRange(bestWeapon) is float bestRange)
					|| range < bestRange
					|| (range == bestRange && GetDPSDifferenceAtRange(weapon, bestWeapon, range) > 0)));
		private ThingWithComps GetClosestRangeWeapon(float desiredRange) =>
			GetRangedWeaponConditional(
				(weapon, bestWeapon) =>
				{
					if (GetWeaponRange(weapon) is float range)
					{
						if (!(GetWeaponRange(bestWeapon) is float bestRange))
							return true;

						var diff = range - desiredRange;
						var bestDiff = bestRange - desiredRange;
						if ((diff >= 0f || bestDiff < 0f) && MathF.Abs(diff) < MathF.Abs(bestDiff))
							return true;
						// fall back to dps if multiple weapons of desired range are found
						if (diff == bestDiff)
							return GetDPSDifferenceAtRange(weapon, bestWeapon, range) > 0;
					}
					return false;
				});
		private ThingWithComps GetRangedWeaponConditional(Func<ThingWithComps, ThingWithComps, bool> comparison)
		{
			var bestWeapon = default(ThingWithComps);
			foreach (var weapon in GetCurrentAndCarriedWeapons(_pawn).carried)
			{
				if (weapon?.def?.IsRangedWeapon == true
					&& !GettersFilters.isDangerousWeapon(weapon)
					&& !GettersFilters.isEMPWeapon(weapon))
				{
					if (comparison(weapon, bestWeapon))
						bestWeapon = weapon;
				}
			}
			return bestWeapon;
		}
		private int GetDPSDifferenceAtRange(ThingWithComps weapon1, ThingWithComps weapon2, float range)
		{
			if (weapon1 == null)
				return int.MinValue;
			if (weapon2 == null)
				return int.MaxValue;
			return (int)MathF.Round(GetDPSAtRange(weapon1, range) - GetDPSAtRange(weapon2, range), MidpointRounding.AwayFromZero);
		}

		// Sort by market value (the same is also used by Simple Sidearms for sorting the UI display)
		private int SortByMarketValue(ThingWithComps a, ThingWithComps b)
		{
			var diff = (int)((b.MarketValue - a.MarketValue) * 1000);
			if (diff != 0) 
				return diff;
			diff = (int)((GetDPS(b) - GetDPS(a)) * 1000);
			if (diff != 0)
				return diff;
			diff = string.Compare(b.def.defName, a.def.defName);
			if (diff != 0) 
				return diff;
			return b.GetHashCode() - a.GetHashCode();
		}
		// Sort by range
		private int SortByRange(ThingWithComps a, ThingWithComps b)
		{
			var aRange = GetWeaponRange(a);
			var bRange = GetWeaponRange(b);
			if ((int?)((bRange - aRange) * 1000) is int diff)
			{
				if (diff != 0)
					return diff;
				diff = (int)((GetDPS(b) - GetDPS(a)) * 1000);
				if (diff != 0)
					return diff;
			}
			else
			{
				if (bRange != null)
					return 1;
				if (aRange != null)
					return -1;
			}
			diff = string.Compare(b.def.defName, a.def.defName);
			if (diff != 0) 
				return diff;
			return b.GetHashCode() - a.GetHashCode();
		}

		private float? GetWeaponRange(ThingWithComps weapon) =>
			weapon?.def?.Verbs?.FirstOrDefault(v => v.Ranged)?.range;

		private float GetDPSAtRange(ThingWithComps thing, float range) =>
			GetDPS(thing) * GetRangeFactor(thing, range);
		private float GetDPS(ThingWithComps thing)
		{
			var verbProps = thing?.def?.Verbs?.FirstOrDefault(v => v.Ranged);
			if (verbProps == null)
				return 0f;

			var burstCount = verbProps.burstShotCount;
			var roundsPerMinute = verbProps.ticksBetweenBurstShots > 0 ? 3600.0f / verbProps.ticksBetweenBurstShots : verbProps.ticksBetweenBurstShots == 0 ? float.PositiveInfinity : 1;
			var cooldownInSeconds = thing.GetStatValue(StatDefOf.RangedWeapon_Cooldown);
			var warmupInSeconds = verbProps.warmupTime;

			float damagePerShot = 0f;
			var projectile = verbProps.defaultProjectile?.projectile;
			if (projectile != null)
				damagePerShot = projectile.GetDamageAmount(thing);

			var dps = damagePerShot * burstCount / (((burstCount - 1.0f) * 60.0f / roundsPerMinute) + cooldownInSeconds + warmupInSeconds);
			//Log.Message($"{thing} DPS  {damagePerShot} * {burstCount} / ((({burstCount} - 1.0) * 60.0 / {roundsPerMinute}) + {cooldownInSeconds} + {warmupInSeconds} = {dps}");
			return dps;
		}
		private float GetRangeFactor(ThingWithComps thing, float range)
		{
			var verbProps = thing?.def?.Verbs?.FirstOrDefault(v => v.Ranged);
			if (verbProps == null || range > verbProps.range)
				return 0f;

			var accuracyTouch = thing.GetStatValue(StatDefOf.AccuracyTouch);
			var accuracyShort = thing.GetStatValue(StatDefOf.AccuracyShort);
			var accuracyMedium = thing.GetStatValue(StatDefOf.AccuracyMedium);
			var accuracyLong = thing.GetStatValue(StatDefOf.AccuracyLong);

			float dpsFactorRange;
			if (range < 3)
				dpsFactorRange = accuracyTouch;
			else if (range < 12)
				dpsFactorRange = interpolate(3, 12, accuracyTouch, accuracyShort, range);
			else if (range < 25)
				dpsFactorRange = interpolate(12, 25, accuracyShort, accuracyMedium, range);
			else if (range < 40)
				dpsFactorRange = interpolate(25, 40, accuracyMedium, accuracyLong, range);
			else
				dpsFactorRange = accuracyLong;

			//Log.Message($"{thing} RFAC 3={accuracyTouch} 12={accuracyShort} 25={accuracyMedium} 40={accuracyLong} {range}={dpsFactorRange}");
			return dpsFactorRange;

			float interpolate(float x0, float x1, float y0, float y1, float x) =>
				(y0 * (x1 - x) + y1 * (x - x0)) / (x1 - x0);
		}

		#region DRAWING METHODS
		private bool DrawButton(Rect rect, Texture texture, Color color, string toolTip, KeyBindingDef key)
		{
			var oriColor = GUI.color;
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, toolTip.Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
				GUI.color = _highlightColor;
			}
			else
				GUI.color = _baseColor;
			GUI.DrawTexture(rect, TextureResources.Background);
			GUI.color = color;
			GUI.DrawTexture(rect, texture);
			GUI.color = oriColor;

			if (key != null && key.MainKey != KeyCode.None)
				Widgets.Label(rect.ContractedBy(_hotKeyLabelRectMarginX, _hotKeyLabelRectMarginY), key.MainKey.ToStringReadable());
			return Widgets.ButtonInvisible(rect, true) || key.KeyDownEvent;
		}

		private void DrawGizmoLabel(string labelText, Rect gizmoRect)
		{
			var labelHeight = Text.CalcHeight(labelText, gizmoRect.width);
			var labelRect = new Rect(gizmoRect.x, gizmoRect.yMax - labelHeight + 12f, gizmoRect.width, labelHeight);
			GUI.DrawTexture(labelRect, TexUI.GrayTextBG);
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperCenter;
			Widgets.Label(labelRect, labelText);
			Text.Anchor = TextAnchor.UpperLeft;
			GUI.color = Color.white;
		}
		#endregion
		#endregion
	}
}
