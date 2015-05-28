using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core.Math
{
	public class PortableLocation
	{
		[JsonProperty ("lat")]
		public double Latitude { get; set; } = 0;

		[JsonProperty ("long")]
		public double Longitude { get; set; } = 0;

		[JsonProperty ("timestamp")]
		public DateTime DateTime { get; set; } = default(DateTime);

		public PortableLocation ()
		{
		}
	}

	public class PortableLocationCollection
	{
		[JsonProperty ("locations")]
		List<PortableLocation> Locations { get; set; } = new List<PortableLocation> ();

		public PortableLocationCollection ()
		{
		}

		public void AddLocation (PortableLocation location)
		{
			Locations.Add (location);
		}
	}
}

