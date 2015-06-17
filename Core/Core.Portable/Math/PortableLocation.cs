using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core.Math
{
	public class PortableLocation
	{
		[JsonProperty ("latitude")]
		public double Latitude { get; set; } = 0;

		[JsonProperty ("longitude")]
		public double Longitude { get; set; } = 0;

		[JsonProperty ("altitude")]
		public double Altitude { get; set; } = 0;

		[JsonProperty ("timestamp")]
		public DateTime DateTime { get; set; } = default(DateTime);

		[JsonProperty ("provider")]
		public string Provider { get; set; } = "";

		[JsonProperty ("reference_file")]
		public string ReferenceFile { get; set; } = "";

		[JsonProperty ("lat")]
		private double LatitudeAlternateSetter {
			// get is intentionally omitted here
			set { Latitude = value; }
		}

		[JsonProperty ("long")]
		private double LongitudeAlternateSetter {
			// get is intentionally omitted here
			set { Longitude = value; }
		}

		public PortableLocation ()
		{
		}

		public override string ToString ()
		{
			return string.Format ("[PortableLocation: Latitude={0}, Longitude={1}, Altitude={2}, DateTime={3}, Provider={4}]", Latitude, Longitude, Altitude, DateTime, Provider);
		}

		public override bool Equals (object obj)
		{
			PortableLocation other = obj as PortableLocation;
			if (other != null) {
				return System.Math.Abs (Latitude - other.Latitude) < EPSILON && System.Math.Abs (Longitude - other.Longitude) < EPSILON && System.Math.Abs (Altitude - other.Altitude) < EPSILON && DateTime == other.DateTime && Provider == other.Provider;
			} else {
				return false;
			}
		}

		public override int GetHashCode ()
		{
			return (int)(Latitude + Longitude * 100.0 + Altitude / 100.0 + DateTime.Ticks);
		}

		static readonly double EPSILON = 0.000001;
	}

	public class PortableLocationCollection
	{
		[JsonProperty ("locations")]
		public List<PortableLocation> Locations { get; set; } = new List<PortableLocation> ();

		public PortableLocationCollection ()
		{
		}

		public void AddLocation (PortableLocation location)
		{
			Locations.Add (location);
		}
	}
}

