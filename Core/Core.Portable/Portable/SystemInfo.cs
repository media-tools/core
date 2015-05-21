using System;
using System.IO;

namespace Core.Portable
{
	public static class SystemInfo
	{
		public static void Assign (ModernOperatingSystem operatingSystem, string applicationPath, Func<bool> isInteractive)
		{
			SystemInfo._operatingSystem = operatingSystem;
			SystemInfo.ApplicationPath = applicationPath;
			SystemInfo._isInteractive = isInteractive;
		}


		public static bool IsRunningOnMono ()
		{
			return Type.GetType ("Mono.Runtime") != null;
		}

		private static ModernOperatingSystem _operatingSystem = ModernOperatingSystem.Undefined;

		public static ModernOperatingSystem OperatingSystem {
			get {
				if (_operatingSystem != ModernOperatingSystem.Undefined) {
					return _operatingSystem;
				} else {
					throw new ArgumentException ("No Platform Info assigned to portable library!");
				}
			}
		}

		public static string ApplicationPath { get; private set; }

		private static Func<bool> _isInteractive = null;

		public static bool IsInteractive {
			get { return _isInteractive (); }
		}
	}

	public enum ModernOperatingSystem
	{
		Linux,
		WindowsDesktop,
		WinRT,
		Undefined,
	}
}

