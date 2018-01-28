using System;

namespace Lupidan.UniNLocation
{
	public struct Location
	{
		public double Latitude;
		public double Longitude;
		public double Altitude;
		public DateTime Timestamp;

		public Location(double latitude, double longitude, double altitude, DateTime timestamp)
		{
			Latitude = latitude;
			Longitude = longitude;
			Altitude = altitude;
			Timestamp = timestamp;
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}, {2}] {3}", Timestamp, Latitude, Longitude, Altitude);
		}
	}
}
