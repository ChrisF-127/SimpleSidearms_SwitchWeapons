using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace SwitchWeapons
{
	public class SwitchWeapons : Mod
	{
		#region PROPERTIES
		public static SwitchWeapons Instance { get; private set; }
		public static SwitchWeaponsSettings Settings { get; private set; }
		#endregion

		#region CONSTRUCTORS
		static SwitchWeapons()
		{
			var harmony = new Harmony("syrus.simplesidearmsswitchweapons");
			harmony.PatchAll();
		}

		public SwitchWeapons(ModContentPack content) : 
			base(content)
		{
			Instance = this;

			LongEventHandler.ExecuteWhenFinished(Initialize);
		}
		#endregion

		#region OVERRIDES
		public override string SettingsCategory() =>
			"Simple Sidearms - Switch Weapons";

		public override void DoSettingsWindowContents(Rect inRect)
		{
			base.DoSettingsWindowContents(inRect);

			Settings.DoSettingsWindowContents(inRect);
		}
		#endregion

		#region PRIVATE METHODS
		private void Initialize()
		{
			Settings = GetSettings<SwitchWeaponsSettings>();
		}
		#endregion
	}
}
