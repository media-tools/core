using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Core.Media.Common
{
	public abstract class Album
	{
		[JsonProperty ("photos")]
		Photo[] Photos_JSON { get { return Photos?.SortByDate ()?.ToArray (); } set { Photos = new HashSet<Photo> (value); } }

		[JsonIgnore]
		public HashSet<Photo> Photos { get; set; } = new HashSet<Photo> ();

		[JsonProperty ("videos")]
		Video[] Videos_JSON { get { return Videos?.SortByDate ()?.ToArray (); } set { Videos = new HashSet<Video> (value); } }

		[JsonIgnore]
		public HashSet<Video> Videos { get; set; } = new HashSet<Video> ();

		public string Name { get; private set; }

		protected Album (string name)
		{
			Name = name;
		}

		public abstract void Load ();

		public virtual void AddPhoto (Photo photo)
		{
			Photos.Add (photo);
		}

		public virtual void AddVideo (Video video)
		{
			Videos.Add (video);
		}

		public override string ToString ()
		{
			return string.Format ("[Album: Name={0}, Photos={1}, Videos={2}]", Name, Photos.Count, Videos.Count);
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

