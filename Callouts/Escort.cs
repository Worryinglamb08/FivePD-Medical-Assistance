using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CalloutAPI;
using CitizenFX.Core;
using SaltyCallouts_FivePD.Utils;
using static CitizenFX.Core.World;
using static SaltyCallouts_FivePD.Utils.SpawnPoint.Spawns.Airports;
using static SaltyCallouts_FivePD.Utils.SpawnPoint.Spawns.Hospitals;
using DrivingStyle = SaltyCallouts_FivePD.Utils.DrivingStyle;

namespace SaltyCallouts_FivePD.Callouts
{
	[CalloutProperties("Medical Escort", "Salzian", "0.1", Probability.High)]
	[SuppressMessage("ReSharper", "UnusedType.Global")]
	public class MedicalEscort : SaltyCallout
	{
		private Ped driver, passenger, patient;
		private Vehicle ambulance;
		private readonly Vector3 target = Lsia.EMS_ARRIVAL.position;
		private readonly float arrivalRadius = 30f;

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
			ambulance.IsEngineRunning = true;
			ambulance.Doors[VehicleDoorIndex.BackLeftDoor].Open(instantly: true);
			ambulance.Doors[VehicleDoorIndex.BackRightDoor].Open(instantly: true);


			driver = await SpawnPed(PedHash.Paramedic01SMM, ambulance.Position);
			driver.SetIntoVehicle(ambulance, VehicleSeat.LeftRear);
			driver.SetFreeWill(false);
			driver.Task.LeaveVehicle(LeaveVehicleFlags.WarpOut);

			passenger = await SpawnPed(PedHash.Paramedic01SMM, ambulance.Position);
			passenger.SetIntoVehicle(ambulance, VehicleSeat.RightRear);
			driver.SetFreeWill(false);

			patient = await SpawnPed(GetRandomPed(), ambulance.Position);
			patient.SetIntoVehicle(ambulance, VehicleSeat.RightRear);
			patient.SetFreeWill(false);
		}

		public override void OnStart(Ped player)
		{
			base.OnStart(player);

			driver.Task.DriveTo(ambulance, target, arrivalRadius, (float) Speed.Rushed,
				(int) DrivingStyle.EmergencySafe);

			ambulance.IsSirenSilent = false;
			foreach (VehicleDoor vehicleDoor in ambulance.Doors.GetAll()) vehicleDoor.Close();

			Blip newBlip = CreateBlip(target);
			newBlip.Color = BlipColor.Red;
			newBlip.ShowRoute = true;

			Marker?.Delete();
			Marker = newBlip;
		}

		private protected override bool Progress()
		{
			switch (progress)
			{
				case 0:
					if (GetDistance(ambulance.Position, target) > arrivalRadius) return false;

					driver.Task.ParkVehicle(ambulance, Lsia.EMS_ARRIVAL.position, Lsia.EMS_ARRIVAL.heading, 5f, true);
					progress++;
					return false;

				case 1:
					if (GetDistance(ambulance.Position, target) > 5f) return false;

					ambulance.EngineHealth = 0f;
					driver.Task.LeaveVehicle();
					passenger.Task.LeaveVehicle();
					return true;

				default:
					return true;
			}
		}

		private protected override bool LifelinessChecks() =>
			driver.Exists() && !driver.IsDead &&
			ambulance.Exists() && ambulance.EngineHealth > 0f && ambulance.HealthFloat > 0f &&
			ElapsedTime < 300000; // 5 minute circuit breaker
	}
}