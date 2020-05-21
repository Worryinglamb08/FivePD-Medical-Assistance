using CitizenFX.Core;

namespace SaltyCallouts_FivePD.Utils
{
	internal static class Extensions
	{
		internal static void SetFreeWill(this Ped ped, bool state)
		{
			ped.AlwaysKeepTask = !state;
			ped.BlockPermanentEvents = !state;
		}
	}
}