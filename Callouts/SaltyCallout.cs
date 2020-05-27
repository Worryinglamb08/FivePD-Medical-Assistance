using System;
using System.Threading.Tasks;
using CalloutAPI;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.BaseScript;
using static CitizenFX.Core.Debug;

namespace SaltyCallouts_FivePD.Utils
{
	public abstract class SaltyCallout : Callout
	{
		private protected abstract int ProcessTickDelay { get; set; }

		protected int ElapsedTime => Game.GameTime - startTime;
		private protected Ped Leader => AssignedPlayers[0];

		private int startTime;
		private CalloutState state = CalloutState.Accepted;

		protected CalloutState State
		{
			get => state;
			set
			{
				Screen.ShowNotification($"Callout state changed:\n{value.ToString()}");
				state = value;
			}
		}

		protected SaltyCallout(string name) => WriteLine($"Initializing SaltyCallout {name}");

		public override void OnStart(Ped closest)
		{
			base.OnStart(closest);

			startTime = Game.GameTime;
			WriteLine($"Starting callout at {startTime} by {closest}...");

			Tick += Process;
		}

		private async Task Process()
		{
			try
			{
				if (!LifelinessChecks())
				{
					WriteLine("LifelinessCheck failed! Ending callout...");
					EndCallout();
				}

				if (await Progress())
				{
					WriteLine("Progress complete. Ending callout...");
					EndCallout();
				}

				await Delay(ProcessTickDelay);
			}
			catch (Exception e)
			{
				WriteLine(e.ToString());
				EndCallout();
			}
		}

		/// <summary>
		/// A ticked function for progress checking.
		/// </summary>
		/// <returns>Return true when callout is finished</returns>
		private protected virtual async Task<bool> Progress() => false;

		/// <summary>
		/// Essential liveness checks of the callout should be implemented here, e.g. if all important peds, vehciles etc.
		/// still exists and are alive.
		/// </summary>
		/// <returns>Return false if something is broken. Automatically ends the callout.</returns>
		private protected virtual bool LifelinessChecks() => true;

		protected enum CalloutState
		{
			Accepted,
			Enroute,
			Arrived,
			Chase,
			TransportEnroute,
			TransportArrived
		}
	}
}