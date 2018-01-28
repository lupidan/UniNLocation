using System;
using System.Runtime.InteropServices;
using AOT;

namespace Lupidan.NativeLocation
{

	public class IOSLocationManager : LocationManager
	{
		private delegate void ReceivedLocationDelegate(double latitude, double longitude, double altitude, double timestamp);
		
		private static IOSLocationManager ActiveLocationManager;
		private Action<Location> _locationReceived;
		
		#region LocationManager implementation
		
		public event Action<Location> LocationReceived
		{
			add { _locationReceived += value;}
			remove { _locationReceived -= value; }
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
			IOSLocationWrapperStartTrackingLocation(TrackingLocationCallback);
		}

		public void StopTracking()
		{
			IOSLocationWrapperStopTrackingLocation();
			ActiveLocationManager = null;
		}
		
		[MonoPInvokeCallback(typeof(ReceivedLocationDelegate))]
		public static void  TrackingLocationCallback(double latitude, double longitude, double altitude, double timestamp)
		{
			var location = new Location(latitude, longitude, altitude, DateTime.FromOADate(timestamp));
			if (ActiveLocationManager._locationReceived != null)
				ActiveLocationManager._locationReceived(location);
		}
		
		#endregion

		[DllImport("__Internal")]
		private static extern int IOSLocationWrapperGetDeviceAuthorizationStatus();
		
		[DllImport("__Internal")]
		private static extern int IOSLocationWrapperGetApplicationAuthorizationStatus();
		
		[DllImport("__Internal")]
		private static extern void IOSLocationWrapperStartTrackingLocation(ReceivedLocationDelegate callback);
		
		[DllImport("__Internal")]
		private static extern void IOSLocationWrapperStopTrackingLocation();
		
	}

}