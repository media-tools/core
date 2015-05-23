using System;
using Core.Portable;
using Core.Shell.Common.FileSystems;
using Core.Common;

namespace Core.Shell.Platform.FileSystems
{
	public static class RegularFileSystems
	{
		public static void Register ()
		{
			FileSystemSubsystem fs = null;
			if (SystemInfo.OperatingSystem == ModernOperatingSystem.Linux) {
				fs = new LinuxFileSystem ();
			} else if (SystemInfo.OperatingSystem == ModernOperatingSystem.WindowsDesktop) {
				fs = new WindowsFileSystem ();
			}

			if (fs != null) {
				FileSystemSubsystems.Subsystems.Add (fs);
				FileSystemSubsystems.DefaultRootDirectory = fs.DefaultRootDirectory;
			}
		}
	}
}

