using System;
using Core.Common;

namespace Core.Shell.Platform.FileSystems
{
	/*public interface ILinuxFileSystemUtilities
	{
		LinuxFileAccessPermissions Permissions (LinuxFile file);

		string PermissionsString (LinuxFile file);

		LinuxFileAccessPermissions Permissions (LinuxDirectory file);

		string PermissionsString (LinuxDirectory file);
	}

	public static class LinuxPlatform
	{
		public static readonly ILinuxFileSystemUtilities Instance;

		static LinuxPlatform ()
		{
			try {
				Instance = (ILinuxFileSystemUtilities)Activator.CreateInstance ("Mono.Posix.dll", "Core.Shell.Platform.Linux.LinuxFileSystemUtilities").Unwrap ();
				Instance = (ILinuxFileSystemUtilities)Activator.CreateInstance ("Core.Shell.Platform.Linux.dll", "Core.Shell.Platform.Linux.LinuxFileSystemUtilities").Unwrap ();
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
				Log.FatalError (ex);
				Instance = null;
			}
		}
	}*/
}

