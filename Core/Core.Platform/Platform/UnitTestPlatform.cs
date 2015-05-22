using System;
using Core.Common;
using Core.Portable;

namespace Core.Platform
{
	public static class UnitTestPlatform
	{
		private static bool isStarted = false;

		public static void Start ()
		{
			if (!isStarted) {
				isStarted = true;

				Assign ();

				if (!Core.Portable.SystemInfo.IsRunningFromNUnit) {
					Log.LogHandler += (type, messageLines) => {
						foreach (string message in messageLines) {
							Console.WriteLine (string.Format ("{0} {1}", formatType (type), message));
						}
					};
				}

			}
		}

		static object formatType (Log.Type type)
		{
			string result = $"[{type}]";
			return result;
		}

		static void Assign ()
		{
			ModernOperatingSystem os;
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				os = ModernOperatingSystem.Linux;
			} else {
				os = ModernOperatingSystem.WindowsDesktop;
			}

			SystemInfo.Assign (
				operatingSystem: os,
				applicationPath: null,
				isInteractive: () => false,
				isRunningFromNUnit: true
			);

			UserInfo.Assign (
				userShortName: "testuser",
				userFullName: "Test User",
				hostName: "hostname",
				userMail: "fuckyou@test.com"
			);
		}

		public static void Finish ()
		{
		}
	}
}

