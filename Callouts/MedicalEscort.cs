using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CalloutAPI;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using SaltyCallouts_FivePD.Routing;
using SaltyCallouts_FivePD.Utils;
using static CitizenFX.Core.Debug;
using static CitizenFX.Core.UI.Screen;
using static CitizenFX.Core.World;
using static SaltyCallouts_FivePD.Utils.Collections;

namespace SaltyCallouts_FivePD.Callouts
{
	[CalloutProperties("Medical Escort", "Salzian", "0.2", Probability.High)]
	[SuppressMessage("ReSharper", "UnusedType.Global")]
	public class MedicalEscort : SaltyCallout
	{
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
// private readonly bool fail;

		#region Callout

		public MedicalEscort() : base("Medical Escort")
		{
			// TODO Revert max once CheckRequirements is fixed https://github.com/KDani-99/FivePD-API/issues/43
			route = ROUTE_COLLECTION.GetNearestRandomRoute(200f, 3000f);

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
			StartDistance = 30;
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

			var waitForPartnerAndDepart = new TaskSequence();
			waitForPartnerAndDepart.AddTask.Wait(5000);
			waitForPartnerAndDepart.AddTask.DriveTo(
				ambulance,
				route!.End.Position,
				0f,
				(float) Speed.Fast,
				(int) RageVehicleDrivingFlags.Emergency);
			waitForPartnerAndDepart.Close();

			driver!.Task.PerformSequence(waitForPartnerAndDepart);

			API.SetBigmapActive(true, false);
		}

		private protected override bool Progress()
		{
			StuckTest();

			switch (progress)
			{
				case 0:
					if (GetDistance(ambulance!.Position, route!.End.Position) > 100)
						return false;

					ShowNotification("Arrived at destination. Wait for ambulance to park.");

					ClearPosition(route!.End.Position, 10f);
					ambulance.IsSirenSilent = true;
					driver?.Task.DriveTo(ambulance, route.End.Position, 0f, (float) Speed.Slow);

					progress++;
					return false;

				case 1:
					if (GetDistance(ambulance!.Position, route!.End.Position) > 5f) return false;

					ShowNotification("The patient has been delivered. Thank you for your assistance.");

					patient?.Delete();
					return true;

				default:
					return true;
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

			if (!(averageVelocity <= 5)) return;

			ambulance.Position =
				GetNextPositionOnStreet(Game.PlayerPed.Position - Game.PlayerPed.ForwardVector * 20f, true);

			averageVelocity = 100f;
		}

		private async void ClearPosition(Vector3 position, float radius)
		{
			foreach (Vehicle filteredVehicle in GetAllVehicles()
				.Where(vehicle => vehicle.IsInRangeOf(position, radius))
			)
			{
				if (filteredVehicle.Driver == null) await CreatePed(GetRandomPed(), filteredVehicle.Position);
				filteredVehicle.Driver?.Task.CruiseWithVehicle(filteredVehicle, (float) Speed.Normal);
			}
		}

		public override void OnCancelBefore()
		{
			ambulanceBlip?.Delete();
			API.SetBigmapActive(false, false);
		}

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

		#endregion
	}
}