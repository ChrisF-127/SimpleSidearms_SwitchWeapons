using SyControlsBuilder;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SwitchWeapons
{
	public class SwitchWeaponsSettings : ModSettings
	{
		#region CONSTANTS
		public const bool Default_ShowAfterDraftToggle = true;

		public const bool Default_ShowLongRangeSwitch = true;
		public const float Default_LongRangeTarget = 40f;

		public const bool Default_ShowMediumRangeSwitch = true;
		public const float Default_MediumRangeTarget = 25f;

		public const bool Default_ShowShortRangeSwitch = true;
		public const float Default_ShortRangeTarget = 12f;

		public const bool Default_RangeUseLongestShortest = false;
		public const bool Default_RangeUseHighestIfNotFound = true;

		public const bool Default_ShowDangerousSwitch = true;
		public const bool Default_ShowEMPSwitch = true;

		public const bool Default_ShowPrevNextSwitch = true;
		public const bool Default_PrevNextSortByRange = true;
		public const bool Default_PrevNextSkipDangerousAndEMP = true;
		#endregion

		#region PROPERTIES
		public bool ShowAfterDraftToggle { get; set; } = Default_ShowAfterDraftToggle;

		public bool ShowLongRangeSwitch { get; set; } = Default_ShowLongRangeSwitch;
		public float LongRangeTarget { get; set; } = Default_LongRangeTarget;

		public bool ShowMediumRangeSwitch { get; set; } = Default_ShowMediumRangeSwitch;
		public float MediumRangeTarget { get; set; } = Default_MediumRangeTarget;

		public bool ShowShortRangeSwitch { get; set; } = Default_ShowShortRangeSwitch;
		public float ShortRangeTarget { get; set; } = Default_ShortRangeTarget;

		public bool RangeUseLongestShortest { get; set; } = Default_RangeUseLongestShortest;
		public bool RangeUseHighestIfNotFound { get; set; } = Default_RangeUseHighestIfNotFound;

		public bool ShowDangerousSwitch { get; set; } = Default_ShowDangerousSwitch;
		public bool ShowEMPSwitch { get; set; } = Default_ShowEMPSwitch;

		public bool ShowPrevNextSwitch { get; set; } = Default_ShowPrevNextSwitch;
		public bool PrevNextSortByRange { get; set; } = Default_PrevNextSortByRange;
		public bool PrevNextSkipDangerousAndEMP { get; set; } = Default_PrevNextSkipDangerousAndEMP;
		#endregion

		#region PUBLIC METHODS
		public void DoSettingsWindowContents(Rect inRect)
		{
			var width = inRect.width;
			var offsetY = 0.0f;

			ControlsBuilder.Begin(inRect);
			try
			{
				// Longest/Mediumest/Shortest text addition when using longest/shortest
				var rangeDesc = RangeUseLongestShortest ? "est" : "";

				ShowAfterDraftToggle = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowAfterDraftToggle".Translate(),
					"SSSW_ShowAfterDraftToggleDesc".Translate(),
					ShowAfterDraftToggle,
					Default_ShowAfterDraftToggle);

				ShowLongRangeSwitch = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowLongRangeSwitch".Translate(),
					("SSSW_ShowLong" + rangeDesc + "RangeSwitchDesc").Translate(),
					ShowLongRangeSwitch,
					Default_ShowLongRangeSwitch);
				// only show if active and not using longest/shortest
				if (ShowLongRangeSwitch && !RangeUseLongestShortest)
				{
					LongRangeTarget = ControlsBuilder.CreateNumeric(
						ref offsetY,
						width,
						"SSSW_LongRangeTarget".Translate(),
						"SSSW_LongRangeTargetDesc".Translate(),
						LongRangeTarget,
						Default_LongRangeTarget,
						nameof(LongRangeTarget));
				}

				ShowMediumRangeSwitch = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowMediumRangeSwitch".Translate(),
					("SSSW_ShowMedium" + rangeDesc + "RangeSwitchDesc").Translate(),
					ShowMediumRangeSwitch,
					Default_ShowMediumRangeSwitch);
				// only show if active
				if (ShowMediumRangeSwitch)
				{
					MediumRangeTarget = ControlsBuilder.CreateNumeric(
						ref offsetY,
						width,
						"SSSW_MediumRangeTarget".Translate(),
						("SSSW_Medium" + rangeDesc + "RangeTargetDesc").Translate(),
						MediumRangeTarget,
						Default_MediumRangeTarget,
						nameof(MediumRangeTarget));
				}

				ShowShortRangeSwitch = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowShortRangeSwitch".Translate(),
					("SSSW_ShowShort" + rangeDesc + "RangeSwitchDesc").Translate(),
					ShowShortRangeSwitch,
					Default_ShowShortRangeSwitch);
				// only show if active and not using longest/shortest
				if (ShowShortRangeSwitch && !RangeUseLongestShortest)
				{
					ShortRangeTarget = ControlsBuilder.CreateNumeric(
						ref offsetY,
						width,
						"SSSW_ShortRangeTarget".Translate(),
						"SSSW_ShortRangeTargetDesc".Translate(),
						ShortRangeTarget,
						Default_ShortRangeTarget,
						nameof(ShortRangeTarget));
				}

				// only show if any range button is shown
				if (ShowLongRangeSwitch || ShowMediumRangeSwitch || ShowShortRangeSwitch)
				{
					RangeUseLongestShortest = ControlsBuilder.CreateCheckbox(
						ref offsetY,
						width,
						"SSSW_RangeUseLongestShortest".Translate(),
						"SSSW_RangeUseLongestShortestDesc".Translate(),
						RangeUseLongestShortest,
						Default_RangeUseLongestShortest);

					// not applicable when using longest/shortest
					if (!RangeUseLongestShortest)
					{
						RangeUseHighestIfNotFound = ControlsBuilder.CreateCheckbox(
							ref offsetY,
							width,
							"SSSW_RangeUseHighestIfNotFound".Translate(),
							"SSSW_RangeUseHighestIfNotFoundDesc".Translate(),
							RangeUseHighestIfNotFound,
							Default_RangeUseHighestIfNotFound);
					}
				}

				ShowDangerousSwitch = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowDangerousSwitch".Translate(),
					"SSSW_ShowDangerousSwitchDesc".Translate(),
					ShowDangerousSwitch,
					Default_ShowDangerousSwitch);
				ShowEMPSwitch = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowEMPSwitch".Translate(),
					"SSSW_ShowEMPSwitchDesc".Translate(),
					ShowEMPSwitch,
					Default_ShowEMPSwitch);

				ShowPrevNextSwitch = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowPrevNextSwitch".Translate(),
					"SSSW_ShowPrevNextSwitchDesc".Translate(),
					ShowPrevNextSwitch,
					Default_ShowPrevNextSwitch);
				PrevNextSortByRange = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_PrevNextSortByRange".Translate(),
					"SSSW_PrevNextSortByRangeDesc".Translate(),
					PrevNextSortByRange,
					Default_PrevNextSortByRange);
				PrevNextSkipDangerousAndEMP = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_PrevNextSkipDangerousAndEMP".Translate(),
					"SSSW_PrevNextSkipDangerousAndEMPDesc".Translate(),
					PrevNextSkipDangerousAndEMP,
					Default_PrevNextSkipDangerousAndEMP);
			}
			finally
			{
				ControlsBuilder.End(offsetY);
			}
		}
		#endregion

		#region OVERRIDES
		public override void ExposeData()
		{
			base.ExposeData();

			bool boolValue;
			float floatValue;

			boolValue = ShowAfterDraftToggle;
			Scribe_Values.Look(ref boolValue, nameof(ShowAfterDraftToggle), Default_ShowAfterDraftToggle);
			ShowAfterDraftToggle = boolValue;
			
			boolValue = ShowLongRangeSwitch;
			Scribe_Values.Look(ref boolValue, nameof(ShowLongRangeSwitch), Default_ShowLongRangeSwitch);
			ShowLongRangeSwitch = boolValue;
			floatValue = LongRangeTarget;
			Scribe_Values.Look(ref floatValue, nameof(LongRangeTarget), Default_LongRangeTarget);
			LongRangeTarget = floatValue;

			boolValue = ShowMediumRangeSwitch;
			Scribe_Values.Look(ref boolValue, nameof(ShowMediumRangeSwitch), Default_ShowMediumRangeSwitch);
			ShowMediumRangeSwitch = boolValue;
			floatValue = MediumRangeTarget;
			Scribe_Values.Look(ref floatValue, nameof(MediumRangeTarget), Default_MediumRangeTarget);
			MediumRangeTarget = floatValue;

			boolValue = ShowShortRangeSwitch;
			Scribe_Values.Look(ref boolValue, nameof(ShowShortRangeSwitch), Default_ShowShortRangeSwitch);
			ShowShortRangeSwitch = boolValue;
			floatValue = ShortRangeTarget;
			Scribe_Values.Look(ref floatValue, nameof(ShortRangeTarget), Default_ShortRangeTarget);
			ShortRangeTarget = floatValue;

			boolValue = RangeUseLongestShortest;
			Scribe_Values.Look(ref boolValue, nameof(RangeUseLongestShortest), Default_RangeUseLongestShortest);
			RangeUseLongestShortest = boolValue;
			boolValue = RangeUseHighestIfNotFound;
			Scribe_Values.Look(ref boolValue, nameof(RangeUseHighestIfNotFound), Default_RangeUseHighestIfNotFound);
			RangeUseHighestIfNotFound = boolValue;

			boolValue = ShowDangerousSwitch;
			Scribe_Values.Look(ref boolValue, nameof(ShowDangerousSwitch), Default_ShowDangerousSwitch);
			ShowDangerousSwitch = boolValue;
			boolValue = ShowEMPSwitch;
			Scribe_Values.Look(ref boolValue, nameof(ShowEMPSwitch), Default_ShowEMPSwitch);
			ShowEMPSwitch = boolValue;

			boolValue = ShowPrevNextSwitch;
			Scribe_Values.Look(ref boolValue, nameof(ShowPrevNextSwitch), Default_ShowPrevNextSwitch);
			ShowPrevNextSwitch = boolValue;
			boolValue = PrevNextSortByRange;
			Scribe_Values.Look(ref boolValue, nameof(PrevNextSortByRange), Default_PrevNextSortByRange);
			PrevNextSortByRange = boolValue;
			boolValue = PrevNextSkipDangerousAndEMP;
			Scribe_Values.Look(ref boolValue, nameof(PrevNextSkipDangerousAndEMP), Default_PrevNextSkipDangerousAndEMP);
			PrevNextSkipDangerousAndEMP = boolValue;
		}
		#endregion
	}
}
