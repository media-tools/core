using System;
using Core.Portable;
using Core.Platform.Windows;
using System.IO;
using System.Linq;
using Core.Common;
using Core.IO;
using System.Security.Principal;
using Core.Platform.Linux;

namespace Core.Platform
{
	public static class DesktopPlatform
	{
		private static NonBlockingFile logfile;

		internal static readonly int LOG_TYPE_LENGTH = 7 + 2;

		private static bool isStarted = false;

		public static void Start ()
		{
			if (!isStarted) {
				isStarted = true;

				Assign ();

				if (!Core.Portable.PlatformInfo.System.IsRunningFromNUnit) {
					Log.LogHandler += (type, messageLines) => {
						//foreach (string message in messageLines) Console.WriteLine (message);
						if (LogTargets.StandardOutput) {
							foreach (string message in messageLines) {
								NonBlockingConsole.WriteLine (string.Format ("{0} {1}", formatType (type), message));
							}
						}
					};
				}

				Log.LogHandler += (type, messageLines) => {
					if (LogTargets.DefaultLogFile) {
						if (logfile == null) {
							logfile = new NonBlockingFile (Storage.DefaultLogFile);
						}
						foreach (string message in messageLines) {
							logfile.WriteLine (string.Format ("{0:yyyyMMdd-HHmmss} {1} {2}", DateTime.Now, formatType (type), message));
						}
					}
				};

			}
			/*if (type == Log.Type.FATAL_ERROR) {
					MessageBox.Show (message, "Fatal Error");
					Application.Exit ();
				} else {*/
		}

		static string formatType (Log.Type type)
		{
			return string.Format ("[{0}]", type).PadRight (LOG_TYPE_LENGTH);
		}

		public static void Finish ()
		{
			if (isStarted) {
				isStarted = false;
				NonBlockingConsole.Finish ();
				if (logfile != null) {
					logfile.Finish ();
				}
			}
		}

		public static class LogTargets
		{
			public static bool StandardOutput { get; set; } = true;

			public static bool DefaultLogFile { get; set; } = true;
		}

		private static void Assign ()
		{
			PlatformInfo.System = new DesktopSystemInfo ();
		}

	}

	public sealed class DesktopSystemInfo : ISystemInfo
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
				return false;
			}
		}

		public string ApplicationPath {
			get {
				return System.Reflection.Assembly.GetEntryAssembly ()?.Location;
			}
		}

		public string WorkingDirectory {
			get {
				return Environment.CurrentDirectory;
			}
		}

		public bool IsInteractive {
			get {
				return !isConsoleInvalid && !isConsoleSizeZero;
			}
		}

		#endregion


		private static bool isConsoleSizeZero { 
			get {
				try {
					return 0 == (Console.WindowHeight + Console.WindowWidth);
				} catch (IOException) {
					isConsoleInvalid = true;
					return true;
				}
			}
		}

		private static bool isConsoleInvalid = false;
	}

	public sealed class DesktopUserInfo : IUserInfo
	{
		#region IUserInfo implementation

		public string UserFullName {
			get {
				if (PlatformInfo.System.OperatingSystem == ModernOperatingSystem.Linux) {
					string home = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
					return (FileHelper.Instance as LinuxFileHelper).GetOwner_Linux (home).RealName;
				} else {
					WindowsIdentity wi = WindowsIdentity.GetCurrent ();
					return wi.Name;
				}
			}
		}

		public string UserShortName {
			get {
				return Environment.UserName;
			}
		}

		public string HostName {
			get {
				return Environment.MachineName;
			}
		}

		public string UserMail {
			get {
				string mail = null;
				if (PlatformInfo.System.OperatingSystem == ModernOperatingSystem.WindowsDesktop) {
					mail = WindowsRegistry.Windows8EmailAddress ();
				}
				return mail;
			}
		}

		public string HomeDirectory {
			get {
				return Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);
			}
		}

		#endregion

		//private static readonly bool isRunningFromNUnit =
		//	AppDomain.CurrentDomain.GetAssemblies ().Any (
		//		a => a.FullName.ToLowerInvariant ().StartsWith ("nunit.framework"));
	}
}

