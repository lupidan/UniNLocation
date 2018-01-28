using System;
using System.Timers;

namespace Lupidan.UniNLocation
{
	public class MockLocationManager : LocationManager
	{
		private readonly Random _random = new Random(20);
		private readonly Timer _activeTimer = new Timer();
		
		#region LocationManager implementation
		
		public event Action<Location> LocationReceived;
		public event Action<string, long> ErrorReceived;
		public LocationAuthorizationStatus DeviceAuthorizationStatus
		{
			get { return LocationAuthorizationStatus.Accepted; }
		}

		public LocationAuthorizationStatus ApplicationAuthorizationStatus
		{
			get { return LocationAuthorizationStatus.Accepted; }
		}

		public void StartTracking()
		{
			_activeTimer.Interval = 1000.0;
			_activeTimer.Elapsed += EmitRandomLocation;
			_activeTimer.Start(); 
		}

		public void StopTracking()
		{
			_activeTimer.Stop();
		}
		
		#endregion

		private void EmitRandomLocation(object sender, ElapsedEventArgs args)
		{
			var randomLocation = new Location(_random.NextDouble(), _random.NextDouble(), _random.NextDouble(), DateTime.Now);
			if (LocationReceived != null)
				LocationReceived(randomLocation);
			
			_activeTimer.Start();
		}
	}
}
