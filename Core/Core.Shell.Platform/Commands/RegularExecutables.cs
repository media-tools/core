using System;
using Core.Portable;
using Core.Shell.Common.Commands;

namespace Core.Shell.Platform.Commands
{
	public static class RegularExecutables
	{
		public static void Register ()
		{
			RegularExecutableSubsystem exes = null;
			if (PlatformInfo.System.OperatingSystem == ModernOperatingSystem.Linux) {
				exes = new LinuxExecutableSubsystem ();
			} else if (PlatformInfo.System.OperatingSystem == ModernOperatingSystem.WindowsDesktop) {
				exes = new WindowsExecutableSubsystem ();
			}

			if (exes != null) {
				CommandSubsystems.Subsystems.Add (exes);
			}
		}
	}
}

