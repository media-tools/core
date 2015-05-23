using System;
using System.IO;

namespace Core.Portable
{
	public static class PlatformInfo
	{
		public static ISystemInfo System { get; set; }

		public static IUserInfo User { get; set; }

		/*
		public bool IsRunningOnMono ()
		{
			return Type.GetType ("Mono.Runtime") != null;
		}
		*/
	}

	public interface ISystemInfo
	{
		ModernOperatingSystem OperatingSystem { get; }

		bool IsRunningFromNUnit { get; }

		string ApplicationPath { get; }

		string WorkingDirectory { get; }

		bool IsInteractive { get; }
	}

	public enum ModernOperatingSystem
	{
		Linux,
		WindowsDesktop,
		WinRT,
		Undefined,
	}

	public interface IUserInfo
	{
		string UserFullName { get; }

		string UserShortName { get; }

		string HostName { get; }

		string UserMail { get; }

		string HomeDirectory { get; }

		/*
		public static string FullNameAndMail {
			get {
				string result;
				if (!string.IsNullOrWhiteSpace (UserFullName) && !string.IsNullOrWhiteSpace (UserMail)) {
					result = $"{UserFullName} <{UserMail}>";
				} else if (!string.IsNullOrWhiteSpace (UserFullName)) {
					result = UserFullName;
				} else if (!string.IsNullOrWhiteSpace (UserMail)) {
					result = UserMail;
				} else {
					result = "Unknown User";
				}
				return result;
			}
		}*/
	}
}

