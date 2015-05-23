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
	}
}

