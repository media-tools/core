using System;

namespace Core.Portable
{
	public static class UserInfo
	{
		public static void Assign (string userShortName, string userFullName, string hostName, string userMail)
		{
			UserFullName = userFullName ?? "Unknown User";
			UserShortName = userShortName ?? "unknown";
			HostName = hostName ?? "localhost";
			UserMail = userMail;
			UserAtHostName = userShortName != null && hostName != null ? string.Format ("{0}@{1}", userShortName, hostName) : userMail ?? "unknown@localhost";
		}

		public static string UserFullName { get; private set; }

		public static string UserShortName { get; private set; }

		public static string HostName { get; private set; }

		public static string UserMail { get; private set; }

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
		}

		public static string UserAtHostName { get; private set; }
	}
}

