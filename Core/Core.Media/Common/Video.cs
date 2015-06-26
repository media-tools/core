using System;
using Core.Math;
using Newtonsoft.Json;

namespace Core.Media.Common
{
	public class Video : Medium
	{
		[JsonProperty ("dimensions")]
		public PhotoDimensions Dimensions { get; set; } = null;

		[JsonProperty ("location")]
		public PortableLocation Location { get; set; } = null;

		public override string ToString ()
		{
			return string.Format ("[Video: Filename={0}, DateTimeLocal={1}, Location={2}]", Filename, DateTimeLocal, Location);
		}

		// Analysis disable once MemberHidesStaticFromOuterClass
		public override bool Equals (object obj)
		{
			Video other = obj as Video;
			if (other != null) {
				return Filename == other.Filename
				&& DateTimeLocal == other.DateTimeLocal
				&& Location == other.Location;
			} else {
				return false;
			}
		}

		public override int GetHashCode ()
		{
			return Filename.GetHashCode ();
		}
	}
}

