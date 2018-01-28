
using Lupidan.NativeLocation;
using UnityEngine;

public class LocationTesterClass : MonoBehaviour
{
	private LocationManager _locationManager;
	
	private void Awake()
	{
		this._locationManager = new IOSLocationManager();
	}

	public void OnStartPressed()
	{
		this._locationManager.LocationReceived += LocationManagerOnLocationReceived;
		this._locationManager.StartTracking();

		if (_locationManager.DeviceAuthorizationStatus != LocationAuthorizationStatus.Accepted)
		{
			
		}
		else if (_locationManager.ApplicationAuthorizationStatus == LocationAuthorizationStatus.Denied)
		{
			
		}
		else
	}

	public void OnStopPressed()
	{
		this._locationManager.LocationReceived -= LocationManagerOnLocationReceived;
		this._locationManager.StopTracking();
	}
	
	private void LocationManagerOnLocationReceived(Location location)
	{
		Debug.Log("Received location " + location);
	}

}
