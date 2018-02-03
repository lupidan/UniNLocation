using System;
using System.Collections;
using System.Collections.Generic;
using Lupidan.UniNLocation;

public static class LocationUtil
{
	private const double EarthRadius = 6372.8;
	
	public static double DistanceInKilometersTo(this Location from, Location to)
	{
		// http://www.movable-type.co.uk/scripts/latlong.html
		// Using the "Equirectangular approximation"
		// Suitable for short distances
		
		var x = (to.LongitudeRad - from.LongitudeRad) * Math.Cos((from.LatitudeRad + to.LatitudeRad) / 2.0);
		var y = (to.LatitudeRad - from.LatitudeRad);
		return Math.Sqrt((x * x) + (y * y)) * EarthRadius;
	} 
	
	public static double AccurateDistanceInKilometersTo(this Location from, Location to)
	{
		// http://www.movable-type.co.uk/scripts/latlong.html
		// Using the "Haversine" formula
		
		var dLatitude = (to.LatitudeRad - from.LatitudeRad);
		var dLatitudeHalf = dLatitude / 2.0;
		var dLongitude = (to.LongitudeRad - from.LongitudeRad);
		var dLongitudeHalf = dLongitude / 2.0;

		var dLatitudeHalfSin = Math.Sin(dLatitudeHalf);
		var dLongitudeHalfSin = Math.Sin(dLongitudeHalf);

		var a = dLatitudeHalfSin * dLatitudeHalfSin +
		        Math.Cos(from.LatitudeRad) * Math.Cos(to.LatitudeRad) *
		        dLongitudeHalfSin * dLongitudeHalfSin;
		var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
		return EarthRadius * c;
	}
}
