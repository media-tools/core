using System;
using System.Collections.Generic;
using Core.Common;
using System.Linq;

namespace Core.IO
{
	public static class PathHelper
	{
		public static readonly char[] PathSeparatorChars = new char[] { '/', '\\' };

		public static string GetFileName (string path)
		{
			if (string.IsNullOrEmpty (path))
				return path;

			//if (path.IndexOfAny (InvalidPathChars) != -1)
			//	throw new ArgumentException ("Illegal characters in path.");

			int nLast = path.LastIndexOfAny (PathSeparatorChars);
			if (nLast >= 0)
				return path.Substring (nLast + 1);

			return path;
		}

		public static string GetFileName (string[] path)
		{
			if (path.Length == 0)
				return string.Empty;
			
			return path.Last ();
		}

		public static string NormalizePath (string path)
		{
			Log.Debug ("PathHelper.NormalizePath: path=", path);
			if (string.IsNullOrWhiteSpace (path))
				return string.Empty;

			// no windows file separators!
			path = path.Replace ('\\', '/');

			// no trailing slash!
			//if (path.ToCharArray ().Any (c => c != '/')) {
			//	path = path.TrimEnd ('/');
			//}

			bool startsWithSlash = path.StartsWith ("/");
			
			string[] oldParts = path.Split (new []{ '/' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> newParts = new List<string> ();
			foreach (string part in oldParts) {
				if (part == ".") {
					// ignore
				} else if (part == "..") {
					// go one up!
					Log.Debug ("PathHelper.NormalizePath: go up from ", newParts.Join ("/"));
					if (newParts.Count > 0) {
						newParts.RemoveAt (newParts.Count - 1);
					}
				} else if (string.IsNullOrEmpty (part)) {
					// empty path part!
				} else {
					// normal path part
					newParts.Add (part);
				}
			}

			path = (startsWithSlash ? "/" : string.Empty) + newParts.Join ("/");

			return path;
		}

		public static string[] NormalizePath (string[] pathParts)
		{
			return NormalizePath (pathParts.Join ("/")).Split ('/');
		}

		public static string CombinePath (params string[] parts)
		{
			if (parts == null || parts.Length == 0) {
				return string.Empty;
			}

			return NormalizePath (path: string.Join ("/", parts.Where (p => p.Length > 0)));
		}

		public static string CombinePath (string firstPart, params string[] otherParts)
		{
			if (firstPart == null || otherParts == null) {
				return string.Empty;
			}

			string[] parts = new[]{ firstPart }.Extend (otherParts);

			return CombinePath (parts: parts);
		}
	}
}

