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
					"SSSW_ShowLongRangeSwitchDesc".Translate(),
					ShowLongRangeSwitch,
					Default_ShowLongRangeSwitch);
				LongRangeTarget = ControlsBuilder.CreateNumeric(
					ref offsetY,
					width,
					"SSSW_LongRangeTarget".Translate(),
					"SSSW_LongRangeTargetDesc".Translate(),
					LongRangeTarget,
					Default_LongRangeTarget,
					nameof(LongRangeTarget));

				ShowMediumRangeSwitch = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowMediumRangeSwitch".Translate(),
					"SSSW_ShowMediumRangeSwitchDesc".Translate(),
					ShowMediumRangeSwitch,
					Default_ShowMediumRangeSwitch);
				MediumRangeTarget = ControlsBuilder.CreateNumeric(
					ref offsetY,
					width,
					"SSSW_MediumRangeTarget".Translate(),
					"SSSW_MediumRangeTargetDesc".Translate(),
					MediumRangeTarget,
					Default_MediumRangeTarget,
					nameof(MediumRangeTarget));

				ShowShortRangeSwitch = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_ShowShortRangeSwitch".Translate(),
					"SSSW_ShowShortRangeSwitchDesc".Translate(),
					ShowShortRangeSwitch,
					Default_ShowShortRangeSwitch);
				ShortRangeTarget = ControlsBuilder.CreateNumeric(
					ref offsetY,
					width,
					"SSSW_ShortRangeTarget".Translate(),
					"SSSW_ShortRangeTargetDesc".Translate(),
					ShortRangeTarget,
					Default_ShortRangeTarget,
					nameof(ShortRangeTarget));

				RangeUseHighestIfNotFound = ControlsBuilder.CreateCheckbox(
					ref offsetY,
					width,
					"SSSW_RangeUseHighestIfNotFound".Translate(),
					"SSSW_RangeUseHighestIfNotFoundDesc".Translate(),
					RangeUseHighestIfNotFound,
					Default_RangeUseHighestIfNotFound);

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
				ControlsBuilder.End();
			}
		}
		#endregion
	}
}
