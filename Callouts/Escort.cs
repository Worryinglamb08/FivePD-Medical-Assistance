using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CalloutAPI;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using SaltyCallouts_FivePD.Utils;
using static CitizenFX.Core.World;
using static SaltyCallouts_FivePD.Utils.Collections;
using static SaltyCallouts_FivePD.Utils.SpawnPoint.Spawns.Airports;
using static SaltyCallouts_FivePD.Utils.SpawnPoint.Spawns.Hospitals;

namespace SaltyCallouts_FivePD.Callouts
{
	[CalloutProperties("Medical Escort", "Salzian", "0.1", Probability.High)]
	[SuppressMessage("ReSharper", "UnusedType.Global")]
	public class MedicalEscort : SaltyCallout
	{
		private Ped driver, passenger, patient;
		private Vehicle ambulance;
		private readonly Vector3 target = Lsia.EMS_ARRIVAL.position;
		private const float ArrivalRadius = 60f;
		private Blip ambulanceBlip;

		public MedicalEscort()
		{
			InitBase(CentralLosSantosMedicalCenter.TRANSFER.position);

			ShortName = "Medical Escort";
			CalloutDescription = "A critical patient needs to be escorted from the hospital to LSIA";
			ResponseCode = 2;
			StartDistance = 20f;
			FixedLocation = true;
		}

		public override async Task Init()
		{
			OnAccept();

			ambulance = await SpawnVehicle(
				VehicleHash.Ambulance,
				CentralLosSantosMedicalCenter.TRANSFER.position,
				CentralLosSantosMedicalCenter.TRANSFER.heading);
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

		public override void OnStart(Ped player)
		{
			base.OnStart(player);

			ambulance.IsSirenSilent = false;
			foreach (VehicleDoor vehicleDoor in ambulance.Doors.GetAll()) vehicleDoor.Close();

			Blip newBlip = CreateBlip(target);
			newBlip.Color = BlipColor.Red;
			newBlip.ShowRoute = true;

			Marker?.Delete();
			Marker = newBlip;
			
			ambulanceBlip = ambulance.AttachBlip();
			ambulanceBlip.Sprite = BlipSprite.Health;
			ambulance.IsEngineRunning = true;

			passenger.Task.EnterVehicle(ambulance, VehicleSeat.RightRear);

			var waitForPartnerAndDepart = new TaskSequence();
			waitForPartnerAndDepart.AddTask.Wait(5000);
			waitForPartnerAndDepart.AddTask.DriveTo(ambulance, target, ArrivalRadius, (float) Speed.Fast,
				(int) DrivingStyle.Rushed);
			waitForPartnerAndDepart.Close();
			driver.Task.PerformSequence(waitForPartnerAndDepart);
		}

		private protected override bool Progress()
		{
			switch (progress)
			{
				case 0:
					if (GetDistance(ambulance.Position, target) > ArrivalRadius) return false;

					driver.Task.ParkVehicle(ambulance, Lsia.EMS_ARRIVAL.position, Lsia.EMS_ARRIVAL.heading, 5f, true);
					ambulance.IsSirenSilent = true;
					progress++;
					return false;

				case 1:
					if (GetDistance(ambulance.Position, target) > 5f) return false;

					ambulance.EngineHealth = 0f;
					ambulance.IsPositionFrozen = true;
					driver.Task.LeaveVehicle();
					passenger.Task.LeaveVehicle();

					Screen.ShowNotification("The patient has been delivered.");
					return true;

				default:
					return true;
			}
		}

		public override void OnCancelBefore()
		{
			base.OnCancelBefore();

			ambulanceBlip?.Delete();
		}

		public override void OnCancelAfter()
		{
			base.OnCancelAfter();

			driver?.Task.Wait(10000);
			passenger?.Task.Wait(10000);
			patient?.Task.Wait(10000);
		}

		private protected override bool LifelinessChecks() =>
			driver.Exists() && !driver.IsDead &&
			ambulance.Exists() && ambulance.EngineHealth > 0f && ambulance.HealthFloat > 0f &&
			ElapsedTime < 300000; // 5 minute circuit breaker
	}
}