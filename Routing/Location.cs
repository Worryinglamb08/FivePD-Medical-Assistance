using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using SaltyCallouts_FivePD.Routing;
using SaltyCallouts_FivePD.Utils;
using static CitizenFX.Core.World;

namespace SaltyCallouts_FivePD
{
	internal class Location
	{
		internal string Name { get; }
		internal Vector3 Position => fivePoint.position;
		internal float Heading => fivePoint.heading;
		internal float DistanceToLocalPlayer => GetDistance(Position, Game.PlayerPed.Position);
		private float RoadDistanceToLocalPlayer => CalculateTravelDistance(Game.PlayerPed.Position, Position);

		public float DistanceTo(Vector3 target) => GetDistance(Position, target);


		// ReSharper disable once FieldCanBeMadeReadOnly.Local throws VerificationException otherwise

		private FivePoint fivePoint;

		public Location(FivePoint fivePoint, string name)
		{
			this.fivePoint = fivePoint;
			Name = name;
		}

		public Location(string name, float x, float y, float z, float heading)
		{
			fivePoint = new FivePoint(x, y, z, heading);
			Name = name;
		}

		public override string ToString() => $"{Name} at {Position}";
	}
}