using System;
using Core.Common;

namespace Core.Platform
{
	public static class SystemInfo
	{
		public static bool IsRunningOnMono ()
		{
			return Type.GetType ("Mono.Runtime") != null;
		}

		public static bool IsRunningOnLinux ()
		{
			return Environment.OSVersion.Platform == PlatformID.Unix;
		}

		public static bool IsRunningOnWindows ()
		{
			return !IsRunningOnLinux ();
		}

		public static string ApplicationPath { get { return System.Reflection.Assembly.GetEntryAssembly ().Location; } }

		private static bool isConsoleSizeZero { 
			get {
				return 0 == (Console.WindowHeight + Console.WindowWidth);
			}
		}

		public static bool IsInteractive {
			get { return !isConsoleSizeZero; }
		}
	}
}

