using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common
{
	public static class CollectionExtensions
	{
		public static IEnumerable<T> TakeLast<T> (this IEnumerable<T> source, int N)
		{
			return source.Skip (Math.Max (0, source.Count () - N));
		}

		public static string Join<T> (this IEnumerable<T> enumerable, string delimiter)
		{
			return string.Join (delimiter, enumerable.Select (e => e.ToString ()).ToArray ());
		}
	}
}

