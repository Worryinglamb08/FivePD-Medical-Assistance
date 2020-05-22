using System.Threading.Tasks;
using CalloutAPI;
using CitizenFX.Core;

namespace SaltyCallouts_FivePD.Utils
{
	public abstract class SaltyCallout : Callout
	{
		public int ElapsedTime => Game.GameTime - startTime;

		protected int progress;
		private int startTime;

		public override void OnStart(Ped closest)
		{
			base.OnStart(closest);
			
			startTime = Game.GameTime;
			Tick += Process;
		}

		/*
		public override void OnStart(Ped closest)
		{
			base.OnStart(closest);

			startTime = Game.GameTime;
			Tick += Process;
		}
		*/

		private async Task Process()
		{
			if (!LifelinessChecks() || Progress()) EndCallout();
			await Task.FromResult(0);
		}

		/// <summary>
		/// A ticked function for progress checking.
		/// </summary>
		/// <returns>Return true when callout is finished</returns>
		private protected virtual bool Progress() => false;

		/// <summary>
		/// Essential liveness checks of the callout should be implemented here, e.g. if all important peds, vehciles etc.
		/// still exists and are alive.
		/// </summary>
		/// <returns>Return false if something is broken. Automatically ends the callout.</returns>
		private protected virtual bool LifelinessChecks() => true;
	}
}