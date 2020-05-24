namespace SaltyCallouts_FivePD.Routing
{
	internal class Route
	{
		public Location Start { get; }
		public Location End { get; }
		public bool IsBidirectional { get; }

		public Route(Location start, Location end, bool isBidirectional = false)
		{
			Start = start;
			End = end;
			IsBidirectional = isBidirectional;
		}

		public override string ToString() => $"Route from: {Start} to {End}";
	}
}