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

