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
		private SettingHandle<bool> _showAfterDraftToggle;
		public static bool ShowAfterDraftToggle { get; private set; }

		public override void DefsLoaded()
		{
			_showAfterDraftToggle = Settings.GetHandle(
				"showAfterDraftToggle",
				"SSSW_ShowAfterDraftToggle".Translate(),
				"SSSW_ShowAfterDraftToggleDesc".Translate(),
				ShowAfterDraftToggle);
			_showAfterDraftToggle.ValueChanged += val => ShowAfterDraftToggle = (SettingHandle<bool>)val;
			ShowAfterDraftToggle = _showAfterDraftToggle;
		}
	}
}
