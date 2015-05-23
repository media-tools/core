using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common
{
	public static class CollectionExtensions
	{
		public static IEnumerable<T> TakeLast<T> (this IEnumerable<T> source, int N)
		{
			var enumerable = source as T[] ?? source.ToArray ();
			return enumerable.Skip (System.Math.Max (0, enumerable.Count () - N));
		}

		public static string Join<T> (this IEnumerable<T> enumerable, string delimiter)
		{
			return string.Join (delimiter, enumerable.Select (e => e.ToString ()).ToArray ());
		}

		public static T[] Extend<T> (this T[] originalArray, params T[] addItem) where T : class
		{
			if (addItem == null) {
				throw new ArgumentNullException ("addItem");
			}
			var arr = addItem;
			if (originalArray == null) {
				return arr;
			}
			return originalArray.Concat (arr).ToArray (); // although Concat is not recommended for performance reasons, see the accepted answer
		}
	}
}

