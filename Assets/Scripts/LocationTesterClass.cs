
using System;
using Lupidan.UniNLocation;
using UnityEngine;

public class LocationTesterClass : MonoBehaviour
{
	private LocationManager _locationManager;

	private Location? _previousLocation;
	private double kilometersAmount = 0.0;
	
	private void Awake()
	{
		#if UNITY_IOS
		_locationManager = new IOSLocationManager();
		#else
		_locationManager = new MockLocationManager();
		#endif
	}

	public void OnStartPressed()
	{
		if (_locationManager.DeviceAuthorizationStatus != LocationAuthorizationStatus.Accepted)
		{
			Debug.Log("Please, enable location services on your device!");
		}
		else if (_locationManager.ApplicationAuthorizationStatus == LocationAuthorizationStatus.NotDetermined)
		{
			_locationManager.RequestLocationPermissions();
		}
		else if (_locationManager.ApplicationAuthorizationStatus == LocationAuthorizationStatus.Denied)
		{
			_locationManager.GoToApplicationSettings();
		}
		else
		{
			_locationManager.LocationReceived += LocationManagerOnLocationReceived;
			_locationManager.ErrorReceived += LocationManagerOnErrorReceived;
			_locationManager.StartTracking();
		}
	}

	public void OnStopPressed()
	{
		_locationManager.LocationReceived -= LocationManagerOnLocationReceived;
		_locationManager.ErrorReceived -= LocationManagerOnErrorReceived;
		_locationManager.StopTracking();
	}
	
	private void LocationManagerOnLocationReceived(Location location)
	{
		if (_previousLocation.HasValue)
		{
			var distance = _previousLocation.Value.DistanceInKilometersTo(location);
			kilometersAmount += distance;
			Debug.Log("ADDING " + distance);
		}
		
		_previousLocation = location;
		
		Debug.Log("Received location " + location);
	}
	
	private void LocationManagerOnErrorReceived(string errorMessage, long errorCode)
	{
		Debug.Log("Error: " + errorMessage + " :: " + errorCode);
	}

}
