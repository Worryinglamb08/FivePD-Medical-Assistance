using System.Data.SqlTypes;
using CitizenFX.Core;

namespace SaltyCallouts_FivePD.Utils
{
	internal class SpawnPoint
	{
		internal Vector3 position;
		internal float heading;

		private SpawnPoint(Vector3 position, float heading = 0f)
		{
			this.position = position;
			this.heading = heading;
		}

		private SpawnPoint(float x, float y, float z, float heading)
		{
			position = new Vector3(x, y, z);
			this.heading = heading;
		}

		internal static class Spawns
		{
			internal static class Hospitals
			{
				internal static class CentralLosSantosMedicalCenter
				{
					internal static readonly SpawnPoint TRANSFER =
						new SpawnPoint(293.53f, -1437.89f, 29.42f, 228.97f);
				}
			}

			internal static class PoliceStations
			{
				internal static class MissionRow
				{
					internal static readonly SpawnPoint GARAGE =
						new SpawnPoint(431.6232f, -996.9889f, 25.35327f, 178.5626f);
				}
			}

			internal static class Airports
			{
				internal static class Lsia
				{
					internal static readonly SpawnPoint HANGAR_FRONT =
						new SpawnPoint(-1026.21f, -2972.06f, 13.56f, 111.14f);

					public static readonly SpawnPoint EMS_ARRIVAL =
						new SpawnPoint(-1026.33f, -2373.82f, 13.56f, 240.54f);
				}
			}
		}
	}
}