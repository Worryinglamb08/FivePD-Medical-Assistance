using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CitizenFX.Core;
using static CitizenFX.Core.VehicleHash;
using static SaltyCallouts_FivePD.Utils.Collections.RawVehicleDrivingFlags;

namespace SaltyCallouts_FivePD.Utils
{
	internal static class Collections
	{
		internal static class PoliceCars
		{
			internal static readonly List<VehicleHash> ALL = new List<VehicleHash>()
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

			internal static readonly List<VehicleHash> URBAN = new List<VehicleHash>()
			{
				Police,
				Police2,
				Police3
			};

			internal static readonly List<VehicleHash> RURAL = new List<VehicleHash>()
			{
				Sheriff,
				Sheriff2
			};

			internal static readonly List<VehicleHash> HIGHWAY = new List<VehicleHash>()
			{
				Sheriff,
				Policeb
			};

			internal static readonly List<VehicleHash> UNDERCOVER = new List<VehicleHash>()
			{
				FBI,
				FBI2,
				Police4
			};

			internal static readonly List<VehicleHash> NOOSE = new List<VehicleHash>()
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
			Player = 0x6F0783F5,
			Civmale = 0x02B8FA80,
			Civfemale = 0x47033600,
			Cop = 0xA49E591C,
			SecurityGuard = 0xF50B51B7,
			PrivateSecurity = 0xA882EB57,
			Fireman = 0xFC2CA767,
			Gang1 = 0x4325F88A,
			Gang2 = 0x11DE95FC,
			Gang9 = 0x8DC30DC3,
			Gang10 = 0x0DBF2731,
			AmbientGangLost = 0x90C7DA60,
			AmbientGangMexican = 0x11A9A7E3,
			AmbientGangFamily = 0x45897C40,
			AmbientGangBallas = 0xC26D562A,
			AmbientGangMarabunte = 0x7972FFBD,
			AmbientGangCult = 0x783E3868,
			AmbientGangSalva = 0x936E7EFB,
			AmbientGangWeicheng = 0x6A3B9F86,
			AmbientGangHillbilly = 0xB3598E9C,
			Dealer = 0x8296713E,
			HatesPlayer = 0x84DCFAAD,
			Hen = 0xC01035F9,
			WildAnimal = 0x7BEA6617,
			Shark = 0x229503C8,
			Cougar = 0xCE133D78,
			NoRelationship = 0xFADE4843,
			Special = 0xD9D08749,
			Mission2 = 0x80401068,
			Mission3 = 0x49292237,
			Mission4 = 0x5B4DC680,
			Mission5 = 0x270A5DFA,
			Mission6 = 0x392C823E,
			Mission7 = 0x024F9485,
			Mission8 = 0x14CAB97B,
			Army = 0xE3D976F3,
			GuardDog = 0x522B964A,
			AggressiveInvestigate = 0xEB47D4E0,
			Medic = 0xB0423AA0,
			Prisoner = 0x7EA26372,
			DomesticAnimal = 0x72F30F6E,
			Deer = 0x31E50E10
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
			DriveReverse = 1024,

			/// <summary>
			/// Removes most pathing limits, the driver even goes on dirt roads
			/// </summary>
			UseShortestPath = 262144,

			/// <summary>
			/// Uses local pathing in ~200 proximity
			/// </summary>
			IgnoreRoads = 4194304,

			OvertakeIfPossible = 524288,

			/// <summary>
			/// Goes straight to destination
			/// </summary>
			DontUsePathing = 16777216,
			AvoidHighwaysWhenPossible = 536870912
		}

		public enum CombinedVehicleDrivingFlags : uint
		{
			EmergencyCautious = YieldToPeds | AvoidVehicles | AvoidEmptyVehicles | AvoidPeds | AvoidObjects |
			                    UseBlinkers | OvertakeIfPossible
		}
	}
}