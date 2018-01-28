using System;
using System.Globalization;

namespace Lupidan.UniNLocation
{
	public struct Location
	{
		public readonly double Latitude;
		public readonly double Longitude;
		public readonly double Altitude;
		public readonly DateTime Timestamp;
		public readonly double LatitudeRad;
		public readonly double LongitudeRad;

		public Location(double latitude, double longitude, double altitude, DateTime timestamp)
		{
			Latitude = latitude;
			Longitude = longitude;
			Altitude = altitude;
			Timestamp = timestamp;
			LatitudeRad = latitude * (Math.PI / 180.0);
			LongitudeRad = longitude * (Math.PI / 180.0);
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}, {2}] {3}", Timestamp.ToString("o", CultureInfo.InvariantCulture), Latitude, Longitude, Altitude);
		}
	}
}
