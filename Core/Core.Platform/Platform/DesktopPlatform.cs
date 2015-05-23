using System;
using Core.Portable;
using Core.Platform.Windows;
using System.IO;
using System.Linq;
using Core.Common;
using Core.IO;

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

				if (!Core.Portable.SystemInfo.IsRunningFromNUnit) {
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
			NonBlockingConsole.Finish ();
			if (logfile != null) {
				logfile.Finish ();
			}
		}

		public static class LogTargets
		{
			public static bool StandardOutput { get; set; } = true;

			public static bool DefaultLogFile { get; set; } = true;
		}

		private static void Assign ()
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

			string applicationPath = System.Reflection.Assembly.GetEntryAssembly ()?.Location;
			string workingDirectory = Environment.CurrentDirectory;

			SystemInfo.Assign (
				operatingSystem: os,
				applicationPath: applicationPath,
				workingDirectory: workingDirectory,
				isInteractive: IsInteractive,
				isRunningFromNUnit: false
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

		//private static readonly bool isRunningFromNUnit =
		//	AppDomain.CurrentDomain.GetAssemblies ().Any (
		//		a => a.FullName.ToLowerInvariant ().StartsWith ("nunit.framework"));

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

			string homeDirectory = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile);

			UserInfo.Assign (
				userShortName: userShortName,
				userFullName: null,
				hostName: hostName,
				userMail: mail,
				homeDirectory: homeDirectory
			);
		}
	}
}

