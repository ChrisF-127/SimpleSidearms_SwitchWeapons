using aRandomKiwi.GFM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SwitchWeapons
{
	internal static class SwitchWeapons_Utility
	{
		internal static bool IsDrafted(Pawn pawn) =>
			pawn?.Drafted == true;
		internal static bool IsGuard(Pawn pawn)
		{
#warning TODO check for dll
			return pawn?.TryGetComp<Comp_Guard>()?.isActiveGuard() == true;
		}

		internal static bool IsSwitchable(Pawn pawn) =>
			IsDrafted(pawn) || IsGuard(pawn);
	}
}
