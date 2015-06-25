using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Core.Media
{
	public class Album
	{
		[JsonProperty ("photos")]
		Photo[] Photos_Internal { get { return Photos?.SortByDate ()?.ToArray (); } set { Photos = new HashSet<Photo> (value); } }

		[JsonIgnore]
		public HashSet<Photo> Photos { get; set; } = new HashSet<Photo> ();


		[JsonProperty ("videos")]
		Video[] Videos_Internal { get { return Videos?.SortByDate ()?.ToArray (); } set { Videos = new HashSet<Video> (value); } }

		[JsonIgnore]
		public HashSet<Video> Videos { get; set; } = new HashSet<Video> ();

		public Album ()
		{
		}

		public void AddPhoto (Photo photo)
		{
			Photos.Add (photo);
		}
	}

	public static class CollectionExtensions
	{
		public static IEnumerable<T> SortByDate<T> (this IEnumerable<T> enumerable) where T : Medium
		{
			return enumerable.OrderBy (l => l.DateTimeLocal);
		}
	}
}

