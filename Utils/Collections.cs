using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.VehicleHash;

namespace SaltyCallouts_FivePD.Utils
{
	internal static class Collections
	{
		internal static class PoliceCars
		{
			internal static readonly List<VehicleHash> ALL = new List<VehicleHash>
			{
				Police,
				Police2,
				Police3,
				Police4,
				Sheriff,
				Sheriff2,
				Policeb,
				FBI,
				FBI2,
				Riot
			};

			internal static readonly List<VehicleHash> URBAN = new List<VehicleHash>
			{
				Police,
				Police2,
				Police3
			};

			internal static readonly List<VehicleHash> RURAL = new List<VehicleHash>
			{
				Sheriff,
				Sheriff2
			};

			internal static readonly List<VehicleHash> HIGHWAY = new List<VehicleHash>
			{
				Sheriff,
				Policeb
			};

			internal static readonly List<VehicleHash> UNDERCOVER = new List<VehicleHash>
			{
				FBI,
				FBI2,
				Police4
			};

			internal static readonly List<VehicleHash> NOOSE = new List<VehicleHash>
			{
				FBI2,
				Riot
			};
		}

		internal enum Speed
		{
			Suburban = 10,
			Street = 20,
			Highway = 30,

			SuperSlow = 5,
			Slow = 10,
			Normal = 20,
			Fast = 30,
			SuperFast = 40
		}

		internal enum RelationshipHash : uint
		{
			Player = 0x_6F07_83F5,
			Civmale = 0x_02B8_FA80,
			Civfemale = 0x_4703_3600,
			Cop = 0x_A49E_591C,
			SecurityGuard = 0x_F50B_51B7,
			PrivateSecurity = 0x_A882_EB57,
			Fireman = 0x_FC2C_A767,
			Gang1 = 0x_4325_F88A,
			Gang2 = 0x_11DE_95FC,
			Gang9 = 0x_8DC3_0DC3,
			Gang10 = 0x_0DBF_2731,
			AmbientGangLost = 0x_90C7_DA60,
			AmbientGangMexican = 0x_11A9_A7E3,
			AmbientGangFamily = 0x_4589_7C40,
			AmbientGangBallas = 0x_C26D_562A,
			AmbientGangMarabunte = 0x_7972_FFBD,
			AmbientGangCult = 0x_783E_3868,
			AmbientGangSalva = 0x_936E_7EFB,
			AmbientGangWeicheng = 0x_6A3B_9F86,
			AmbientGangHillbilly = 0x_B359_8E9C,
			Dealer = 0x_8296_713E,
			HatesPlayer = 0x_84DC_FAAD,
			Hen = 0x_C010_35F9,
			WildAnimal = 0x_7BEA_6617,
			Shark = 0x_2295_03C8,
			Cougar = 0x_CE13_3D78,
			NoRelationship = 0x_FADE_4843,
			Special = 0x_D9D0_8749,
			Mission2 = 0x_8040_1068,
			Mission3 = 0x_4929_2237,
			Mission4 = 0x_5B4D_C680,
			Mission5 = 0x_270A_5DFA,
			Mission6 = 0x_392C_823E,
			Mission7 = 0x_024F_9485,
			Mission8 = 0x_14CA_B97B,
			Army = 0x_E3D9_76F3,
			GuardDog = 0x_522B_964A,
			AggressiveInvestigate = 0x_EB47_D4E0,
			Medic = 0x_B042_3AA0,
			Prisoner = 0x_7EA2_6372,
			DomesticAnimal = 0x_72F3_0F6E,
			Deer = 0x_31E5_0E10
		}

		public enum RawVehicleDrivingFlags : uint
		{
			FollowTraffic = 1,
			YieldToPeds = 2,
			AvoidVehicles = 4,
			AvoidEmptyVehicles = 8,
			AvoidPeds = 16,
			AvoidObjects = 32,
			StopAtTrafficLight = 128,
			UseBlinkers = 256,
			AllowGoingWrongWayIfNeeded = 512,
			DriveReverse = 1_024,

			/// <summary>
			/// Removes most pathing limits, the driver even goes on dirt roads
			/// </summary>
			UseShortestPath = 262_144,

			/// <summary>
			/// Uses local pathing in ~200 proximity
			/// </summary>
			IgnoreRoads = 4_194_304,

			OvertakeIfPossible = 524_288,

			/// <summary>
			/// Goes straight to destination
			/// </summary>
			DontUsePathing = 16_777_216,
			AvoidHighwaysWhenPossible = 536_870_912
		}

		public enum CustomVehicleDrivingFlags : uint
		{
			// Calculated from RageVehicleDrivingFlags.Emergeny without WrongWay
			Emergency = 0b_100_00000000_00110110
		}

		public enum RageVehicleDrivingFlags : uint
		{
			None = 0,
			FollowTraffic = 1,
			YieldToCrossingPedestrians = 2,
			DriveAroundVehicles = 4,
			DriveAroundPeds = 16, // 0x00000010
			DriveAroundObjects = 32, // 0x00000020
			RespectIntersections = 128, // 0x00000080
			AllowWrongWay = 512, // 0x00000200
			Reverse = 1_024, // 0x00000400
			AllowMedianCrossing = 262_144, // 0x00040000
			DriveBySight = 4_194_304, // 0x00400000
			IgnorePathFinding = 16_777_216, // 0x01000000
			AvoidHighways = 536_870_912, // 0x20000000
			StopAtDestination = 2_147_483_648, // 0x80000000

			Normal = RespectIntersections | DriveAroundObjects | DriveAroundPeds | DriveAroundVehicles |
			         YieldToCrossingPedestrians | FollowTraffic, // 0x000000B7

			Emergency = AllowMedianCrossing | AllowWrongWay | DriveAroundObjects | DriveAroundPeds |
			            DriveAroundVehicles | YieldToCrossingPedestrians, // 0x00040236
		}

		internal static class Spawns
		{
			internal static class Hospitals
			{
				internal static readonly Location CENTRAL_LOS_SANTOS_MEDICAL_CENTER =
					new Location("Central Los Santos Medical Center", 293, -1_437, 30, 228);

				internal static readonly Location MOUNT_ZONAH_MEDICAL_CENTER =
					new Location("Mount Zonah Medical Center", -529, -327, 35, 28);

				internal static readonly Location PILLBOX_HILL_MEDICAL_CENTER =
					new Location("Pillbox Hill Medical Center", 360, -593, 29, 162);

				// TODO: The routing is still weird here. Needs a better solution (#4)
				// internal static readonly FivePoint VESPUCCI_LIFEGUARD_TOWER_3 = new FivePoint(-1422, -1513, 1, 289);

				internal static readonly Location SANDY_SHORES_MEDICAL_CENTER =
					new Location("Sandy Shores Medical Center", 1_827, 3_693, 34, 301);

				internal static readonly Location PALETO_BAY_MEDICAL_CENTER =
					new Location("Paleto Bay Medical Center", -235, 6_308, 31, 135);
			}

			internal static class PoliceStations
			{
				internal static class MissionRow
				{
					internal static readonly Location GARAGE =
						new Location("Mission Row Garage", 431, -996, 26, 178);
				}
			}

			internal static class Airports
			{
				internal static class Lsia
				{
					internal static readonly Location HANGAR_1 =
						new Location(new FivePoint(-1_026, -2_972, 14, 111), "LSIA Hangar 1");

					public static readonly Location EMS_TRANSFER =
						new Location("LSIA EMS Transfer", -1_026, -2_373, 14, 240);
				}
			}
		}
	}
}