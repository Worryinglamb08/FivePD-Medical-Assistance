using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CalloutAPI;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using SaltyCallouts_FivePD.Routing;
using SaltyCallouts_FivePD.Utils;
using static CitizenFX.Core.BaseScript;
using static CitizenFX.Core.Debug;
using static CitizenFX.Core.UI.Screen;
using static CitizenFX.Core.World;
using static SaltyCallouts_FivePD.Utils.Collections;

namespace SaltyCallouts_FivePD.Callouts
{
	[CalloutProperties("Medical Escort", "Salzian", "latest", Probability.Low)]
	[SuppressMessage("ReSharper", "UnusedType.Global")]
	public class MedicalEscort : SaltyCallout
	{
		private protected override int ProcessTickDelay { get; set; } = 500;

		private static readonly RouteCollection ROUTE_COLLECTION = new RouteCollection(
			/* TODO LSIA is broken cause the ambulance can't find it's way out the gate (#5)
			new Route(Spawns.Airports.Lsia.EMS_TRANSFER, Spawns.Hospitals.CENTRAL_LOS_SANTOS_MEDICAL_CENTER, true),
			new Route(Spawns.Airports.Lsia.EMS_TRANSFER, Spawns.Hospitals.PILLBOX_HILL_MEDICAL_CENTER, true),
			new Route(Spawns.Airports.Lsia.EMS_TRANSFER, Spawns.Hospitals.MOUNT_ZONAH_MEDICAL_CENTER, true),
			*/
			new Route(Spawns.Hospitals.CENTRAL_LOS_SANTOS_MEDICAL_CENTER, Spawns.Hospitals.PILLBOX_HILL_MEDICAL_CENTER,
				true),
			new Route(Spawns.Hospitals.PILLBOX_HILL_MEDICAL_CENTER, Spawns.Hospitals.MOUNT_ZONAH_MEDICAL_CENTER, true),
			new Route(Spawns.Hospitals.MOUNT_ZONAH_MEDICAL_CENTER, Spawns.Hospitals.CENTRAL_LOS_SANTOS_MEDICAL_CENTER,
				true),
			new Route(Spawns.Hospitals.SANDY_SHORES_MEDICAL_CENTER, Spawns.Hospitals.PILLBOX_HILL_MEDICAL_CENTER)

			// TODO Paleto bay ahas weird routing via the 
			//new Route(Spawns.Hospitals.PALETO_BAY_MEDICAL_CENTER, Spawns.Hospitals.PILLBOX_HILL_MEDICAL_CENTER)
		);


		private Ped? driver;
		private Ped? passenger;
		private Ped? patient;
		private Vehicle? ambulance;
		private Blip? ambulanceBlip;
		private readonly Route route;

		private float averageVelocity;
		private int parkStartTime;

		#region Callout

		public MedicalEscort() : base("Medical Escort")
		{
			// TODO Revert max once CheckRequirements is fixed https://github.com/KDani-99/FivePD-API/issues/43
			route = ROUTE_COLLECTION.GetNearestRandomRoute(100f, 3000f);

			/* TODO Revert once CheckRequirements is fixed https://github.com/KDani-99/FivePD-API/issues/43
			if (route == null)
			{
				WriteLine("Unable to find route");
				fail = true;
			}
			*/

			InitBase(route.Start.Position);

			ShortName = "Medical Escort";
			CalloutDescription = "A critical patient needs to be escorted.";
			ResponseCode = 2;
			StartDistance = 50f;
		}

		/* TODO CheckRequirements is broken https://github.com/KDani-99/FivePD-API/issues/43
		public override Task<bool> CheckRequirements()
		{
			WriteLine($"Invokation failed: {fail}");
			return Task.FromResult(false);
		}
		*/


		public override async Task Init()
		{
			OnAccept(StartDistance);
			Marker.Scale = StartDistance;

			if (route != null)
			{
				ambulance = await SpawnVehicle(VehicleHash.Ambulance, route.Start.Position, route.Start.Heading);
				ambulance.IsSirenActive = true;
				ambulance.IsSirenSilent = true;
				ambulance.Doors[VehicleDoorIndex.FrontLeftDoor].Open();
				ambulance.Doors[VehicleDoorIndex.BackRightDoor].Open();

				driver = await SpawnPed(PedHash.Paramedic01SMM, ambulance.Position);
				driver.SetIntoVehicle(ambulance, VehicleSeat.Driver);
				driver.SetFreeWill(false);

				passenger = await SpawnPed(PedHash.Paramedic01SMM, ambulance.Position);
				passenger.SetIntoVehicle(ambulance, VehicleSeat.RightRear);
				passenger.SetFreeWill(false);
				passenger.Task.LeaveVehicle();

				patient = await SpawnPed(GetRandomPed(), ambulance.Position);
				patient.SetIntoVehicle(ambulance, VehicleSeat.LeftRear);

				patient.Kill();
				patient.SetFreeWill(false);
			}
			else
			{
				WriteLine("!! Route was null");
				EndCallout();
			}
		}

		public override void OnStart(Ped player)
		{
			base.OnStart(player);

			ambulance!.IsEngineRunning = true;
			ambulance!.IsSirenSilent = false;
			foreach (VehicleDoor vehicleDoor in ambulance.Doors.GetAll()) vehicleDoor.Close();

			averageVelocity = 100f;

			Blip newBlip = CreateBlip(route!.End.Position);
			newBlip.Color = BlipColor.Red;
			newBlip.ShowRoute = true;

			Marker?.Delete();
			Marker = newBlip;

			ambulanceBlip = ambulance.AttachBlip();
			ambulanceBlip.Sprite = BlipSprite.Health;
			ambulanceBlip.Color = BlipColor.Green;

			passenger?.Task.EnterVehicle(ambulance, VehicleSeat.RightRear);

			API.SetBigmapActive(true, false);
		}

		private protected override async Task<bool> Progress()
		{
			switch (State)
			{
				case CalloutState.Accepted:
					State = CalloutState.Enroute;

					ShowNotification("Drive to the hospital and escort the ambulance.");
					return false;

				case CalloutState.Enroute:
					if (!(route.Start.DistanceTo(Leader.Position) < StartDistance)) return false;

					State = CalloutState.Arrived;

					ShowNotification("Wait for the ambulance to ready up...");
					return false;

				case CalloutState.Arrived:
					ambulance!.Doors[VehicleDoorIndex.FrontLeftDoor].Close();
					passenger!.Task.EnterVehicle(ambulance, VehicleSeat.RightRear);

					await Delay(Utilities.RANDOM.Next(3000, 7000));

					State = CalloutState.TransportEnroute;

					ShowNotification("The ambulance will now follow the prrimary officer.");
					return false;

				case CalloutState.TransportEnroute:
					StuckTest();

					Vector3 followOffset = -Leader.CurrentVehicle.ForwardVector;
					float followSpeed = CalculateFollowSpeed(ambulance!, Leader, followOffset, (float) Speed.SuperFast);

					driver!.Task.DriveTo(
						ambulance,
						Leader.Position + followOffset,
						10f,
						followSpeed,
						(int) DrivingStyle.Rushed);

					if (!(GetDistance(ambulance!.Position, route.End.Position) < 50f)) return false;

					ambulance.IsSirenSilent = true;
					State = CalloutState.TransportArrived;

					ShowNotification("Wait for the ambulance to park.");
					parkStartTime = Game.GameTime;
					return false;

				case CalloutState.TransportArrived:
					ArrivedStuckTest();

					driver!.Task.DriveTo(
						ambulance,
						route!.End.Position,
						5f,
						(float) Speed.Normal,
						(int) DrivingStyle.ShortestPath);

					if (!(GetDistance(ambulance!.Position, route!.End.Position) < 10f)) return false;

					driver.Task.LeaveVehicle();
					passenger!.Task.LeaveVehicle();
					await Delay(2000);

					driver.Task.GoTo(ambulance.Position - ambulance.ForwardVector * 7f);
					passenger.Task.GuardCurrentPosition();
					await Delay(1000);

					driver.Task.GuardCurrentPosition();

					ShowNotification("Thank you for your assistance!");
					EndCallout();
					return true;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void OnCancelBefore() => ambulanceBlip?.Delete();

		public override void OnCancelAfter() => API.SetBigmapActive(false, false);

		private protected override bool LifelinessChecks()
		{
			try
			{
				return driver!.Exists() && !driver.IsDead &&
				       ambulance!.Exists() && ambulance.EngineHealth > 0f && ambulance.HealthFloat > 0f &&
				       ElapsedTime < 300000;
			}
			catch (NullReferenceException e)
			{
				WriteLine(e.ToString());
				return false;
			}
		}

		private void StuckTest()
		{
			if (ambulance == null) return;

			averageVelocity = averageVelocity * 0.8f + ambulance.Velocity.Length() * 0.2f;

			// If ambulance freshly spawned, temporarily disable collisions with players
			AssignedPlayers.ForEach(ped =>
			{
				Vehicle? pedVehicle = ped.CurrentVehicle;
				if (pedVehicle == null) return;

				bool toggle = !(averageVelocity > 70 &&
				                GetDistance(ambulance.Position, pedVehicle.Position) <
				                ambulance.Velocity.Length() + pedVehicle.Velocity.Length());
				ambulance.SetNoCollision(
					pedVehicle,
					toggle);
			});

			if (averageVelocity > MathUtil.Clamp(Leader.Velocity.Length() - 1f, 0f, 3f) &&
			    GetDistance(Leader.Position, ambulance.Position) < 200f) return;

			ambulance.Position =
				GetNextPositionOnStreet(Game.PlayerPed.Position - Game.PlayerPed.ForwardVector * 20f, true);
			ambulance.Heading = Leader.CurrentVehicle.Heading;

			averageVelocity = 100f;
		}

		private void ArrivedStuckTest()
		{
			if (Game.GameTime <= parkStartTime + 10000) return;
			
			ShowNotification("Seems the ambulance driver is drunk... You're off to go.");
			EndCallout();
		}

		private static float CalculateFollowSpeed(Entity follower, Entity destination, Vector3 followOffset,
			float speedCap)
		{
			float distance = GetDistance(follower.Position, destination.Position + followOffset);
			float speedDifference = (destination.Velocity - follower.Velocity).Length();

			return MathUtil.Clamp(distance - speedDifference, 0f, speedCap);
		}

		#endregion
	}
}