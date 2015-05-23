using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public class LinuxFileSystem : RegularFileSystem
	{
		public LinuxFileSystem ()
		{
			AddPrefix ("/");
			DefaultRootDirectory = new RegularDirectory ("/", "");
		}

		protected override VirtualFile File (string prefix, string path)
		{
			return new RegularFile (prefix: prefix, path: path);
		}

		protected override VirtualDirectory Directory (string prefix, string path)
		{
			return new RegularDirectory (prefix: prefix, path: path);
		}
	}
}

