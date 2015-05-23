using System;
using Core.Shell.Common.FileSystems;

namespace Core.Shell.Platform.FileSystems
{
	public class LinuxFile : RegularFile
	{
		static ILinuxFileUtilities util { get { return LinuxPlatform.Instance; } }

		public LinuxFile (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix, path, fileSystem)
		{
		}
	}

	public class LinuxDirectory : RegularDirectory
	{
		static ILinuxFileUtilities util { get { return LinuxPlatform.Instance; } }

		public LinuxDirectory (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix, path, fileSystem)
		{
		}
	}

	public class WindowsFile : RegularFile
	{
		public WindowsFile (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix, path, fileSystem)
		{
		}
	}

	public class WindowsDirectory : RegularDirectory
	{
		public WindowsDirectory (string prefix, string path, RegularFileSystem fileSystem)
			: base (prefix, path, fileSystem)
		{
		}
	}
}

