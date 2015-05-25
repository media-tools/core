using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common;

namespace Core.Shell.Common.FileSystems
{
	public static class FileSystemHelper
	{
		public static string FormatNativePath (string nativePath)
		{
			// no windows file separators!
			nativePath = nativePath.Replace ('\\', '/').TrimEnd ('/');

			nativePath = FormatPathWithoutPrefix (path: nativePath, preserveFrontSlash: true);

			return nativePath;
		}

		public static string FormatPathWithoutPrefix (string path, bool preserveFrontSlash)
		{
			path = ResolvePath (path: path, preserveFrontSlash: preserveFrontSlash);

			if (path.ToCharArray ().Any (c => c != '/')) {
				path = path.TrimEnd ('/');
			}
			return path;
		}

		public static string CombinePath (bool preserveFrontSlash, params string[] parts)
		{
			if (parts.Length == 0) {
				return string.Empty;
			}

			// it doesn't really matter whether we set "preserveFrontSlash" to true or false
			return FormatPathWithoutPrefix (path: string.Join ("/", parts.Where (p => p.Length > 0)), preserveFrontSlash: preserveFrontSlash);
		}

		public static string CombinePath (IVirtualDirectory first, params string[] parts)
		{
			return first.VirtualPrefix + CombinePath (preserveFrontSlash: false, parts: first.VirtualPath.Extend (parts));
		}

		private static string ResolvePath (string path, bool preserveFrontSlash)
		{
			bool startsWithSlash = path.StartsWith ("/");

			string[] oldParts = path.Split (new []{ '/' }, StringSplitOptions.RemoveEmptyEntries);
			List<string> newParts = new List<string> ();
			foreach (string part in oldParts) {
				if (part == ".") {
					// ignore
				} else if (part == "..") {
					// go one up!
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

			path = newParts.Join ("/");
			if (preserveFrontSlash && startsWithSlash) {
				path = "/" + path;
			}
			return path;
		}
	}
}
