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

				if (!Core.Portable.PlatformInfo.System.IsRunningFromNUnit) {
					Log.LogHandler += (type, messageLines) => {
						foreach (string message in messageLines) {
							Console.WriteLine (string.Format ("{0} {1}", formatType (type), message));
						}
					};
				}

			}
		}

		public static void Finish ()
		{
		}

		static object formatType (Log.Type type)
		{
			string result = $"[{type}]";
			return result;
		}

		static void Assign ()
		{
			var dummy = new UnitTestPlatformInfo ();
			PlatformInfo.System = dummy;
			PlatformInfo.User = dummy;
		}

		class UnitTestPlatformInfo : ISystemInfo, IUserInfo
		{
			#region ISystemInfo implementation

			public ModernOperatingSystem OperatingSystem {
				get {
					ModernOperatingSystem os;
					if (Environment.OSVersion.Platform == PlatformID.Unix) {
						os = ModernOperatingSystem.Linux;
					} else {
						os = ModernOperatingSystem.WindowsDesktop;
					}
					return os;
				}
			}

			public bool IsRunningFromNUnit {
				get {
					return true;
				}
			}

			public string ApplicationPath {
				get {
					return null;
				}
			}

			public string WorkingDirectory {
				get {
					return "/nonexistent";
				}
			}

			public bool IsInteractive {
				get {
					return false;
				}
			}

			#endregion

			#region IUserInfo implementation

			public string UserFullName {
				get {
					return "Test User";
				}
			}

			public string UserShortName {
				get {
					return "testuser";
				}
			}

			public string HostName {
				get {
					return "hostname";
				}
			}

			public string UserMail {
				get {
					return "fuckyou@test.com";
				}
			}

			public string HomeDirectory {
				get {
					return "/nonexistent";
				}
			}

			#endregion
		}
	}
}

