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
		}

		#region FIELDS
		private const float _iconGap = 1f;
		private const float _iconSize = 32f;

		private Color _highlightColor = new Color(0.7f, 0.7f, 0.4f, 1f);
		private Color _baseColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		private Pawn _pawn = null;
		#endregion

		#region CONSTRUCTORS
		public Gizmo_SwitchWeapon(Pawn pawn)
		{
			_pawn = pawn;
			defaultLabel = "SSSW_SwitchWeaponsLabel".Translate();
			defaultDesc = "SSSW_SwitchWeaponsDesc".Translate();
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

			var _oriColor = GUI.color;
			var gizmoRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), Height);
			Widgets.DrawWindowBackground(gizmoRect);

			var center = gizmoRect.center;
			var vector = new Vector2(center.x, center.y - 4);

			var button = SwitchButtonEnum.None;
			if (DrawRangedIcon(vector))
				button = SwitchButtonEnum.Ranged;
			if (DrawMeleeIcon(vector))
				button = SwitchButtonEnum.Melee;
			if (DrawDisableIcon(vector))
				button = SwitchButtonEnum.Disable;
			if (DrawUnarmedIcon(vector))
				button = SwitchButtonEnum.Unarmed;

			GUI.color = _oriColor;

			DrawGizmoLabel("SSSW_Switch".Translate(), gizmoRect);

			return button != SwitchButtonEnum.None ? 
				new GizmoResult(GizmoState.Interacted, new Event(Event.current) { button = (int)button }) : 
				new GizmoResult(GizmoState.Clear);
		}

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

						// Switch to the best ranged weapon
						WeaponAssingment.equipBestWeaponFromInventoryByPreference(_pawn, Enums.DroppingModeEnum.Calm, Enums.PrimaryWeaponMode.Ranged);
					}
					break;
				case SwitchButtonEnum.Melee:
					{
						// Unset forced weapons
						var memory = CompSidearmMemory.GetMemoryCompForPawn(_pawn);
						memory.UnsetForcedWeapon(true);

						// Switch to the best melee weapon
						WeaponAssingment.equipBestWeaponFromInventoryByPreference(_pawn, Enums.DroppingModeEnum.Calm, Enums.PrimaryWeaponMode.Melee);

						// Set the weapon as forced if it is a melee weapon
						var weapon = _pawn.equipment.Primary;
						if (weapon?.def?.IsMeleeWeapon == true)
							memory.SetWeaponAsForced(weapon.toThingDefStuffDefPair(), true);
					}
					break;
				case SwitchButtonEnum.Disable:
					{
						// Unset forced weapons
						CompSidearmMemory.GetMemoryCompForPawn(_pawn).UnsetForcedWeapon(true);

						// Switch to preference
						WeaponAssingment.equipBestWeaponFromInventoryByPreference(_pawn, Enums.DroppingModeEnum.Calm);
					}
					break;
				case SwitchButtonEnum.Unarmed:
					{
						// Unset forced weapons
						var memory = CompSidearmMemory.GetMemoryCompForPawn(_pawn);
						memory.UnsetForcedWeapon(true);

						// Set unarmed as forced
						memory.SetUnarmedAsForced(true);

						// Switch to unarmed
						WeaponAssingment.equipBestWeaponFromInventoryByPreference(_pawn, Enums.DroppingModeEnum.Calm);
					}
					break;

				default:
				case SwitchButtonEnum.None:
					break;
			}
		}

		public override bool GroupsWith(Gizmo other) => other is Gizmo_SwitchWeapon;
		#endregion

		#region PRIVATE METHODS
		private bool DrawRangedIcon(Vector2 center)
		{
			Rect rect = new Rect
			{
				x = center.x - _iconSize,
				y = center.y - _iconSize,
				width = _iconSize,
				height = _iconSize,
			};

			if (Mouse.IsOver(rect))
			{
				GUI.color = _highlightColor;
				GUI.DrawTexture(rect, TextureResources.ForceRanged);
				TooltipHandler.TipRegion(rect, "SSSW_Ranged".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
			}
			else
			{
				GUI.color = _baseColor;
				GUI.DrawTexture(rect, TextureResources.ForceRanged);
			}

			return Widgets.ButtonInvisible(rect, true);
		}

		private bool DrawMeleeIcon(Vector2 center)
		{
			Rect rect = new Rect
			{
				x = center.x - _iconSize,
				y = center.y + _iconGap,
				width = _iconSize,
				height = _iconSize,
			};

			if (Mouse.IsOver(rect))
			{
				GUI.color = _highlightColor;
				GUI.DrawTexture(rect, TextureResources.ForceMelee);
				TooltipHandler.TipRegion(rect, "SSSW_Melee".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
			}
			else
			{
				GUI.color = _baseColor;
				GUI.DrawTexture(rect, TextureResources.ForceMelee);
			}
			
			return Widgets.ButtonInvisible(rect, true);
		}

		private bool DrawDisableIcon(Vector2 center)
		{
			Rect rect = new Rect
			{
				x = center.x + _iconGap,
				y = center.y - _iconSize,
				width = _iconSize,
				height = _iconSize,
			};

			if (Mouse.IsOver(rect))
			{
				GUI.color = _highlightColor;
				GUI.DrawTexture(rect, TextureResources.Disable);
				TooltipHandler.TipRegion(rect, "SSSW_Disable".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
			}
			else
			{
				GUI.color = _baseColor;
				GUI.DrawTexture(rect, TextureResources.Disable);
			}
			
			return Widgets.ButtonInvisible(rect, true);
		}

		private bool DrawUnarmedIcon(Vector2 center)
		{
			Rect rect = new Rect
			{
				x = center.x + _iconGap,
				y = center.y + _iconGap,
				width = _iconSize,
				height = _iconSize,
			};

			if (Mouse.IsOver(rect))
			{
				GUI.color = _highlightColor;
				GUI.DrawTexture(rect, TextureResources.ForceUnarmed);
				TooltipHandler.TipRegion(rect, "SSSW_Unarmed".Translate());
				MouseoverSounds.DoRegion(rect, SoundDefOf.Mouseover_Command);
			}
			else
			{
				GUI.color = _baseColor;
				GUI.DrawTexture(rect, TextureResources.ForceUnarmed);
			}
			
			return Widgets.ButtonInvisible(rect, true);
		}
		#endregion


		// Borrowed this method from Simple Sidearms - all credits goes to its author - Thanks!
		public void DrawGizmoLabel(string labelText, Rect gizmoRect)
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
	}
}
