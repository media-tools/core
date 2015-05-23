using System;
using Core.Common;

namespace Core.Shell.Platform.FileSystems
{
	public interface ILinuxFileUtilities
	{
		
	}

	public static class LinuxPlatform
	{
		public static readonly ILinuxFileUtilities Instance;

		static LinuxPlatform ()
		{
			try {
				Instance = (ILinuxFileUtilities)Activator.CreateInstance ("Core.Shell.Platform.Linux", "Core.Shell.Platform.Linux.LinuxFileUtilities").Unwrap ();
			} catch (Exception ex) {
				Log.FatalError (ex);
				Instance = null;
			}
		}
	}
}

