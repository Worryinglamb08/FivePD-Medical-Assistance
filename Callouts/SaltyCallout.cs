using System.Threading.Tasks;
using CalloutAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Debug;

namespace SaltyCallouts_FivePD.Utils
{
	public abstract class SaltyCallout : Callout
	{
		protected float Tolerance { get; } = 5f;

		protected int ElapsedTime => Game.GameTime - startTime;

		protected int progress;
		private int startTime;

		protected SaltyCallout(string name)
		{
			WriteLine($"Initializing SaltyCallout {name}");
		}

		public override void OnStart(Ped closest)
		{
			base.OnStart(closest);

			startTime = Game.GameTime;
			WriteLine($"Starting callout at {startTime}...");

			Tick += Process;
		}

		private Task Process()
		{
			if (!LifelinessChecks())
			{
				WriteLine("LifelinessCheck failed! Ending callout...");
				EndCallout();
			}

			if (Progress())
			{
				WriteLine("Progress complete. Ending callout...");
				EndCallout();
			}

			return BaseScript.Delay(1000);
		}

		/// <summary>
		/// A ticked function for progress checking.
		/// Runs once a second.
		/// </summary>
		/// <returns>Return true when callout is finished</returns>
		private protected virtual bool Progress() => false;

		/// <summary>
		/// Essential liveness checks of the callout should be implemented here, e.g. if all important peds, vehciles etc.
		/// still exists and are alive.
		/// Runs once a second.
		/// </summary>
		/// <returns>Return false if something is broken. Automatically ends the callout.</returns>
		private protected virtual bool LifelinessChecks() => true;
	}
}