using System;
using Core.Portable;
using Core.Platform.Windows;
using System.IO;
using System.Linq;
using Core.Common;

namespace Core.Platform
{
	public static class PlatformInfoDesktop
	{
		private static bool done = false;

		public static void Assign ()
		{
			if (!done) {
				assignSystemInfo ();
				assignUserInfo ();
				done = true;
			}
			Console.WriteLine ("PlatformInfoDesktop.Assign finished!");
		}

		private static void assignSystemInfo ()
		{
			ModernOperatingSystem os;
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				os = ModernOperatingSystem.Linux;
			} else {
				os = ModernOperatingSystem.WindowsDesktop;
			}

			string applicationPath = System.Reflection.Assembly.GetEntryAssembly ()?.Location;

			SystemInfo.Assign (
				operatingSystem: os,
				applicationPath: applicationPath,
				isInteractive: IsInteractive,
				isRunningFromNUnit: isRunningFromNUnit
			);
		}


		private static bool isConsoleSizeZero { 
			get {
				if (isRunningFromNUnit) { 
					return true;
				} else {
					try {
						return 0 == (Console.WindowHeight + Console.WindowWidth);
					} catch (IOException ex) {
						isConsoleInvalid = true;
						return true;
					}
				}
			}
		}

		private static bool isConsoleInvalid = false;

		private static bool IsInteractive ()
		{
			return !isRunningFromNUnit && !isConsoleInvalid && !isConsoleSizeZero;
		}

		private static readonly bool isRunningFromNUnit = 
			AppDomain.CurrentDomain.GetAssemblies ().Any (
				a => a.FullName.ToLowerInvariant ().StartsWith ("nunit.framework"));

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

