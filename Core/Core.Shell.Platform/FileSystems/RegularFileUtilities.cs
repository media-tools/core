using System;
using System.IO;

namespace Core.Shell.Platform.FileSystems
{
	public static class RegularFileUtilities
	{
		public static bool IsDirectory (string realPath)
		{
			// get the file attributes for file or directory
			FileAttributes attr = File.GetAttributes (realPath);

			// detect whether its a directory or file
			return attr.HasFlag (FileAttributes.Directory);
		}
	}
}