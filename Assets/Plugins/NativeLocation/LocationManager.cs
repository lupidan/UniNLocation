using System;

namespace Lupidan.NativeLocation
{
	public interface LocationManager
	{
		event Action<Location> LocationReceived;
		LocationAuthorizationStatus DeviceAuthorizationStatus { get; }
		LocationAuthorizationStatus ApplicationAuthorizationStatus { get; }
		
		void StartTracking();
		void StopTracking();
	}
}


