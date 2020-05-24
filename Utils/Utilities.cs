using System;

namespace SaltyCallouts_FivePD.Utils
{
	internal static class Utilities
	{
		internal static readonly Random RANDOM = new Random();
		internal static bool RandomBool => RANDOM.Next(1) == 0;
	}
}