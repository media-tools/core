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

		public static T[] Extend<T> (this T[] firstArray, params T[] secondArray) where T : class
		{
			if (secondArray == null) {
				throw new ArgumentNullException ("secondArray");
			}
			if (firstArray == null) {
				return secondArray;
			}
			return firstArray.Concat (secondArray).ToArray (); // although Concat is not recommended for performance reasons
		}

		public static T[] Extend<T> (this T firstItem, params T[] secondArray) where T : class
		{
			if (secondArray == null) {
				throw new ArgumentNullException ("secondArray");
			}
			if (firstItem == null) {
				return secondArray;
			}
			return new T[]{ firstItem }.Concat (secondArray).ToArray (); // although Concat is not recommended for performance reasons
		}
	}
}

