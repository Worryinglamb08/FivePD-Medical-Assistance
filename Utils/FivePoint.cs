using CitizenFX.Core;

namespace SaltyCallouts_FivePD.Utils
{
	internal struct FivePoint
	{
		internal Vector3 position;
		internal readonly float heading;

		public FivePoint(Vector3 position, float heading)
		{
			this.position = position;
			this.heading = heading;
		}

		public FivePoint(float x, float y, float z, float heading)
		{
			position = new Vector3(x, y, z);
			this.heading = heading;
		}
	}
}