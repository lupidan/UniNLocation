using System;
using System.Runtime.InteropServices;
using AOT;

namespace Lupidan.NativeLocation
{
	public class IOSLocationManager : LocationManager
	{
		private delegate void LocationReceivedDelegate(double latitude, double longitude, double altitude, double timestamp);
		private delegate void LocationErrorDelegate(string errorMessage, long errorCode);
		
		private static IOSLocationManager ActiveLocationManager;
		private Action<Location> _locationReceived;
		private Action<string, long> _errorReceived;
		
		#region LocationManager implementation
		
		public event Action<Location> LocationReceived
		{
			add { _locationReceived += value;}
			remove { _locationReceived -= value; }
		}
		
		public event Action<string, long> ErrorReceived
		{
			add { _errorReceived += value;}
			remove { _errorReceived -= value; }
		}

		public LocationAuthorizationStatus DeviceAuthorizationStatus
		{
			get { return (LocationAuthorizationStatus) IOSLocationWrapperGetDeviceAuthorizationStatus(); }
		}
		
		public LocationAuthorizationStatus ApplicationAuthorizationStatus
		{
			get { return (LocationAuthorizationStatus) IOSLocationWrapperGetApplicationAuthorizationStatus(); }
		}

		public void StartTracking()
		{
			ActiveLocationManager = this;
			IOSLocationWrapperStartTrackingLocation(LocationReceivedCallback, LocationErrorCallback);
		}

		public void StopTracking()
		{
			IOSLocationWrapperStopTrackingLocation();
			ActiveLocationManager = null;
		}
		
		#endregion
		
		#region C# => Objective C calls

		[DllImport("__Internal")]
		private static extern int IOSLocationWrapperGetDeviceAuthorizationStatus();
		
		[DllImport("__Internal")]
		private static extern int IOSLocationWrapperGetApplicationAuthorizationStatus();

		[DllImport("__Internal")]
		private static extern void IOSLocationWrapperRequestApplicationPermissions();

		[DllImport("__Internal")]
		private static extern void IOSLocationWrapperGoToApplicationSettings();
		
		[DllImport("__Internal")]
		private static extern void IOSLocationWrapperStartTrackingLocation(LocationReceivedDelegate locationCallback, LocationErrorDelegate errorCallback);
		
		[DllImport("__Internal")]
		private static extern void IOSLocationWrapperStopTrackingLocation();
		
		#endregion
		
		#region Objective C => C# callbacks
		
		[MonoPInvokeCallback(typeof(LocationReceivedDelegate))]
		private static void  LocationReceivedCallback(double latitude, double longitude, double altitude, double timestamp)
		{
			var location = new Location(latitude, longitude, altitude, DateTime.FromOADate(timestamp));
			if (ActiveLocationManager._locationReceived != null)
				ActiveLocationManager._locationReceived(location);
		}
		
		[MonoPInvokeCallback(typeof(LocationErrorDelegate))]
		private static void  LocationErrorCallback(string error, long errorCode)
		{
			if (ActiveLocationManager._errorReceived != null)
				ActiveLocationManager._errorReceived(error, errorCode);
		}
		
		#endregion
	}
}
