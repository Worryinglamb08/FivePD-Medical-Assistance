using static CitizenFX.Core.VehicleDrivingFlags;

namespace SaltyCallouts_FivePD.Utils
{
	enum DrivingStyle : uint
	{
		EmergencySafe = AvoidObjects | AvoidPeds | AvoidVehicles | AvoidEmptyVehicles | AllowMedianCrossing,
		EmergencyUnsafe = EmergencySafe | AllowGoingWrongWay
	}
}