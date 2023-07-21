using HarmonyLib;
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

			Next,
			Previous,
		}

		#region FIELDS
		private const float _iconGap = 1f;
		private const float _iconSize = 32f;

		private Color _rangedColor = new Color(0.4f, 0.8f, 0.4f);
		private Color _disableColor = new Color(0.8f, 0.8f, 0.8f);
		private Color _meleeColor = new Color(0.8f, 0.4f, 0.4f);
		private Color _unarmedColor = new Color(0.8f, 0.6f, 0.4f);

		private Color _nextPrevColor = new Color(0.8f, 0.8f, 0.8f);

		private Color _baseColor = new Color(0.5f, 0.5f, 0.5f);
		private Color _highlightColor = new Color(1.0f, 1.0f, 1.0f);

		private const float _hotKeyLabelRectMarginX = 3f;
		private const float _hotKeyLabelRectMarginY = 1f;

		private readonly Pawn _pawn = null;

		private readonly KeyBindingDef hotKeyRanged = SwitchWeaponKeyBindingDefOf.SSSW_Ranged;
		private readonly KeyBindingDef hotKeyMelee = SwitchWeaponKeyBindingDefOf.SSSW_Melee;
		private readonly KeyBindingDef hotKeyDisable = SwitchWeaponKeyBindingDefOf.SSSW_Disable;
		private readonly KeyBindingDef hotKeyUnarmed = SwitchWeaponKeyBindingDefOf.SSSW_Unarmed;
		private readonly KeyBindingDef hotKeyNext = SwitchWeaponKeyBindingDefOf.SSSW_Next;
		private readonly KeyBindingDef hotKeyPrevious = SwitchWeaponKeyBindingDefOf.SSSW_Previous;
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

			var buttonTopLeft = new Vector2(topLeft.x + 4, topLeft.y + 2);
			var button = SwitchButtonEnum.None;

			var oriFont = Text.Font;
			Text.Font = GameFont.Tiny;
			if (DrawRangedIcon(buttonTopLeft, hotKeyRanged))
				button = SwitchButtonEnum.Ranged;
			if (DrawMeleeIcon(buttonTopLeft, hotKeyMelee))
				button = SwitchButtonEnum.Melee;
			if (DrawDisableIcon(buttonTopLeft, hotKeyDisable))
				button = SwitchButtonEnum.Disable;
			if (DrawUnarmedIcon(buttonTopLeft, hotKeyUnarmed))
				button = SwitchButtonEnum.Unarmed;

			// would love for these buttons only to appear if the pawn(s) carry any weapons that can be switched to, but for performance reasons it is probably better not to do that
			if (DrawNextIcon(buttonTopLeft, hotKeyNext))
				button = SwitchButtonEnum.Next;
			if (DrawPreviousIcon(buttonTopLeft, hotKeyPrevious))
				button = SwitchButtonEnum.Previous;
			Text.Font = Text.Font;

			DrawGizmoLabel("SSSW_Switch".Translate(), gizmoRect);

			if (button != SwitchButtonEnum.None)
				return new GizmoResult(GizmoState.Interacted, new Event(Event.current) { button = (int)button });
			return new GizmoResult(GizmoState.Clear);
		}

		public override float GetWidth(float maxWidth) => 
			_iconSize * 3 + _iconGap * 2 + 8;

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);

			switch ((SwitchButtonEnum)ev.button)
			{
				case SwitchButtonEnum.Ranged:
					{
						// Unset forced weapons
						var memory = CompSidearmMemory.GetMemoryCompForPawn(_pawn);
						memory.UnsetForcedWeapon(true);

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
						// Unset forced weapons
						var memory = CompSidearmMemory.GetMemoryCompForPawn(_pawn);
						memory.UnsetForcedWeapon(true);

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
						var memory = CompSidearmMemory.GetMemoryCompForPawn(_pawn);

						// Unset forced weapons
						memory.UnsetForcedWeapon(true);

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
						// Unset forced weapons
						var memory = CompSidearmMemory.GetMemoryCompForPawn(_pawn);

						// Unset forced weapons, otherwise it won't switch to unarmed
						memory.UnsetForcedWeapon(true);

						// Set unarmed as forced
						memory.SetUnarmedAsForced(true);

						// Switch to unarmed
						if (!SwitchTo(null))
							Log.Warning("Simple Sidearms - Switch Weapons: [Unarmed] switching to unarmed failed");
					}
					break;

				case SwitchButtonEnum.Next:
					{
						// Try find next weapon to switch to
						if (GetWeapon(_pawn, true) is ThingDefStuffDefPair pair)
						{
							// Set weapon as forced so it sticks
							CompSidearmMemory.GetMemoryCompForPawn(_pawn).SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [Next] switching to next weapon failed");
						}
					}
					break;
				case SwitchButtonEnum.Previous:
					{
						// Try find previous weapon to switch to
						if (GetWeapon(_pawn, false) is ThingDefStuffDefPair pair)
						{
							// Set weapon as forced so it sticks
							CompSidearmMemory.GetMemoryCompForPawn(_pawn).SetWeaponAsForced(pair, true);

							// Switch to weapon
							if (!SwitchTo(pair))
								Log.Warning("Simple Sidearms - Switch Weapons: [Previous] switching to previous weapon failed");
						}
					}
					break;

				default:
				case SwitchButtonEnum.None:
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

		private ThingDefStuffDefPair? GetWeapon(Pawn pawn, bool getNext)
		{
			// Shouldn't have a null-pawn, still better to check for it
			if (pawn == null)
				return null;

			// Get currently equipped weapon
			var currentWeapon = pawn.equipment?.Primary;
			// Get carried weapons
			var carriedWeapons = pawn.GetCarriedWeapons(true, true);
			
			// Check if has a weapon equipped (otherwise the pawn is unarmed) and is carrying weapons
			if (currentWeapon?.def != null && carriedWeapons?.Count() > 0)
			{
				var weapons = new List<ThingWithComps>();

				// If equipped weapon is ranged, find all carried ranged weapons
				if (currentWeapon.def.IsRangedWeapon)
				{
					foreach (var carriedWeapon in carriedWeapons)
						if (carriedWeapon?.def?.IsRangedWeapon == true)
							weapons.Add(carriedWeapon);
				}
				// ...otherwise find all carried melee weapons
				else
				{
					foreach (var carriedWeapon in carriedWeapons)
						if (carriedWeapon?.def?.IsMeleeWeapon == true)
							weapons.Add(carriedWeapon);
				}

				// Sort weapons by market value (the same is also used by Simple Sidearms for sorting the UI display)
				weapons.SortStable((a, b) => (int)((b.MarketValue - a.MarketValue) * 1000));

				// Convert weapons to ThingDefStuffDefPairs
				var pairs = new List<ThingDefStuffDefPair>();
				foreach (var weapon in weapons)
				{
					var stuff = weapon.toThingDefStuffDefPair();
					if (!pairs.Contains(stuff))
						pairs.Add(stuff);
				}

				// Get index and pair of currently equipped weapon
				var currentPair = currentWeapon.toThingDefStuffDefPair();
				var index = pairs.IndexOf(currentPair);

				// Return next or previous weapon
				index += getNext ? 1 : -1;
				if (index >= 0 && index < pairs.Count())
					return pairs[index];
			}

			// No further weapon
			return null;
		}

		#region DRAWING METHODS
		private bool DrawRangedIcon(Vector2 topLeft, KeyBindingDef key)
		{
			Rect rect = new Rect
			{
				x = topLeft.x,
				y = topLeft.y,
				width = _iconSize,
				height = _iconSize,
			};

			var oriColor = GUI.color;
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, "SSSW_Ranged".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
				GUI.color = _highlightColor;
			}
			else
				GUI.color = _baseColor;
			GUI.DrawTexture(rect, TextureResources.Background);
			GUI.color = _rangedColor;
			GUI.DrawTexture(rect, TextureResources.ForceRanged);
			GUI.color = oriColor;

			if (key != null && key.MainKey != KeyCode.None)
				Widgets.Label(rect.ContractedBy(_hotKeyLabelRectMarginX, _hotKeyLabelRectMarginY), key.MainKey.ToStringReadable());
			return Widgets.ButtonInvisible(rect, true) || key.KeyDownEvent;
		}

		private bool DrawMeleeIcon(Vector2 topLeft, KeyBindingDef key)
		{
			Rect rect = new Rect
			{
				x = topLeft.x,
				y = topLeft.y + _iconGap + _iconSize,
				width = _iconSize,
				height = _iconSize,
			};

			var oriColor = GUI.color;
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, "SSSW_Melee".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
				GUI.color = _highlightColor;
			}
			else
				GUI.color = _baseColor;
			GUI.DrawTexture(rect, TextureResources.Background);
			GUI.color = _meleeColor;
			GUI.DrawTexture(rect, TextureResources.ForceMelee);
			GUI.color = oriColor;

			if (key != null && key.MainKey != KeyCode.None)
				Widgets.Label(rect.ContractedBy(_hotKeyLabelRectMarginX, _hotKeyLabelRectMarginY), key.MainKey.ToStringReadable());
			return Widgets.ButtonInvisible(rect, true) || key.KeyDownEvent;
		}

		private bool DrawDisableIcon(Vector2 topLeft, KeyBindingDef key)
		{
			Rect rect = new Rect
			{
				x = topLeft.x + _iconGap + _iconSize,
				y = topLeft.y,
				width = _iconSize,
				height = _iconSize,
			};

			var oriColor = GUI.color;
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, "SSSW_Disable".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
				GUI.color = _highlightColor;
			}
			else
				GUI.color = _baseColor;
			GUI.DrawTexture(rect, TextureResources.Background);
			GUI.color = _disableColor;
			GUI.DrawTexture(rect, TextureResources.Disable);
			GUI.color = oriColor;

			if (key != null && key.MainKey != KeyCode.None)
				Widgets.Label(rect.ContractedBy(_hotKeyLabelRectMarginX, _hotKeyLabelRectMarginY), key.MainKey.ToStringReadable());
			return Widgets.ButtonInvisible(rect, true) || key.KeyDownEvent;
		}

		private bool DrawUnarmedIcon(Vector2 topLeft, KeyBindingDef key)
		{
			Rect rect = new Rect
			{
				x = topLeft.x + _iconGap + _iconSize,
				y = topLeft.y + _iconGap + _iconSize,
				width = _iconSize,
				height = _iconSize,
			};

			var oriColor = GUI.color;
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, "SSSW_Unarmed".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
				GUI.color = _highlightColor;
			}
			else
				GUI.color = _baseColor;
			GUI.DrawTexture(rect, TextureResources.Background);
			GUI.color = _unarmedColor;
			GUI.DrawTexture(rect, TextureResources.ForceUnarmed);
			GUI.color = oriColor;

			if (key != null && key.MainKey != KeyCode.None)
				Widgets.Label(rect.ContractedBy(_hotKeyLabelRectMarginX, _hotKeyLabelRectMarginY), key.MainKey.ToStringReadable());
			return Widgets.ButtonInvisible(rect, true) || key.KeyDownEvent;
		}

		private bool DrawNextIcon(Vector2 topLeft, KeyBindingDef key)
		{
			Rect rect = new Rect
			{
				x = topLeft.x + _iconGap * 2 + _iconSize * 2,
				y = topLeft.y,
				width = _iconSize,
				height = _iconSize,
			};

			var oriColor = GUI.color;
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, "SSSW_Next".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
				GUI.color = _highlightColor;
			}
			else
				GUI.color = _baseColor;
			GUI.DrawTexture(rect, TextureResources.Background);
			GUI.color = _nextPrevColor;
			GUI.DrawTexture(rect, TextureResources.Next);
			GUI.color = oriColor;

			if (key != null && key.MainKey != KeyCode.None)
				Widgets.Label(rect.ContractedBy(_hotKeyLabelRectMarginX, _hotKeyLabelRectMarginY), key.MainKey.ToStringReadable());
			return Widgets.ButtonInvisible(rect, true) || key.KeyDownEvent;
		}

		private bool DrawPreviousIcon(Vector2 topLeft, KeyBindingDef key)
		{
			Rect rect = new Rect
			{
				x = topLeft.x + _iconGap * 2 + _iconSize * 2,
				y = topLeft.y + _iconGap + _iconSize,
				width = _iconSize,
				height = _iconSize,
			};

			var oriColor = GUI.color;
			if (Mouse.IsOver(rect))
			{
				TooltipHandler.TipRegion(rect, "SSSW_Previous".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
				GUI.color = _highlightColor;
			}
			else
				GUI.color = _baseColor;
			GUI.DrawTexture(rect, TextureResources.Background);
			GUI.color = _nextPrevColor;
			GUI.DrawTexture(rect, TextureResources.Previous);
			GUI.color = oriColor;

			if (key != null && key.MainKey != KeyCode.None)
				Widgets.Label(rect.ContractedBy(_hotKeyLabelRectMarginX, _hotKeyLabelRectMarginY), key.MainKey.ToStringReadable());
			return Widgets.ButtonInvisible(rect, true) || key.KeyDownEvent;
		}

		// Borrowed this method from Simple Sidearms - all credits goes to its author - Thanks!
		private void DrawGizmoLabel(string labelText, Rect gizmoRect)
		{
			var labelHeight = Text.CalcHeight(labelText, gizmoRect.width);
			labelHeight -= 2f;
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
