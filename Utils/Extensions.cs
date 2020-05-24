using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static SaltyCallouts_FivePD.Utils.Utilities;
using Debug = System.Diagnostics.Debug;

namespace SaltyCallouts_FivePD.Utils
{
	internal static class Extensions
	{
		internal static void SetFreeWill(this Ped ped, bool state)
		{
			ped.AlwaysKeepTask = !state;
			ped.BlockPermanentEvents = !state;
		}

		internal static T GetRandom<T>(this List<T> list) =>
			list.Count switch
			{
				0 => throw new NullReferenceException("List was null"),
				1 => list[0],
				_ => list[RANDOM.Next(list.Count - 1)]
			};

		internal static Location ToLocation(
			this FivePoint point,
			string name
		) =>
			new Location(point, name);

		internal static IEnumerable<Location> GetInRangeOfPlayer(
			this IEnumerable<Location> locations,
			float min,
			float max)
		{
			var inRange = locations.Where(location =>
				min < location.DistanceToLocalPlayer && location.DistanceToLocalPlayer < max).ToList();

			if (!inRange.Any()) throw new Exception($"No destinations in range of {min} to {max} found.");

			return inRange;
		}
	}
}