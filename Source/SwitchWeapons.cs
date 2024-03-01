using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HugsLib;
using HugsLib.Settings;
using Verse;

namespace SwitchWeapons
{
	public class SwitchWeapons : ModBase
	{
		public static SettingHandle<bool> ShowAfterDraftToggle { get; private set; }

		public static SettingHandle<bool> ShowLongRangeSwitch { get; private set; }
		public static SettingHandle<bool> ShowMediumRangeSwitch { get; private set; }
		public static SettingHandle<bool> ShowShortRangeSwitch { get; private set; }
		public static SettingHandle<float> LongRangeTarget { get; private set; }
		public static SettingHandle<float> MediumRangeTarget { get; private set; }
		public static SettingHandle<float> ShortRangeTarget { get; private set; }

		public static SettingHandle<bool> ShowDangerousSwitch { get; private set; }

		public static SettingHandle<bool> ShowPrevNextSwitch { get; private set; }
		public static SettingHandle<bool> PrevNextSortByRange { get; private set; }
		public static SettingHandle<bool> PrevNextSkipDangerous { get; private set; }

		public override void DefsLoaded()
		{
			var name = nameof(ShowAfterDraftToggle);
			ShowAfterDraftToggle = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				true);

			name = nameof(ShowLongRangeSwitch);
			ShowLongRangeSwitch = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				true);
			name = nameof(LongRangeTarget);
			LongRangeTarget = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				40f);
			name = nameof(ShowMediumRangeSwitch);
			ShowMediumRangeSwitch = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				true);
			name = nameof(MediumRangeTarget);
			MediumRangeTarget = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				25f);
			name = nameof(ShowShortRangeSwitch);
			ShowShortRangeSwitch = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				true);
			name = nameof(ShortRangeTarget);
			ShortRangeTarget = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				12f);

			name = nameof(ShowDangerousSwitch);
			ShowDangerousSwitch = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				true);

			name = nameof(ShowPrevNextSwitch);
			ShowPrevNextSwitch = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				true);
			name = nameof(PrevNextSortByRange);
			PrevNextSortByRange = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				true);
			name = nameof(PrevNextSkipDangerous);
			PrevNextSkipDangerous = Settings.GetHandle(
				name,
				$"SSSW_{name}".Translate(),
				$"SSSW_{name}Desc".Translate(),
				true);
		}
	}
}
