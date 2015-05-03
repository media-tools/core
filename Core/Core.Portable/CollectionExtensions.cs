using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common
{
	public static class CollectionExtensions
	{
		public static IEnumerable<T> TakeLast<T> (IEnumerable<T> source, int N)
		{
			return source.Skip (Math.Max (0, source.Count () - N));
		}
	}
}

