using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using SaltyCallouts_FivePD.Utils;

namespace SaltyCallouts_FivePD.Routing
{
	internal class RouteCollection
	{
		private List<Route> Routes { get; }

		public RouteCollection(List<Route> locations) => Routes = locations;
		public RouteCollection(params Route[] routes) => Routes = routes.ToList();

		internal Route GetNearestRandomRoute(float min, float max)
		{
			if (Routes.Count == 0) throw new Exception("Routes was empty");

			var possibleRoutes = Routes.Where(route =>
				route.Start.DistanceToLocalPlayer > min && route.Start.DistanceToLocalPlayer < max).ToArray();

			if (possibleRoutes.Length != 0)
				return possibleRoutes.OrderBy(route => route.Start.DistanceToLocalPlayer).ToList().GetRandom();

			var possibleRoute = Routes.Where(route => route.Start.DistanceToLocalPlayer > min).ToList().GetRandom();
			return possibleRoute ?? Routes.OrderBy(route => route.Start.DistanceToLocalPlayer).First();
		}

		private class InvalidRouteException : Exception
		{
			public InvalidRouteException(string message) : base(message)
			{
			}
		}
	}
}