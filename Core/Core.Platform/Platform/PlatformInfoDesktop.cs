using System;
using Core.Portable;
using Core.Platform.Windows;
using System.IO;

namespace Core.Platform
{
	public static class PlatformInfoDesktop
	{
		public static void Assign ()
		{
			assignSystemInfo ();
			assignUserInfo ();
		}

		private static void assignSystemInfo ()
		{
			ModernOperatingSystem os;
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				os = ModernOperatingSystem.Linux;
			} else {
				os = ModernOperatingSystem.WindowsDesktop;
			}

			SystemInfo.Assign (
				operatingSystem: os,
				applicationPath: System.Reflection.Assembly.GetEntryAssembly ().Location,
				isInteractive: IsInteractive
			);
		}


		private static bool isConsoleSizeZero { 
			get {
				try {
					return 0 == (Console.WindowHeight + Console.WindowWidth);
				} catch (IOException ex) {
					isConsoleInvalid = true;
					return true;
				}
			}
		}

		private static bool isConsoleInvalid = false;

		private static bool IsInteractive ()
		{
			return !isConsoleInvalid && !isConsoleSizeZero;
		}

		private static void assignUserInfo ()
		{
			//Console.WriteLine ("UserName: {0}", Environment.UserName);
			//Console.WriteLine ("UserDomainName: {0}", Environment.UserDomainName);
			//Console.WriteLine ("MachineName: {0}", Environment.MachineName);

			string userShortName = Environment.UserName;
			string hostName = Environment.MachineName;

			string mail;
			if (SystemInfo.OperatingSystem == ModernOperatingSystem.WindowsDesktop) {
				mail = WindowsRegistry.Windows8EmailAddress ();
			} else {
				mail = userShortName + "@" + hostName;
			}

			UserInfo.Assign (
				userShortName: userShortName,
				userFullName: null,
				hostName: hostName,
				userMail: mail
			);
		}
	}
}

