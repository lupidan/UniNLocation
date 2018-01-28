using System;

namespace Lupidan.UniNLocation
{
	public interface LocationManager
	{
		event Action<Location> LocationReceived;
		event Action<string, long> ErrorReceived;
		LocationAuthorizationStatus DeviceAuthorizationStatus { get; }
		LocationAuthorizationStatus ApplicationAuthorizationStatus { get; }

		void RequestLocationPermissions();
		void GoToApplicationSettings();
		
		void StartTracking();
		void StopTracking();
	}
}


